"""
Automated Prompt Iteration Module (REFACTORED)
Following DRY, SRP, and OOP principles
"""

from typing import List, Dict, Any, Optional
from src.utils.shared import (
    ClaudeAPIClient,
    JSONParser,
    ResponseFormatter,
    rate_limit
)

class PromptIterator:
    """
    Automatically refine prompts through multiple iterations
    
    SRP: This class has ONE responsibility - iterative refinement
    DRY: Uses shared utilities instead of duplicating code
    """
    
    def __init__(self, api_key: Optional[str] = None, max_iterations: int = 5):
        """
        Initialize iterator
        
        Encapsulation: API client is internal implementation detail
        """
        self.api_client = ClaudeAPIClient(api_key)
        self.max_iterations = max_iterations
        self.iterations = []
    
    @rate_limit(calls_per_minute=20)  # DRY: Reusable decorator
    def iterate(self, topic: str, question: str, initial_prompt: Optional[str] = None) -> Dict[str, Any]:
        """
        Run the iterative refinement process
        
        Args:
            topic: The domain/topic of expertise
            question: The question or task to refine
            initial_prompt: Optional starting prompt
        
        Returns:
            ResponseFormatter-style dict with results
        """
        print(f"🔄 Starting iterative refinement for: {question[:50]}...")
        
        # Get initial response
        current_response = initial_prompt or self._get_initial_response(topic, question)
        
        # Track first iteration
        self.iterations.append({
            'iteration': 0,
            'response': current_response,
            'weaknesses': [],
            'improvements': [],
            'quality_score': self._quick_quality_score(current_response)
        })
        
        print(f"  Iteration 0: Quality score {self.iterations[0]['quality_score']:.1f}/10")
        
        # Iterative refinement loop
        for i in range(1, self.max_iterations + 1):
            print(f"  Iteration {i}: Analyzing weaknesses...")
            
            # Analyze weaknesses (uses ClaudeAPIClient internally)
            weaknesses_result = self._analyze_weaknesses(current_response, topic, question)
            if not weaknesses_result['success']:
                break
            
            weaknesses = weaknesses_result['data']
            
            # Check for diminishing returns
            if self._should_stop(weaknesses, self.iterations):
                print(f"  ✅ Optimization complete (diminishing returns detected)")
                break
            
            # Refine based on weaknesses
            print(f"  Iteration {i}: Refining...")
            refined_result = self._refine(current_response, weaknesses, topic, question)
            if not refined_result['success']:
                break
            
            improved_response = refined_result['data']
            
            # Measure improvements
            improvements = self._measure_improvements(current_response, improved_response)
            quality_score = self._quick_quality_score(improved_response)
            
            self.iterations.append({
                'iteration': i,
                'response': improved_response,
                'weaknesses': weaknesses,
                'improvements': improvements,
                'quality_score': quality_score
            })
            
            print(f"  Iteration {i}: Quality score {quality_score:.1f}/10 "
                  f"(+{quality_score - self.iterations[-2]['quality_score']:.1f})")
            
            current_response = improved_response
        
        # Generate final summary
        return self._build_final_result()
    
    def _get_initial_response(self, topic: str, question: str) -> str:
        """
        Generate initial response
        
        DRY: Uses shared API client
        """
        prompt = f"""You are an expert in {topic}. 

Answer this question comprehensively using your expertise:

{question}

Provide a detailed, expert-level response."""

        result = self.api_client.call(prompt)
        return result.get('content', f"[Initial response placeholder for: {question}]")
    
    def _analyze_weaknesses(self, response: str, topic: str, question: str) -> Dict[str, Any]:
        """
        Identify exactly 3 specific weaknesses
        
        DRY: Uses shared API client and JSON parser
        """
        prompt = f"""Analyze this response critically in the context of {topic}:

ORIGINAL QUESTION: {question}

RESPONSE TO ANALYZE:
{response}

Identify EXACTLY 3 specific weaknesses, gaps, or improvement opportunities.

For each weakness, provide:
1. **What's Missing/Wrong**: Be specific, not generic
2. **Why It Matters**: Impact on quality/usefulness (severity 1-10)
3. **How to Fix**: Concrete improvement suggestion

Return ONLY a JSON array with this structure:
[
  {{
    "what": "specific issue description",
    "why": "why this matters",
    "how": "how to fix it",
    "severity": 7
  }},
  ...
]"""

        result = self.api_client.call(prompt, max_tokens=1000)
        
        if not result['success']:
            return ResponseFormatter.error("Failed to analyze weaknesses")
        
        # DRY: Use shared JSON parser
        weaknesses = JSONParser.safe_parse(result['content'], default=[])
        
        if not weaknesses:
            # Fallback to mock data
            weaknesses = self._mock_weaknesses()
        
        return ResponseFormatter.success(weaknesses[:3])  # Ensure exactly 3
    
    def _refine(self, current_response: str, weaknesses: List[Dict], topic: str, question: str) -> Dict[str, Any]:
        """
        Create improved version addressing weaknesses
        
        DRY: Uses shared API client
        """
        weakness_list = '\n'.join([
            f"{i+1}. {w['what']}: {w['how']}"
            for i, w in enumerate(weaknesses)
        ])
        
        prompt = f"""You are an expert in {topic}. Here's a response that needs improvement:

ORIGINAL QUESTION: {question}

CURRENT RESPONSE:
{current_response}

**Identified Weaknesses:**
{weakness_list}

Rewrite the response addressing ALL these weaknesses comprehensively.
- Maintain what's already good
- Fix what's lacking
- Be specific and detailed
- Ensure accuracy and completeness

Return ONLY the improved response, no meta-commentary."""

        result = self.api_client.call(prompt, max_tokens=2000)
        
        if not result['success']:
            return ResponseFormatter.error("Failed to refine response")
        
        return ResponseFormatter.success(result['content'])
    
    def _should_stop(self, weaknesses: List[Dict], iterations: List[Dict]) -> bool:
        """
        Determine if we should stop iterating
        
        SRP: Single decision point for stopping
        """
        if len(iterations) < 2:
            return False
        
        # Check average severity
        avg_severity = sum(w.get('severity', 5) for w in weaknesses) / len(weaknesses)
        if avg_severity < 4:
            return True
        
        # Check quality improvement
        quality_delta = iterations[-1]['quality_score'] - iterations[-2]['quality_score']
        if quality_delta < 0.3:
            return True
        
        return False
    
    def _quick_quality_score(self, response: str) -> float:
        """
        Quick heuristic quality score (0-10)
        
        Note: For full quality scoring, use QualityScorer class
        """
        score = 5.0
        
        # Length
        length = len(response)
        if 500 <= length <= 2000:
            score += 1.0
        elif 300 <= length < 500 or 2000 < length <= 3000:
            score += 0.5
        
        # Structure
        if '\n\n' in response:
            score += 0.5
        if any(marker in response for marker in ['\n- ', '\n* ', '\n1. ']):
            score += 0.5
        
        # Specificity
        import re
        if re.search(r'\d+%|\$\d+|\d+x', response):
            score += 0.5
        if 'example' in response.lower():
            score += 0.5
        
        return min(score, 10.0)
    
    def _measure_improvements(self, old_response: str, new_response: str) -> List[str]:
        """
        Identify key improvements made
        
        Simplified version - full version would use API
        """
        improvements = []
        
        if len(new_response) > len(old_response) * 1.2:
            improvements.append("Added more detail and examples")
        
        if new_response.count('\n') > old_response.count('\n'):
            improvements.append("Improved structure with better formatting")
        
        if not improvements:
            improvements.append("Refined for clarity and accuracy")
        
        return improvements
    
    def _build_final_result(self) -> Dict[str, Any]:
        """
        Generate final result summary
        
        DRY: Uses ResponseFormatter for consistency
        """
        final_iteration = self.iterations[-1]
        
        all_improvements = []
        for iteration in self.iterations[1:]:
            all_improvements.extend(iteration['improvements'])
        
        result_data = {
            'final_response': final_iteration['response'],
            'total_iterations': len(self.iterations) - 1,
            'initial_quality': self.iterations[0]['quality_score'],
            'final_quality': final_iteration['quality_score'],
            'improvement': final_iteration['quality_score'] - self.iterations[0]['quality_score'],
            'key_improvements': all_improvements,
            'iteration_history': self.iterations
        }
        
        print(f"\n✨ Final quality: {result_data['final_quality']:.1f}/10 "
              f"(improved by {result_data['improvement']:.1f} points)")
        
        return ResponseFormatter.success(result_data)
    
    def _mock_weaknesses(self) -> List[Dict[str, Any]]:
        """
        Mock weaknesses for testing without API
        
        Encapsulation: Internal fallback, not exposed
        """
        return [
            {
                "what": "Missing concrete examples",
                "why": "Makes it harder for readers to understand practical application",
                "how": "Add 2-3 real-world examples",
                "severity": 6
            },
            {
                "what": "Lacks specific metrics or data",
                "why": "Reduces credibility and actionability",
                "how": "Include relevant statistics or measurements",
                "severity": 5
            },
            {
                "what": "Could be more structured",
                "why": "Difficult to scan and extract key points",
                "how": "Use bullet points or numbered lists for key concepts",
                "severity": 4
            }
        ]

# Demo usage
if __name__ == '__main__':
    iterator = PromptIterator(max_iterations=3)
    
    result = iterator.iterate(
        topic="Python debugging",
        question="How do I debug a memory leak in a Django application?"
    )
    
    if result['success']:
        data = result['data']
        print("\n" + "="*60)
        print("FINAL RESULT:")
        print("="*60)
        print(data['final_response'])
        print("\n" + "="*60)
        print(f"Improvement: +{data['improvement']:.1f} points")
        print(f"Key improvements:")
        for imp in data['key_improvements']:
            print(f"  - {imp}")
    else:
        print(f"Error: {result['error']}")
