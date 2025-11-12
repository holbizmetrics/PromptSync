# How to Push PromptSync to GitHub

## Quick Start (5 Minutes)

### 1. Download the Files
All PromptSync files are ready in this directory.

### 2. Create GitHub Repository
Go to: https://github.com/new
- Name: `promptsync`
- Description: "The Git of AI Prompts - with intelligence built in"
- Public or Private (your choice)
- DON'T initialize with README (we have one)
- Click "Create repository"

### 3. Push to GitHub

```bash
# Navigate to your local promptsync directory
cd /path/to/promptsync

# Initialize git
git init

# Add all files
git add .

# First commit
git commit -m "Initial commit: PromptSync with DNA Lab

- Complete DNA Lab features (reverse eng, iteration, security, quality, harvesting)
- Workflow chaining
- A/B testing
- Comprehensive documentation
- Project instructions"

# Connect to your GitHub repo
git remote add origin https://github.com/YOUR_USERNAME/promptsync.git

# Push to GitHub
git branch -M main
git push -u origin main
```

### 4. Verify
Visit: https://github.com/YOUR_USERNAME/promptsync

You should see:
- ✅ All 19 files
- ✅ Beautiful README with badges
- ✅ Complete documentation
- ✅ Working code

### 5. Next Steps

```bash
# Make changes
# ... edit files ...

# Commit
git add .
git commit -m "Add hotkey system"

# Push
git push
```

## Troubleshooting

### Authentication Error
```bash
# Use Personal Access Token
git remote set-url origin https://YOUR_TOKEN@github.com/YOUR_USERNAME/promptsync.git
git push

# Or use SSH
git remote set-url origin git@github.com:YOUR_USERNAME/promptsync.git
git push
```

### Token Creation
1. Go to: https://github.com/settings/tokens
2. Click "Generate new token (classic)"
3. Scopes: Select `repo`
4. Expiration: 30 days (or more)
5. Click "Generate token"
6. Copy the token (you won't see it again!)

## Continuous Updates

When I create updates:
1. I'll tell you which files changed
2. You pull my updates
3. Review changes
4. Commit & push

```bash
# Get updates from Claude
# ... download new files ...

# Review what changed
git status
git diff

# Commit if good
git add .
git commit -m "Update from Claude: Added XYZ feature"
git push
```

## GitHub Repo Settings

After pushing, configure:

### Branch Protection (Recommended)
Settings → Branches → Add rule
- Branch name: `main`
- ✅ Require pull request reviews
- ✅ Require status checks to pass

### About Section
Add to repo description:
- Website: (your demo URL)
- Topics: `prompt-management`, `ai`, `llm`, `git`, `workflow-automation`

### Social Preview
Settings → General → Social preview
- Upload: Create a nice banner image
- Or use: https://socialify.git.ci/YOUR_USERNAME/promptsync/image

## Making it Public

If you want community:
Settings → General → Danger Zone → Change visibility → Public

Then:
1. Share on Twitter
2. Post on Reddit (r/MachineLearning)
3. Launch on Product Hunt
4. Add to Awesome Lists

---

**Ready to push?** Run the commands above! 🚀
