# GitHub Copilot Instructions for PromptSync

**Add these after uploading the code to GitHub**

Go to: Settings → Copilot → Repository instructions

---

## Instructions to Paste (497 chars)

```
PromptSync: AI workflow platform managing prompts as intelligent, version-controlled assets in Git. DNA Lab features: reverse engineering, auto-iteration, security scanning, quality scoring, encryption, web harvesting. Composable workflows chain prompts into pipelines. Context-aware matching. Python 3.8+. Follow PROJECT_INSTRUCTIONS.md for Git-first, safety-by-default patterns. Modular architecture: src/dna/ (intelligence), src/workflow/ (automation), src/eval/ (testing). Performance: <200ms hotkey response, <1s quality checks.
```

---

## What This Does

**Copilot will:**
- ✅ Suggest code following PromptSync patterns
- ✅ Respect Git-first principles
- ✅ Auto-add security checks
- ✅ Follow performance constraints
- ✅ Generate modular, composable code
- ✅ Reference PROJECT_INSTRUCTIONS.md

**Example:**
```python
# You type:
def new_dna_feature

# Copilot suggests:
def new_dna_feature(self, input_data: Dict[str, Any]) -> Dict[str, Any]:
    """
    New DNA Lab feature following PromptSync patterns
    
    Args:
        input_data: Dict with 'content', 'context', etc.
    
    Returns:
        Dict with 'success': bool, 'data': Any, 'confidence': int
    """
    # Safety check first (following safety-by-default)
    if not self._validate_input(input_data):
        return {'success': False, 'error': 'Invalid input'}
    
    # Your feature logic...
```

---

## Alternative: Shorter Version (If 500 char limit is strict)

```
Python AI workflow platform. Git-native prompt management. DNA Lab: reverse engineering, iteration, security, quality, harvesting. Modular: src/dna/ (intelligence), src/workflow/ (automation). Safety-first, performance: <200ms. Follow PROJECT_INSTRUCTIONS.md patterns.
```

**Length:** 267 characters

---

## When to Add

**Best Time:** Right after your first push

**Steps:**
1. Push all code to GitHub ✅
2. Go to repo Settings
3. Click "Copilot" in sidebar  
4. Click "Add instructions"
5. Paste the instructions above
6. Save

**Now Copilot understands PromptSync!** 🤖

---

## Testing Copilot

After adding instructions, try creating a new file:

```python
# test_copilot.py
from src.dna import 

# Copilot should suggest all DNA modules correctly
```

It should understand:
- Your architecture (src/dna/, src/workflow/)
- Your patterns (Dict returns with success/data)
- Your constraints (performance, safety)
- Your style (following PROJECT_INSTRUCTIONS.md)

---

**Enjoy AI-powered PromptSync development!** ✨
