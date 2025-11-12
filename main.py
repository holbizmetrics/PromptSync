#!/usr/bin/env python3
"""
PromptSync - GitHub-Synced Prompt Manager with DNA Analysis
Main entry point for the application
"""

import sys
import time
import threading
from pathlib import Path

# Core modules
from src.github_sync import GitHubSync
from src.hotkey_listener import HotkeyListener
from src.matcher import PromptMatcher
from src.ui import PromptUI
from src.config import load_config

# DNA modules
from src.dna.reverse_engineer import ReverseEngineer
from src.dna.iterator import PromptIterator
from src.dna.security_check import SecurityChecker
from src.dna.quality_score import QualityScorer
from src.dna.encryptor import PromptEncryptor

class PromptSync:
    """Main application controller"""
    
    def __init__(self):
        self.config = load_config()
        self.setup_components()
    
    def setup_components(self):
        """Initialize all components"""
        print("🚀 Starting PromptSync...")
        
        # Core components
        self.github = GitHubSync(
            token=self.config['github']['token'],
            repo=self.config['github']['repo'],
            local_path=self.config['local_path']
        )
        
        self.matcher = PromptMatcher(self.config['local_path'])
        self.ui = PromptUI()
        self.hotkey = HotkeyListener(
            hotkey=self.config['hotkey'],
            callback=self.handle_hotkey
        )
        
        # DNA components
        self.reverse_engineer = ReverseEngineer()
        self.iterator = PromptIterator()
        self.security = SecurityChecker()
        self.quality = QualityScorer()
        self.encryptor = PromptEncryptor()
        
        print("✅ All components initialized")
    
    def handle_hotkey(self):
        """Called when user presses the hotkey"""
        print("🔥 Hotkey triggered!")
        
        # Get current context
        context = self.matcher.get_current_context()
        
        # Find matching prompts
        matches = self.matcher.find_matching_prompts(context)
        
        # Show UI with matches
        selected = self.ui.show_prompt_selector(matches, context)
        
        if selected:
            # User selected a prompt
            self.use_prompt(selected)
    
    def use_prompt(self, prompt):
        """Execute the selected prompt"""
        content = prompt['content']
        
        # Check if encrypted
        if self.encryptor.is_encrypted(content):
            print("🔒 Prompt is encrypted, decrypting...")
            content = self.encryptor.decrypt(content)
        
        # Copy to clipboard
        import pyperclip
        pyperclip.copy(content)
        print("📋 Prompt copied to clipboard!")
    
    def sync_background(self):
        """Background thread for GitHub sync"""
        while True:
            try:
                print("🔄 Syncing with GitHub...")
                self.github.pull()
                print("✅ Sync complete")
            except Exception as e:
                print(f"❌ Sync error: {e}")
            
            time.sleep(self.config['github']['sync_interval'])
    
    def run(self):
        """Start the application"""
        print("""
╔═══════════════════════════════════════╗
║         PromptSync v0.1.0             ║
║  GitHub-Synced Prompt Manager + DNA   ║
╚═══════════════════════════════════════╝
        """)
        
        # Initial sync
        print("📥 Performing initial sync...")
        self.github.pull()
        
        # Start background sync thread
        sync_thread = threading.Thread(target=self.sync_background, daemon=True)
        sync_thread.start()
        
        # Start hotkey listener
        print(f"⌨️  Listening for hotkey: {self.config['hotkey']}")
        print("Press Ctrl+C to exit\n")
        
        try:
            self.hotkey.listen()
        except KeyboardInterrupt:
            print("\n👋 Shutting down PromptSync...")
            sys.exit(0)

def main():
    """Entry point"""
    app = PromptSync()
    app.run()

if __name__ == '__main__':
    main()
