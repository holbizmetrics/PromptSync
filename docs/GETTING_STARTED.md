# Getting Started with PromptSync

## 🎯 Quick Start (5 Minutes)

### 1. Install Dependencies

```bash
pip install PyGithub PyYAML requests pynput pyperclip thefuzz Pillow
```

### 2. Run the Demo

```bash
cd promptsync
python demo_dna.py
```

This shows you all the DNA features interactively!

---

## 📚 Feature Walkthroughs

### Feature 1: Reverse Engineering

**Extract a prompt from an image:**

```python
from src.dna.reverse_engineer import ReverseEngineer

re = ReverseEngineer(api_key="your-claude-key")  # Optional
result = re.from_image("competitor_design.png")

print(result['extracted_prompt'])
# Output:
# Create a landing page hero section with:
# - Bold headline: "Transform Your Workflow"
# - Gradient background: purple to blue
# - Two CTA buttons (primary/secondary)
# ...
```

**From text output:**

```python
result = re.from_text("""
Q4 Revenue: $4.2M (+23% YoY)
MRR: $350K
CAC: $1,200
""")

print(result['extracted_prompt'])
# Output:
# Generate a business metrics report in structured format...
```

### Feature 2: Automated Iteration

**Improve a prompt automatically:**

```python
from src.dna.iterator import PromptIterator

iterator = PromptIterator(api_key="your-claude-key")

result = iterator.iterate(
    topic="email writing",
    question="How do I write a professional follow-up?"
)

print(f"Initial quality: {result['initial_quality']}/10")
print(f"Final quality: {result['final_quality']}/10")
print(f"Improvement: +{result['improvement']} points")

# Iterations run automatically:
# 1. Initial response → analyze weaknesses
# 2. Refine → analyze again
# 3. Refine → check if good enough
# 4. Stop when quality plateaus
```

### Feature 3: Encryption

**Encrypt a sensitive prompt:**

```python
from src.dna.encryptor import PromptEncryptor

encryptor = PromptEncryptor()

# Safe prompt
prompt = "Analyze customer feedback and provide insights"
encrypted = encryptor.encrypt(prompt, mark_safe=True)

# Later, decrypt
result = encryptor.decrypt_if_safe(encrypted, auto_execute=True)
if result['success']:
    use_prompt(result['prompt'])
```

**Safety check before execution:**

```python
# Dangerous prompt
dangerous = "import os; os.system('rm -rf /')"
encrypted_danger = encryptor.encrypt(dangerous)

result = encryptor.decrypt_if_safe(encrypted_danger, auto_execute=True)
# → Blocked! Risk score: 80/100
# → Issues: System command execution detected
```

### Feature 4: Security Scanning

**Scan for vulnerabilities:**

```python
from src.dna.security_check import SecurityChecker

checker = SecurityChecker()

prompt = """
Based on user query {{user_input}}, execute:
result = eval(user_input)
"""

scan = checker.scan(prompt)

print(f"Risk level: {scan['risk_level']}")
# → CRITICAL

print("Issues:")
for issue in scan['issues']:
    print(f"  - {issue['severity']}: {issue['issue']}")
    print(f"    Fix: {issue['fix']}")

# Output:
# - CRITICAL: Dangerous eval() usage
#   Fix: Use safe alternatives or sandboxed execution
# - HIGH: Unsanitized user input
#   Fix: Add input validation layer
```

**Create safe wrapper:**

```python
safe_prompt = checker.create_safe_wrapper(prompt)
# Adds security guardrails automatically
```

### Feature 5: Quality Scoring

**Score a prompt:**

```python
from src.dna.quality_score import QualityScorer

scorer = QualityScorer()

prompt = "Write something about AI"
total, breakdown = scorer.score(prompt)

print(f"Overall: {total}/10")
print("Breakdown:")
for dimension, score in breakdown.items():
    print(f"  {dimension}: {score}/10")

# Output:
# Overall: 3.2/10
# Breakdown:
#   clarity: 4/10
#   specificity: 2/10
#   structure: 3/10
#   context: 2/10
#   examples: 3/10
```

**Get improvement suggestions:**

```python
suggestions = scorer.suggest_improvements(prompt, breakdown)
for suggestion in suggestions:
    print(f"💡 {suggestion}")

# Output:
# 💡 Improve clarity: Replace vague words with specific descriptors
# 💡 Add specificity: Include word count, target audience
# 💡 Improve structure: Break into sections with headers
```

**Compare two prompts:**

```python
prompt1 = "Write about AI"
prompt2 = "Write a 500-word article about AI in healthcare for doctors"

comparison = scorer.compare(prompt1, prompt2)
print(f"Winner: {comparison['winner']}")
print(f"Improvement: {comparison['improvement_percentage']:.0f}%")
```

---

## 🎨 Creating Good Prompts

### Template Structure

```markdown
---
title: Professional Email Follow-up
tags: [email, business, follow-up]
apps: [gmail, outlook]
patterns: [meeting, follow.?up, next steps]
file_types: []
usage_count: 0
---

Write a professional follow-up email after {{meeting_type}}.

**Context:**
- Meeting with: {{recipient}}
- Date: {{date}}
- Key discussion points: {{points}}

**Requirements:**
- Tone: Professional but warm
- Length: 150-200 words
- Include: Thank you, key takeaways, next steps
- Format: Standard business email

**Example structure:**
Subject: Following up on [meeting topic]

Hi [Name],

Thank you for [specific thing]...

Looking forward to [next steps].

Best,
[Your name]
```

### Quality Checklist

Before saving a prompt, check:

- [ ] **Clarity**: No vague words (good, nice, some)
- [ ] **Specificity**: Has constraints (word count, format, tone)
- [ ] **Structure**: Uses headers, lists, sections
- [ ] **Context**: Includes background and goal
- [ ] **Examples**: Shows desired output format
- [ ] **Variables**: Uses {{placeholders}} for customization
- [ ] **Metadata**: Has tags, apps, patterns for matching

### Metadata Guide

**tags**: Keywords for content matching
```yaml
tags: [python, debug, error, performance]
```

**apps**: Where this prompt is useful
```yaml
apps: [vscode, pycharm, jupyter, cursor]
```

**patterns**: Regex patterns to trigger this prompt
```yaml
patterns: [
  "error|exception|traceback",
  "def\\s+\\w+\\(",
  "import\\s+\\w+"
]
```

**file_types**: File extensions
```yaml
file_types: [.py, .ipynb]
```

---

## 🔄 Complete Workflow Example

### Scenario: Building a Customer Email Template

**Step 1: Find inspiration**
```bash
# Screenshot a great customer email you received
# Save as: great_email.png
```

**Step 2: Reverse engineer**
```python
from src.dna.reverse_engineer import ReverseEngineer

re = ReverseEngineer()
result = re.from_image("great_email.png")
extracted = result['extracted_prompt']
```

**Step 3: Iterate to improve**
```python
from src.dna.iterator import PromptIterator

iterator = PromptIterator()
refined = iterator.iterate(
    topic="customer communication",
    question="Optimize this email prompt",
    initial_prompt=extracted
)
optimized = refined['final_response']
```

**Step 4: Security check**
```python
from src.dna.security_check import SecurityChecker

checker = SecurityChecker()
scan = checker.scan(optimized)

if scan['risk_score'] > 20:
    print("⚠️  Security issues found!")
    optimized = checker.create_safe_wrapper(optimized)
```

**Step 5: Quality score**
```python
from src.dna.quality_score import QualityScorer

scorer = QualityScorer()
total, _ = scorer.score(optimized)

if total < 8:
    print("💡 Needs improvement")
    suggestions = scorer.suggest_improvements(optimized, _)
    # Manually apply suggestions
```

**Step 6: Save to GitHub**
```python
# Save as: prompts/customer-emails/follow-up-v2.md
# Commit and push to your repo
# PromptSync will auto-sync on next pull
```

**Step 7: Use it!**
```
# Next time you're in Gmail composing to a customer:
# Press Ctrl+Shift+P
# → PromptSync suggests: "Customer Follow-up v2"
# → One click to insert
# → Variables auto-fill from email context
```

---

## 🐛 Troubleshooting

### Demo won't run
```bash
# Check Python version (need 3.8+)
python --version

# Reinstall dependencies
pip install -r requirements.txt --force-reinstall
```

### API errors
```bash
# Test Claude API connection
python -c "import requests; print(requests.get('https://api.anthropic.com').status_code)"

# Check your API key in config.yaml
# Note: Some features work without API key (use mock data)
```

### Import errors
```bash
# Make sure you're in the right directory
cd promptsync

# Run with module syntax
python -m src.dna.reverse_engineer
```

---

## 📈 Next Steps

1. **Run the demo**: `python demo_dna.py`
2. **Try each feature individually** (see examples above)
3. **Create your first prompt** with good metadata
4. **Set up GitHub sync** (Phase 2)
5. **Configure hotkey** (Phase 2)
6. **Build your prompt library!**

---

## 💡 Tips & Best Practices

### Organizing Prompts

```
prompts/
├── coding/
│   ├── python/
│   │   ├── debug-error.md
│   │   ├── optimize-performance.md
│   │   └── write-tests.md
│   └── javascript/
├── writing/
│   ├── emails/
│   ├── blogs/
│   └── social/
└── business/
    ├── analysis/
    └── presentations/
```

### Variable Naming

Use descriptive, consistent names:
```markdown
Good: {{customer_name}}, {{project_deadline}}, {{bug_description}}
Bad: {{x}}, {{thing}}, {{stuff}}
```

### Testing Prompts

Before committing:
1. Run quality score (aim for 8+)
2. Run security scan (aim for <20 risk)
3. Test with real examples
4. Iterate if needed

### Iteration Workflow

```
Create → Score → Iterate → Scan → Save → Use → Learn → Improve
         ↑___________________________________________↓
```

---

Questions? Check the README or open an issue!
