# PromptSync Browser Extension - Harvest Mode

## Overview
Chrome/Firefox extension for capturing prompts from the web with one click.

## Features
- Highlight text → Right-click → "Harvest Prompt"
- Hotkey: Ctrl+Shift+H (customizable)
- Preview before saving
- Auto-categorization
- Direct GitHub commit

## File Structure
```
extension/
├── manifest.json           # Extension config
├── background.js          # Service worker
├── content.js             # Page interaction
├── popup/
│   ├── popup.html        # Extension popup UI
│   ├── popup.js          # Popup logic
│   └── popup.css         # Styles
└── assets/
    ├── icon16.png
    ├── icon48.png
    └── icon128.png
```

## manifest.json
```json
{
  "manifest_version": 3,
  "name": "PromptSync Harvester",
  "version": "0.1.0",
  "description": "Capture and sync prompts from anywhere on the web",
  "permissions": [
    "activeTab",
    "contextMenus",
    "storage"
  ],
  "host_permissions": [
    "http://*/*",
    "https://*/*"
  ],
  "background": {
    "service_worker": "background.js"
  },
  "content_scripts": [
    {
      "matches": ["<all_urls>"],
      "js": ["content.js"]
    }
  ],
  "action": {
    "default_popup": "popup/popup.html",
    "default_icon": {
      "16": "assets/icon16.png",
      "48": "assets/icon48.png",
      "128": "assets/icon128.png"
    }
  },
  "commands": {
    "harvest-selection": {
      "suggested_key": {
        "default": "Ctrl+Shift+H",
        "mac": "Command+Shift+H"
      },
      "description": "Harvest prompt from selection"
    }
  }
}
```

## background.js
```javascript
// Service worker for PromptSync extension

// Create context menu
chrome.runtime.onInstalled.addListener(() => {
  chrome.contextMenus.create({
    id: 'harvest-prompt',
    title: 'Harvest Prompt',
    contexts: ['selection']
  });
});

// Handle context menu clicks
chrome.contextMenus.onClicked.addListener((info, tab) => {
  if (info.menuItemId === 'harvest-prompt') {
    harvestSelection(info.selectionText, tab.url);
  }
});

// Handle keyboard shortcut
chrome.commands.onCommand.addListener((command) => {
  if (command === 'harvest-selection') {
    // Get selection from active tab
    chrome.tabs.query({active: true, currentWindow: true}, (tabs) => {
      chrome.tabs.sendMessage(tabs[0].id, {action: 'getSelection'}, (response) => {
        if (response && response.selection) {
          harvestSelection(response.selection, tabs[0].url);
        }
      });
    });
  }
});

async function harvestSelection(text, url) {
  // Send to PromptSync backend
  const extracted = {
    content: text,
    url: url,
    extracted_at: new Date().toISOString()
  };
  
  try {
    // Call local PromptSync server (or direct GitHub API)
    const response = await fetch('http://localhost:5000/api/harvest', {
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      body: JSON.stringify(extracted)
    });
    
    const result = await response.json();
    
    // Show notification
    chrome.notifications.create({
      type: 'basic',
      iconUrl: 'assets/icon48.png',
      title: 'Prompt Harvested',
      message: `Captured "${result.title}" (Confidence: ${result.confidence}%)`
    });
    
    // Open preview popup
    chrome.windows.create({
      url: chrome.runtime.getURL('popup/preview.html') + '?id=' + result.id,
      type: 'popup',
      width: 600,
      height: 800
    });
    
  } catch (error) {
    console.error('Harvest failed:', error);
    chrome.notifications.create({
      type: 'basic',
      iconUrl: 'assets/icon48.png',
      title: 'Harvest Failed',
      message: error.message
    });
  }
}
```

## content.js
```javascript
// Content script for page interaction

// Listen for messages from background
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  if (request.action === 'getSelection') {
    const selection = window.getSelection().toString();
    sendResponse({selection: selection});
  }
});

// Add visual feedback when hovering over harvestable content
document.addEventListener('mouseup', () => {
  const selection = window.getSelection();
  if (selection.toString().length > 10) {
    // Show mini harvest button near selection
    showHarvestButton(selection);
  }
});

function showHarvestButton(selection) {
  // Remove existing button
  const existing = document.getElementById('promptsync-harvest-btn');
  if (existing) existing.remove();
  
  // Create floating button
  const button = document.createElement('div');
  button.id = 'promptsync-harvest-btn';
  button.innerHTML = '📋 Harvest';
  button.style.cssText = `
    position: absolute;
    background: #4F46E5;
    color: white;
    padding: 6px 12px;
    border-radius: 6px;
    font-size: 12px;
    cursor: pointer;
    z-index: 10000;
    box-shadow: 0 2px 8px rgba(0,0,0,0.2);
  `;
  
  // Position near selection
  const range = selection.getRangeAt(0);
  const rect = range.getBoundingClientRect();
  button.style.top = (window.scrollY + rect.bottom + 5) + 'px';
  button.style.left = (window.scrollX + rect.left) + 'px';
  
  // Click handler
  button.addEventListener('click', () => {
    chrome.runtime.sendMessage({
      action: 'harvest',
      text: selection.toString(),
      url: window.location.href
    });
    button.remove();
  });
  
  document.body.appendChild(button);
  
  // Auto-remove after 3 seconds
  setTimeout(() => button.remove(), 3000);
}
```

## popup/popup.html
```html
<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <title>PromptSync Harvester</title>
  <link rel="stylesheet" href="popup.css">
</head>
<body>
  <div class="container">
    <header>
      <h1>🧬 PromptSync</h1>
      <span class="version">v0.1.0</span>
    </header>
    
    <div class="stats">
      <div class="stat">
        <span class="label">Harvested Today:</span>
        <span class="value" id="today-count">0</span>
      </div>
      <div class="stat">
        <span class="label">Total Library:</span>
        <span class="value" id="total-count">0</span>
      </div>
    </div>
    
    <div class="actions">
      <button id="harvest-page" class="btn btn-primary">
        📄 Harvest This Page
      </button>
      <button id="open-library" class="btn btn-secondary">
        📚 View Library
      </button>
      <button id="settings" class="btn btn-secondary">
        ⚙️ Settings
      </button>
    </div>
    
    <div class="recent">
      <h3>Recent Harvests</h3>
      <div id="recent-list">
        <!-- Populated by JS -->
      </div>
    </div>
    
    <div class="quick-help">
      <p><strong>Quick Tips:</strong></p>
      <ul>
        <li>Highlight text → Right-click → "Harvest Prompt"</li>
        <li>Or press <kbd>Ctrl+Shift+H</kbd></li>
        <li>Preview before saving to GitHub</li>
      </ul>
    </div>
  </div>
  
  <script src="popup.js"></script>
</body>
</html>
```

## popup/popup.css
```css
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  width: 350px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  background: #F9FAFB;
}

.container {
  padding: 16px;
}

header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

h1 {
  font-size: 18px;
  color: #111827;
}

.version {
  font-size: 11px;
  color: #6B7280;
  background: #E5E7EB;
  padding: 2px 6px;
  border-radius: 4px;
}

.stats {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 16px;
}

.stat {
  background: white;
  padding: 12px;
  border-radius: 8px;
  border: 1px solid #E5E7EB;
}

.stat .label {
  display: block;
  font-size: 11px;
  color: #6B7280;
  margin-bottom: 4px;
}

.stat .value {
  display: block;
  font-size: 24px;
  font-weight: 600;
  color: #4F46E5;
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
}

.btn {
  padding: 10px 16px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #4F46E5;
  color: white;
}

.btn-primary:hover {
  background: #4338CA;
}

.btn-secondary {
  background: white;
  color: #374151;
  border: 1px solid #D1D5DB;
}

.btn-secondary:hover {
  background: #F3F4F6;
}

.recent h3 {
  font-size: 14px;
  color: #374151;
  margin-bottom: 8px;
}

#recent-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.recent-item {
  background: white;
  padding: 10px;
  border-radius: 6px;
  border: 1px solid #E5E7EB;
  font-size: 13px;
}

.recent-item .title {
  font-weight: 500;
  color: #111827;
  margin-bottom: 4px;
}

.recent-item .meta {
  font-size: 11px;
  color: #6B7280;
}

.quick-help {
  margin-top: 16px;
  padding: 12px;
  background: #EEF2FF;
  border-radius: 6px;
  border: 1px solid #C7D2FE;
}

.quick-help p {
  font-size: 12px;
  font-weight: 500;
  color: #4F46E5;
  margin-bottom: 8px;
}

.quick-help ul {
  list-style: none;
  font-size: 12px;
  color: #4338CA;
}

.quick-help li {
  padding: 4px 0;
}

kbd {
  background: white;
  border: 1px solid #C7D2FE;
  border-radius: 3px;
  padding: 2px 4px;
  font-size: 10px;
  font-family: monospace;
}
```

## Installation Instructions

### For Development:
1. Clone the repo
2. Open Chrome → `chrome://extensions/`
3. Enable "Developer mode"
4. Click "Load unpacked"
5. Select the `extension/` folder

### For Users:
- Chrome Web Store (coming soon)
- Firefox Add-ons (coming soon)

## Local Server Setup

The extension needs a local PromptSync server running:

```bash
# In promptsync directory
python -m src.server.harvest_api

# Runs on http://localhost:5000
```

## Privacy & Ethics

- **Respects robots.txt**: Checks before harvesting
- **User consent**: Always previews before saving
- **Source attribution**: Includes original URL
- **Local-first**: Data stays on your machine
- **No tracking**: Zero telemetry or analytics

## Roadmap

- [ ] Firefox support
- [ ] Drag-to-harvest images
- [ ] Bulk harvest (RSS feeds)
- [ ] Team sharing
- [ ] Mobile companion
