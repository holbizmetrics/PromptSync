# PromptSync

**The Git of AI Prompts - with intelligence built in.**

Stop copy-pasting prompts. Start managing them like code.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Python 3.8+](https://img.shields.io/badge/python-3.8+-blue.svg)](https://www.python.org/downloads/)
[![Status: Alpha](https://img.shields.io/badge/status-alpha-orange.svg)](https://github.com/YOUR_USERNAME/promptsync)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)

> **Note:** PromptSync is in active development. Phase 1 (DNA Lab) is complete. Phase 2 (Core Platform) is in progress. See [ROADMAP.md](ROADMAP.md) for details.

---

## 📺 Demo

```bash
python demo_dna.py  # See all DNA features in action
```

**Coming soon:** Video walkthrough + Product Hunt launch

---

## 🎯 The Problem

**Prompts are scattered everywhere:**
- 📝 Lost in chat histories
- 🗂️ Buried in notes apps  
- 🧠 Stuck in your memory
- ❌ No version control
- 🔁 Manual copy-paste every time

**Result:** Wasted time, lost knowledge, no improvement.

---

## ✨ The Solution

**PromptSync turns prompts into intelligent, evolving assets:**

```
Press Ctrl+Shift+P anywhere → AI suggests YOUR prompts → One click → Done
```

**3 seconds** from intent to action. Always the right prompt, always up-to-date.

---

## 🚀 What Makes PromptSync Different

| Feature | Traditional Tools | PromptSync |
|---------|------------------|------------|
| **Storage** | Database/Cloud | Git (version control) |
| **Intelligence** | Static templates | DNA Lab (6 features) |
| **Workflow** | Manual steps | Automated chains |
| **Context** | Manual search | Auto-suggests |
| **Evolution** | Never improves | Self-optimizing |

### 1. **Git-Native** 
Your prompts in Git, not a database. Version control you already know.

### 2. **DNA Lab** (6 Intelligence Features)
- 🔬 **Reverse Engineering**: Extract prompts from images/text
- 🔄 **Auto-Iteration**: Self-improving prompts (3-5 cycles)
- 🛡️ **Security Scanning**: Detect vulnerabilities before execution
- 📊 **Quality Scoring**: 5-dimension evaluation
- 🔒 **Encryption**: Secure sensitive prompts
- 🕸️ **Web Harvesting**: Capture prompts while browsing

### 3. **Composable Workflows**
Chain prompts into automated pipelines:
```
Harvest → Reverse Engineer → Iterate → Scan → Deploy
```

### 4. **Context-Aware**
Knows what you're doing. Suggests relevant prompts automatically.

---

## 💡 Quick Example

### Without PromptSync:
1. Open notes app → 5 sec
2. Search for prompt → 10 sec
3. Copy prompt → 2 sec
4. Paste into AI → 2 sec
5. Realize it's outdated → 😤

**Total:** 19+ seconds, every time

### With PromptSync:
1. Press `Ctrl+Shift+P` → instant
2. Type 2 letters → 1 sec
3. Press `Enter` → instant

**Total:** <3 seconds, always current

**Time saved per year:** 40+ hours 💰

---

## 🎬 Quick Start

### Prerequisites
- Python 3.8+
- Git
- GitHub account

### Installation

```bash
# 1. Clone the repo
git clone https://github.com/YOUR_USERNAME/promptsync.git
cd promptsync

# 2. Install dependencies
pip install -r requirements.txt

# 3. Try the DNA Lab demo
python demo_dna.py
```

### Configuration

```bash
# 1. Copy config template
cp config.example.yaml config.yaml

# 2. Edit config (add your GitHub token)
# Get token at: https://github.com/settings/tokens
# Needs 'repo' scope

# 3. Create your prompts repository on GitHub
# Name it: my-prompts (or whatever you prefer)
```

### Your First Prompt

Create `prompts/test-prompt.md` in your GitHub repo:

```markdown
---
title: Debug Python Error
tags: [python, debug, error]
apps: [vscode, pycharm]
patterns: [traceback, exception, error]
---

I'm getting this error:
{{error}}

In this code:
{{code}}

Help me debug it.
```

### Run PromptSync

```bash
python main.py
# Press Ctrl+Shift+P anywhere to access prompts
```

**That's it!** 🎉

---

## 🧬 DNA Lab Features

### 1. Reverse Engineering 📸

**Extract prompts from any output**

```python
from src.dna.reverse_engineer import ReverseEngineer

re = ReverseEngineer()

# From image
result = re.from_image("competitor_design.png")
print(result['extracted_prompt'])
# → "Create hero section with gradient purple/blue..."

# From text
result = re.from_text(business_report)
print(result['extracted_prompt'])
# → "Generate quarterly report with metrics table..."
```

**Use cases:**
- Learn from competitor content
- Replicate successful formats
- Build prompt library from examples

---

### 2. Automated Iteration 🔄

**Self-improving prompts**

```python
from src.dna.iterator import PromptIterator

iterator = PromptIterator()
result = iterator.iterate(
    topic="email writing",
    question="Write a professional follow-up"
)

print(f"Improved from {result['initial_quality']}/10")
print(f"to {result['final_quality']}/10")
# Improved from 6.2/10 to 8.7/10 (+2.5 points)
```

**How it works:**
1. Generate initial response
2. Analyze 3 specific weaknesses
3. Refine to address issues
4. Repeat until quality plateaus
5. Return optimized prompt

---

### 3. Security Scanning 🛡️

**Detect vulnerabilities before execution**

```python
from src.dna.security_check import SecurityChecker

checker = SecurityChecker()
result = checker.scan(prompt_template)

print(f"Risk Level: {result['risk_level']}")
print(f"Risk Score: {result['risk_score']}/100")

# Detects:
# - Prompt injection attacks
# - Code execution risks (eval, exec)
# - PII exposure
# - Unsafe patterns
```

---

### 4. Quality Scoring 📊

**5-dimension evaluation**

```python
from src.dna.quality_score import QualityScorer

scorer = QualityScorer()
total, breakdown = scorer.score(prompt)

print(f"Overall: {total:.1f}/10")
print(f"Clarity: {breakdown['clarity']}/10")
print(f"Specificity: {breakdown['specificity']}/10")
# + Structure, Context, Examples
```

---

### 5. Encryption 🔒

**Secure sensitive prompts**

```python
from src.dna.encryptor import PromptEncryptor

encryptor = PromptEncryptor()

# Encrypt
encrypted = encryptor.encrypt(sensitive_prompt, mark_safe=True)

# Decrypt with safety check
result = encryptor.decrypt_if_safe(encrypted, auto_execute=True)
if result['success']:
    use_prompt(result['prompt'])
```

---

### 6. Prompt Harvesting 🕸️

**Capture prompts from the web**

```python
from src.dna.harvester import PromptHarvester

harvester = PromptHarvester()

# From URL
result = harvester.extract_from_web({
    'url': 'https://competitor.com/pricing'
})

# Generate prompt
prompt = harvester.create_prompt_from_harvest(result)
print(prompt['title'])  # Auto-categorized
print(f"Confidence: {prompt['frontmatter']['confidence']}%")
```

**Browser extension:** Press `Ctrl+Shift+H` to harvest (coming soon)

---

### 7. Workflow Chaining 🔗

**Compose prompts into pipelines**

```python
from src.workflow.chain_builder import ChainBuilder

builder = ChainBuilder()

# Build pipeline
harvest = builder.add_step('harvest_web', {'url': '{{input}}'})
iterate = builder.add_step('dna_iterate', {
    'input': f'{{{{{harvest}.content}}}}'
})
score = builder.add_step('quality_score', {
    'input': f'{{{{{iterate}.output}}}}'
})

workflow = builder.build(name="content-pipeline")

# Execute
result = workflow.run({'input': 'https://example.com'})
# → Automated: Harvest → Improve → Score
```

---

### 8. A/B Testing 📈

**Data-driven prompt evolution**

```python
from src.eval.ab_tester import ABTester

tester = ABTester()

# Add variants
tester.add_variant('baseline', "Analyze: {{input}}")
tester.add_variant('improved', """Analyze {{input}}:
1. Key findings
2. Recommendations
3. Action items""")

# Run test
results = tester.run_test(test_cases, evaluator)

# Winner: improved (+42% accuracy)
```

---

## 🎯 Use Cases

### For Solo Developers
```
✅ Version control for prompts
✅ Reverse engineer from examples
✅ Auto-iterate for quality
✅ Security scan before use
```

### For Teams
```
✅ Shared Git repo = single source of truth
✅ Pull requests for prompt reviews
✅ Branch protection for production
✅ Audit logs via Git history
```

### For Content Creators
```
✅ Harvest inspiration while browsing
✅ Iterate for quality improvement
✅ Reusable templates with variables
✅ Track what works (analytics)
```

### For Enterprises
```
✅ Security scanning mandatory
✅ Compliance via version control
✅ On-premise deployment (roadmap)
✅ SSO/SAML integration (roadmap)
```

---

## 📦 Project Structure

```
promptsync/
├── README.md              # You are here
├── ROADMAP.md             # Development timeline
├── GETTING_STARTED.md     # Detailed tutorials
├── FEATURES.md            # Feature deep dive
├── PROJECT_INSTRUCTIONS.md # Development guidelines
│
├── demo_dna.py            # Interactive demo ← Start here
├── main.py                # Main application
├── config.example.yaml    # Configuration template
├── requirements.txt       # Dependencies
│
├── src/
│   ├── dna/              # DNA Lab features ✅
│   │   ├── reverse_engineer.py
│   │   ├── iterator.py
│   │   ├── security_check.py
│   │   ├── quality_score.py
│   │   ├── encryptor.py
│   │   └── harvester.py
│   ├── workflow/         # Chaining ✅
│   │   └── chain_builder.py
│   ├── eval/             # A/B testing ✅
│   │   └── ab_tester.py
│   └── core/             # (In progress)
│       ├── github_sync.py
│       ├── hotkey.py
│       ├── context.py
│       └── matcher.py
│
└── prompts/              # Your synced repo
```

---

## 🛣️ Roadmap

**Current Phase:** Phase 2 - Core Platform (Weeks 1-8)

### ✅ Phase 1: DNA Lab (Complete)
- [x] Reverse Engineering
- [x] Automated Iteration
- [x] Security Scanning
- [x] Quality Scoring
- [x] Encryption
- [x] Harvesting
- [x] Workflow Chaining
- [x] A/B Testing

### 🔄 Phase 2: Core Platform (In Progress)
- [ ] GitHub Sync (bi-directional)
- [ ] Hotkey System (cross-platform)
- [ ] Context Detection
- [ ] Smart Matching
- [ ] Popup UI

### 📋 Phase 3: Intelligence (Q1 2026)
- [ ] Semantic Search
- [ ] Usage Analytics
- [ ] Learning Patterns
- [ ] Drift Detection

### 📋 Phase 4: Team Collaboration (Q2 2026)
- [ ] Real-time Co-editing
- [ ] Branch Management
- [ ] Access Control
- [ ] Audit Logs

### 📋 Phase 5: Ecosystem (Q3 2026)
- [ ] Browser Extension
- [ ] VS Code Plugin
- [ ] Slack/Teams Integration
- [ ] Mobile Companion

See [ROADMAP.md](ROADMAP.md) for complete details.

---

## 🤝 Contributing

We welcome contributions! Here's how to help:

### Quick Contributions
- ⭐ Star this repo
- 🐛 Report bugs
- 💡 Suggest features
- 📖 Improve docs

### Code Contributions
1. Fork the repo
2. Create a branch (`git checkout -b feature/amazing`)
3. Follow `PROJECT_INSTRUCTIONS.md`
4. Commit changes (`git commit -m 'Add amazing feature'`)
5. Push to branch (`git push origin feature/amazing`)
6. Open a Pull Request

**Code Style:**
- Follow `Clean_Code_And_OOP_Principles.md`
- Follow `Vibe_Coding_Guide_Extended.md`
- Follow `PROJECT_INSTRUCTIONS.md`

---

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| [README.md](README.md) | Overview & quick start (you are here) |
| [ROADMAP.md](ROADMAP.md) | Development timeline & phases |
| [GETTING_STARTED.md](GETTING_STARTED.md) | Detailed tutorials & examples |
| [FEATURES.md](FEATURES.md) | Complete feature documentation |
| [PROJECT_INSTRUCTIONS.md](PROJECT_INSTRUCTIONS.md) | Development guidelines |
| [COMPETITIVE_ANALYSIS.md](COMPETITIVE_ANALYSIS.md) | Market positioning |
| [COMPLETE_VISION.md](COMPLETE_VISION.md) | Long-term vision |

---

## 💬 Community

- **Discussions:** [GitHub Discussions](https://github.com/YOUR_USERNAME/promptsync/discussions)
- **Issues:** [Bug Reports & Feature Requests](https://github.com/YOUR_USERNAME/promptsync/issues)
- **Discord:** Coming in Phase 4
- **Twitter:** [@promptsync](https://twitter.com/promptsync) (coming soon)

---

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

**TL;DR:** Free to use, modify, distribute. Attribution appreciated.

---

## 🙏 Acknowledgments

Built with:
- [PyGithub](https://github.com/PyGithub/PyGithub) - GitHub API
- [Claude API](https://anthropic.com) - AI intelligence
- [pynput](https://github.com/moses-palmer/pynput) - Global hotkeys
- And many other amazing open-source libraries

Special thanks to the AI community for inspiration.

---

## ⭐ Star History

[![Star History Chart](https://api.star-history.com/svg?repos=YOUR_USERNAME/promptsync&type=Date)](https://star-history.com/#YOUR_USERNAME/promptsync&Date)

---

## 🚀 Get Started

```bash
git clone https://github.com/YOUR_USERNAME/promptsync.git
cd promptsync
python demo_dna.py
```

**Questions? Ideas? Issues?**
Open a [GitHub Discussion](https://github.com/YOUR_USERNAME/promptsync/discussions) or [Issue](https://github.com/YOUR_USERNAME/promptsync/issues).

---

**Built with ❤️ by developers tired of copy-pasting prompts**

[⬆ Back to Top](#promptsync)
