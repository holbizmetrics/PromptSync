"""
Quality Score Module
Evaluate prompt quality across multiple dimensions
"""

import re
from typing import Dict, Tuple, List

class QualityScorer:
    """Score prompt quality and suggest improvements"""
    
    def __init__(self):
        self.dimensions = [
            'clarity',
            'specificity',
            'structure',
            'context',
            'examples'
        ]
    
    def score(self, prompt: str) -> Tuple[float, Dict[str, float]]:
        """
        Calculate comprehensive quality score
        
        Args:
            prompt: The prompt text to evaluate
        
        Returns:
            Tuple of (total_score, dimension_breakdown)
        """
        scores = {
            'clarity': self._score_clarity(prompt),
            'specificity': self._score_specificity(prompt),
            'structure': self._score_structure(prompt),
            'context': self._score_context(prompt),
            'examples': self._score_examples(prompt)
        }
        
        total = sum(scores.values()) / len(scores)
        return total, scores
    
    def _score_clarity(self, prompt: str) -> float:
        """Score clarity and unambiguity (0-10)"""
        score = 10.0
        
        # Penalize vague words
        vague_words = [
            'good', 'nice', 'some', 'maybe', 'probably', 'kind of',
            'sort of', 'things', 'stuff', 'something', 'anything'
        ]
        
        vague_count = sum(1 for word in vague_words if word in prompt.lower())
        score -= min(vague_count * 0.5, 3.0)
        
        # Penalize questions without context
        if '?' in prompt and len(prompt) < 50:
            score -= 2.0
        
        # Reward specific language
        specific_indicators = [
            'exactly', 'specifically', 'must', 'should', 'required',
            'format:', 'style:', 'tone:', 'audience:'
        ]
        specific_count = sum(1 for ind in specific_indicators if ind in prompt.lower())
        score += min(specific_count * 0.5, 2.0)
        
        return max(0, min(score, 10))
    
    def _score_specificity(self, prompt: str) -> float:
        """Score level of detail and constraints (0-10)"""
        score = 3.0  # Base score
        
        # Count constraints
        constraints = [
            r'\d+\s*words?',           # word count
            r'\d+\s*paragraphs?',       # paragraph count
            r'format:',                 # format specification
            r'audience:',               # audience specification
            r'tone:',                   # tone specification
            r'style:',                  # style specification
            r'include:',                # inclusion requirements
            r'exclude:',                # exclusion requirements
            r'must\s+\w+',              # must requirements
            r'should\s+\w+',            # should requirements
            r'\d+[-–]\d+',              # ranges
            r'between\s+\d+\s+and\s+\d+' # between statements
        ]
        
        constraint_count = sum(
            1 for pattern in constraints 
            if re.search(pattern, prompt, re.IGNORECASE)
        )
        
        score += min(constraint_count * 1.5, 7.0)
        
        return min(score, 10)
    
    def _score_structure(self, prompt: str) -> float:
        """Score organization and formatting (0-10)"""
        score = 5.0  # Base score
        
        # Reward sections/paragraphs
        if '\n\n' in prompt:
            score += 1.5
        
        # Reward lists
        has_bullets = bool(re.search(r'\n\s*[-*•]', prompt))
        has_numbers = bool(re.search(r'\n\s*\d+[\.)]\s', prompt))
        
        if has_bullets:
            score += 1.5
        if has_numbers:
            score += 1.5
        
        # Reward headers
        if re.search(r'^#+\s+\w+', prompt, re.MULTILINE):
            score += 1.0
        if re.search(r'\*\*\w+\*\*|__\w+__', prompt):
            score += 0.5
        
        # Penalize wall of text
        if len(prompt) > 200 and '\n' not in prompt:
            score -= 2.0
        
        return max(0, min(score, 10))
    
    def _score_context(self, prompt: str) -> float:
        """Score contextual information and background (0-10)"""
        score = 5.0  # Base score
        
        # Length as proxy for context
        length = len(prompt)
        if length < 50:
            score = 2.0
        elif length < 100:
            score = 4.0
        elif length < 200:
            score = 6.0
        elif length < 500:
            score = 8.0
        else:
            score = 9.0
        
        # Reward context-providing phrases
        context_indicators = [
            'background:', 'context:', 'goal:', 'purpose:', 'objective:',
            'audience:', 'target:', 'use case:', 'scenario:', 'situation:'
        ]
        
        context_count = sum(
            1 for ind in context_indicators 
            if ind in prompt.lower()
        )
        
        score += min(context_count * 0.5, 1.0)
        
        return min(score, 10)
    
    def _score_examples(self, prompt: str) -> float:
        """Score inclusion of examples and demonstrations (0-10)"""
        score = 3.0  # Base score
        
        # Look for example indicators
        example_indicators = [
            'example:', 'for example', 'e.g.', 'such as', 'like:',
            'for instance', 'demonstration:', 'sample:'
        ]
        
        has_examples = any(
            ind in prompt.lower() 
            for ind in example_indicators
        )
        
        if has_examples:
            score += 4.0
        
        # Look for actual examples (quoted text, code blocks)
        if '```' in prompt:
            score += 2.0
        if re.search(r'["\'].*?["\']', prompt):
            score += 1.0
        
        return min(score, 10)
    
    def suggest_improvements(self, prompt: str, scores: Dict[str, float]) -> List[str]:
        """Generate specific improvement suggestions"""
        suggestions = []
        
        # Clarity improvements
        if scores['clarity'] < 7:
            suggestions.append(
                "Improve clarity: Replace vague words (good, nice, some) with specific descriptors"
            )
            suggestions.append(
                "Add explicit constraints: Specify format, length, style, or tone"
            )
        
        # Specificity improvements
        if scores['specificity'] < 7:
            suggestions.append(
                "Add specificity: Include word count, target audience, or format requirements"
            )
            suggestions.append(
                "Define constraints: Use 'must include', 'should avoid', or 'between X and Y'"
            )
        
        # Structure improvements
        if scores['structure'] < 7:
            suggestions.append(
                "Improve structure: Break into sections with headers or numbered lists"
            )
            suggestions.append(
                "Use formatting: Add bullet points for requirements or steps"
            )
        
        # Context improvements
        if scores['context'] < 7:
            suggestions.append(
                "Add context: Include background, goal, audience, or use case"
            )
            suggestions.append(
                "Expand prompt: Provide more detail about desired outcome"
            )
        
        # Example improvements
        if scores['examples'] < 7:
            suggestions.append(
                "Include examples: Add 'for example' with concrete instances"
            )
            suggestions.append(
                "Show don't tell: Provide sample output or demonstrations"
            )
        
        return suggestions[:5]  # Return top 5
    
    def compare(self, prompt1: str, prompt2: str) -> Dict[str, any]:
        """Compare two prompts and determine winner"""
        
        score1, breakdown1 = self.score(prompt1)
        score2, breakdown2 = self.score(prompt2)
        
        winner = 'prompt1' if score1 > score2 else 'prompt2' if score2 > score1 else 'tie'
        improvement = abs(score2 - score1)
        
        # Find which dimensions improved
        improved_dimensions = []
        declined_dimensions = []
        
        for dim in self.dimensions:
            delta = breakdown2[dim] - breakdown1[dim]
            if delta > 0.5:
                improved_dimensions.append((dim, delta))
            elif delta < -0.5:
                declined_dimensions.append((dim, delta))
        
        return {
            'winner': winner,
            'prompt1_score': score1,
            'prompt2_score': score2,
            'improvement_percentage': (improvement / max(score1, score2)) * 100 if max(score1, score2) > 0 else 0,
            'improved_dimensions': improved_dimensions,
            'declined_dimensions': declined_dimensions,
            'breakdown1': breakdown1,
            'breakdown2': breakdown2
        }
    
    def generate_report(self, prompt: str) -> str:
        """Generate a detailed quality report"""
        
        total, breakdown = self.score(prompt)
        suggestions = self.suggest_improvements(prompt, breakdown)
        
        report = f"""
╔═══════════════════════════════════════════════════════════════╗
║                     PROMPT QUALITY REPORT                     ║
╚═══════════════════════════════════════════════════════════════╝

Overall Score: {total:.1f}/10 {'⭐' * int(total / 2)}

Dimension Breakdown:
"""
        
        for dim, score in breakdown.items():
            bar = '█' * int(score) + '░' * (10 - int(score))
            status = '✅' if score >= 7 else '⚠️' if score >= 5 else '❌'
            report += f"  {status} {dim.capitalize():15s}: {bar} {score:.1f}/10\n"
        
        if suggestions:
            report += f"\n💡 Improvement Suggestions:\n"
            for i, suggestion in enumerate(suggestions, 1):
                report += f"   {i}. {suggestion}\n"
        
        # Grade
        if total >= 9:
            grade = "A (Excellent)"
        elif total >= 8:
            grade = "B (Good)"
        elif total >= 7:
            grade = "C (Acceptable)"
        elif total >= 6:
            grade = "D (Needs Work)"
        else:
            grade = "F (Significant Improvement Needed)"
        
        report += f"\nOverall Grade: {grade}\n"
        
        return report

# Demo
if __name__ == '__main__':
    scorer = QualityScorer()
    
    # Test prompts
    prompts = {
        "Low Quality": "Write something about AI",
        
        "Medium Quality": "Write a 500-word blog post about AI trends in healthcare",
        
        "High Quality": """Write a 500-word blog post about AI trends in healthcare 
for C-suite executives.

Structure:
1. Hook with recent breakthrough
2. Three key trends with supporting data
3. Actionable takeaways

Tone: Professional but accessible
Format: Markdown with headers
Include: Real-world examples from top hospitals

Example opening: "When Mayo Clinic reduced diagnostic errors by 23% using 
AI-powered imaging analysis, it signaled a fundamental shift..."
"""
    }
    
    for label, prompt in prompts.items():
        print("\n" + "="*70)
        print(f"{label} Prompt")
        print("="*70)
        report = scorer.generate_report(prompt)
        print(report)
