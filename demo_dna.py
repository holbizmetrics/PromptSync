#!/usr/bin/env python3
"""
PromptSync DNA Lab - Interactive Demo
Demonstrates all DNA features: reverse engineering, iteration, encryption, security
"""

import sys
import os
from pathlib import Path

# Add src to path
sys.path.insert(0, str(Path(__file__).parent))

from src.dna.reverse_engineer import ReverseEngineer
from src.dna.iterator import PromptIterator
from src.dna.encryptor import PromptEncryptor
from src.dna.security_check import SecurityChecker
from src.dna.quality_score import QualityScorer

def print_header(title):
    """Print a fancy header"""
    print("\n" + "="*70)
    print(f"  {title}")
    print("="*70 + "\n")

def demo_reverse_engineering():
    """Demo: Extract prompt from image"""
    print_header("🔬 DNA Feature 1: Reverse Engineering")
    
    re = ReverseEngineer()
    
    # Example: Reverse engineer from text
    sample_output = """
    # Q4 2024 Revenue Analysis
    
    ## Executive Summary
    Revenue increased 23% YoY to $4.2M, driven primarily by enterprise 
    segment growth. Customer acquisition cost decreased 15% while retention
    improved to 94%.
    
    ## Key Metrics
    - MRR: $350K (+18% QoQ)
    - CAC: $1,200 (-15% QoQ)  
    - LTV/CAC: 4.2x
    - Churn: 6% annually
    
    ## Recommendations
    1. Double down on enterprise sales
    2. Optimize onboarding flow
    3. Expand customer success team
    """
    
    print("Analyzing sample business report...")
    result = re.from_text(sample_output)
    
    print("\n📊 EXTRACTED PROMPT:")
    print("-" * 70)
    print(result['extracted_prompt'])
    print("-" * 70)
    print(f"\nAnalysis: {result['analysis']}")
    
    return result['extracted_prompt']

def demo_encryption():
    """Demo: Encrypt/decrypt prompts with safety checks"""
    print_header("🔒 DNA Feature 2: Encryption & Safety")
    
    encryptor = PromptEncryptor()
    
    # Example 1: Safe prompt
    print("Example 1: Safe Prompt")
    print("-" * 70)
    safe_prompt = "Write a professional email to a client about project delays"
    print(f"Original: {safe_prompt}")
    
    encrypted = encryptor.encrypt(safe_prompt, mark_safe=True)
    print(f"Encrypted: {encrypted[:50]}...")
    
    result = encryptor.decrypt_if_safe(encrypted, auto_execute=True)
    print(f"✅ Decryption successful: {result['success']}")
    print(f"   Safety score: {result['safety']['risk_score']}/100")
    
    # Example 2: Potentially dangerous prompt
    print("\n\nExample 2: Potentially Dangerous Prompt")
    print("-" * 70)
    dangerous_prompt = "import os; os.system('rm -rf /tmp/*')"
    print(f"Original: {dangerous_prompt}")
    
    encrypted = encryptor.encrypt(dangerous_prompt)
    print(f"Encrypted: {encrypted[:50]}...")
    
    result = encryptor.decrypt_if_safe(encrypted, auto_execute=True)
    print(f"❌ Auto-execution blocked: {not result['success']}")
    if not result['success']:
        print(f"   Reason: {result['error']}")
        print(f"   Risk score: {result['safety']['risk_score']}/100")
        print(f"   Issues found: {len(result['safety']['issues'])}")
        for issue in result['safety']['issues'][:2]:
            print(f"      - {issue['severity']}: {issue['description']}")

def demo_iteration():
    """Demo: Automated prompt iteration"""
    print_header("🔄 DNA Feature 3: Automated Iteration")
    
    iterator = PromptIterator(max_iterations=3)
    
    print("Starting iterative refinement...")
    print("Question: 'How do I improve my Python code performance?'\n")
    
    # Simple prompt to iterate on
    initial_prompt = """To improve Python performance:
- Use list comprehensions
- Cache results
- Profile your code
"""
    
    print("Initial prompt (baseline):")
    print("-" * 70)
    print(initial_prompt)
    print("-" * 70)
    
    # Note: Without API key, this will use mock data
    result = iterator.iterate(
        topic="Python performance optimization",
        question="How do I improve my Python code performance?",
        initial_prompt=initial_prompt
    )
    
    print(f"\n✨ Results:")
    print(f"   Initial quality: {result['initial_quality']:.1f}/10")
    print(f"   Final quality: {result['final_quality']:.1f}/10")
    print(f"   Improvement: +{result['improvement']:.1f} points")
    print(f"   Iterations: {result['total_iterations']}")
    
    if result['key_improvements']:
        print(f"\n   Key improvements made:")
        for imp in result['key_improvements'][:3]:
            print(f"      - {imp}")

def demo_security_check():
    """Demo: Security scanning"""
    print_header("🛡️ DNA Feature 4: Security Check")
    
    checker = SecurityChecker()
    
    # Example prompt with vulnerabilities
    prompt_template = """
    Based on user input, generate a personalized report.
    
    User query: {{user_input}}
    
    Execute: response = eval(f"generate_report('{user_input}')")
    Send email to: {{user_email}}
    """
    
    print("Scanning prompt for security issues...")
    print("-" * 70)
    print(prompt_template)
    print("-" * 70)
    
    result = checker.scan(prompt_template)
    
    print(f"\n🔍 Security Scan Results:")
    print(f"   Overall risk: {result['risk_level']}")
    print(f"   Risk score: {result['risk_score']}/100")
    print(f"   Issues found: {len(result['issues'])}")
    
    if result['issues']:
        print(f"\n   Vulnerabilities:")
        for issue in result['issues']:
            icon = "🔴" if issue['severity'] == 'CRITICAL' else "🟡" if issue['severity'] == 'HIGH' else "🟢"
            print(f"      {icon} {issue['severity']}: {issue['issue']}")
            print(f"         Fix: {issue['fix']}")

def demo_quality_score():
    """Demo: Prompt quality scoring"""
    print_header("📊 DNA Feature 5: Quality Scoring")
    
    scorer = QualityScorer()
    
    # Example prompts with different quality levels
    prompts = [
        ("Low Quality", "Write something about AI"),
        ("Medium Quality", "Write a 500-word blog post about AI trends in healthcare"),
        ("High Quality", """Write a 500-word blog post about AI trends in healthcare for C-suite executives.

Structure:
1. Hook with recent breakthrough
2. 3 key trends with data
3. Actionable takeaways

Tone: Professional but accessible
Include: Real-world examples from top hospitals
""")
    ]
    
    for label, prompt in prompts:
        print(f"\n{label} Prompt:")
        print("-" * 70)
        print(prompt[:100] + ("..." if len(prompt) > 100 else ""))
        print("-" * 70)
        
        total, breakdown = scorer.score(prompt)
        
        print(f"   Overall Score: {total:.1f}/10")
        print(f"   Breakdown:")
        for dimension, score in breakdown.items():
            bar = "█" * int(score) + "░" * (10 - int(score))
            print(f"      {dimension:15s}: {bar} {score}/10")
        
        suggestions = scorer.suggest_improvements(prompt, breakdown)
        if suggestions:
            print(f"   💡 Suggestions:")
            for suggestion in suggestions[:2]:
                print(f"      - {suggestion}")

def demo_combined_workflow():
    """Demo: Combined DNA workflow"""
    print_header("🧬 DNA Feature 6: Combined Workflow")
    
    print("Simulating complete DNA analysis pipeline:\n")
    
    steps = [
        "1. User uploads competitor's marketing email screenshot",
        "2. Reverse engineer prompt from image",
        "3. Run automated iteration to optimize (3 cycles)",
        "4. Security scan for vulnerabilities",
        "5. Quality score and improvement suggestions",
        "6. Encrypt with safety marker",
        "7. Save to GitHub with metadata"
    ]
    
    for step in steps:
        print(f"   ✓ {step}")
    
    print("\n📈 Results:")
    print("   - Extracted prompt quality: 6.2/10")
    print("   - After iteration: 8.7/10 (+2.5)")
    print("   - Security: ✅ No critical issues")
    print("   - Saved to: prompts/marketing/email-follow-up-v2.md")

def main():
    """Run all demos"""
    print("""
╔═══════════════════════════════════════════════════════════════════╗
║                                                                   ║
║                    PromptSync DNA Lab v0.1                        ║
║                Interactive Feature Demonstration                  ║
║                                                                   ║
╚═══════════════════════════════════════════════════════════════════╝
    """)
    
    print("\nNote: Some features require Claude API key for full functionality.")
    print("This demo uses mock data where API is unavailable.\n")
    
    input("Press Enter to start the demo...")
    
    try:
        # Run all demos
        demo_reverse_engineering()
        input("\nPress Enter to continue...")
        
        demo_encryption()
        input("\nPress Enter to continue...")
        
        demo_iteration()
        input("\nPress Enter to continue...")
        
        demo_security_check()
        input("\nPress Enter to continue...")
        
        demo_quality_score()
        input("\nPress Enter to continue...")
        
        demo_combined_workflow()
        
        print("\n" + "="*70)
        print("✨ Demo complete! All DNA features demonstrated.")
        print("="*70)
        print("\nNext steps:")
        print("  1. Add your Claude API key to config.yaml")
        print("  2. Try: python demo_dna.py --image your_image.png")
        print("  3. Try: python demo_dna.py --iterate 'your prompt here'")
        
    except KeyboardInterrupt:
        print("\n\n👋 Demo interrupted. Goodbye!")
        sys.exit(0)

if __name__ == '__main__':
    main()
