#!/bin/bash
# Instructions for uploading PromptSync to GitHub

echo "🚀 PromptSync Upload Script"
echo "=============================="
echo ""
echo "Run these commands in order:"
echo ""
echo "# 1. Navigate to this directory"
echo "cd /path/to/promptsync"
echo ""
echo "# 2. Initialize git"
echo "git init"
echo ""
echo "# 3. Add all files"
echo "git add ."
echo ""
echo "# 4. Create initial commit"
cat << 'COMMIT'
git commit -m "Initial commit: PromptSync with DNA Lab

✨ Features:
- Complete DNA Lab (6 intelligence features)
  - Reverse Engineering (extract from images/text)
  - Automated Iteration (self-improving prompts)
  - Security Scanning (vulnerability detection)
  - Quality Scoring (5-dimension evaluation)
  - Encryption (secure sensitive prompts)
  - Prompt Harvesting (web capture)
- Workflow Chaining (composable pipelines)
- A/B Testing (empirical evaluation)

📚 Documentation:
- Comprehensive README with quick start
- Detailed ROADMAP (6 phases)
- FEATURES deep dive
- PROJECT_INSTRUCTIONS for development
- GETTING_STARTED tutorials

🏗️ Architecture:
- Modular: src/dna/, src/workflow/, src/eval/
- Git-native version control
- Safety-first design
- Performance optimized (<200ms response)

🎯 Status:
- Phase 1 (DNA Lab): ✅ Complete
- Phase 2 (Core Platform): 🔄 In Progress
- Production-ready prototypes
- Interactive demo included

Built with ❤️ by developers tired of copy-pasting prompts"
COMMIT
echo ""
echo "# 5. Add remote"
echo "git remote add origin https://github.com/holbizmetrics/PromptSync.git"
echo ""
echo "# 6. Push to GitHub"
echo "git branch -M main"
echo "git push -u origin main"
echo ""
echo "✅ Done! Check: https://github.com/holbizmetrics/PromptSync"
