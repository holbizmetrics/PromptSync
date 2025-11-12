"""
Automated Prompt Iteration Module
Iteratively refine prompts through analysis and improvement cycles
"""

import json
import time
from typing import List, Dict, Any, Optional
import requests

class PromptIterator:
    """Automatically refine prompts through multiple iterations"""
    
    def __init__(self, api_key: Optional[str] = None, max_iterations: int = 5):
        self.api_key = api_key
        self.max_iterations = max_iterations
        self.api_url = "https://api.anthropic.com/v1/messages"
    
    def iterate(self, topic: str, question: str, initial_prompt: Optional[str] = None) -> Dict[str, Any]:
        """
        Run the iterative refinement process
        
        Args:
            topic: The domain/topic of expertise
            question: The question or task to refine
            initial_prompt: Optional starting prompt (if None, generates one)
        
        Returns:
            Dict with final result and iteration history
        """
        print(f"🔄 Starting iterative refinement for: {question[:50]}...")
        
        iterations = []
        
        # Get initial response
        if initial_prompt:
            current_response = initial_prompt
        else:
            current_response = self._get_initial_response(topic, question)
        
        iterations.append({
            'iteration': 0,
            'response': current_response,
            'weaknesses': [],
            'improvements': [],
            'quality_score': self._quick_quality_score(current_response)
        })
        
        print(f"  Iteration 0: Quality score {iterations[0]['quality_score']:.1f}/10")
        
        # Iterative refinement loop
        for i in range(1, self.max_iterations + 1):
            print(f"  Iteration {i}: Analyzing weaknesses...")
            
            # Analyze weaknesses
            weaknesses = self._analyze_weaknesses(current_response, topic, question)
            
            # Check for diminishing returns
            if self._should_stop(weaknesses, iterations):
                print(f"  ✅ Optimization complete (diminishing returns detected)")
                break
            
            # Refine based on weaknesses
            print(f"  Iteration {i}: Refining...")
            improved_response = self._refine(current_response, weaknesses, topic, question)
            
            # Measure improvements
            improvements = self._measure_improvements(current_response, improved_response)
            quality_score = self._quick_quality_score(improved_response)
            
            iterations.append({
                'iteration': i,
                'response': improved_response,
                'weaknesses': weaknesses,
                'improvements': improvements,
                'quality_score': quality_score
            })
            
            print(f"  Iteration {i}: Quality score {quality_score:.1f}/10 (+{quality_score - iterations[-2]['quality_score']:.1f})")
            
            current_response = improved_response
        
        # Generate final summary
        final_iteration = iterations[-1]
        all_improvements = []
        for iteration in iterations[1:]:
            all_improvements.extend(iteration['improvements'])
        
        result = {
            'final_response': final_iteration['response'],
            'total_iterations': len(iterations) - 1,
            'initial_quality': iterations[0]['quality_score'],
            'final_quality': final_iteration['quality_score'],
            'improvement': final_iteration['quality_score'] - iterations[0]['quality_score'],
            'key_improvements': all_improvements,
            'iteration_history': iterations
        }
        
        print(f"\n✨ Final quality: {result['final_quality']:.1f}/10 (improved by {result['improvement']:.1f} points)")
        
        return result
    
    def _get_initial_response(self, topic: str, question: str) -> str:
        """Generate initial response"""
        
        if not self.api_key:
            return f"[Initial response placeholder for: {question}]"
        
        prompt = f"""You are an expert in {topic}. 

Answer this question comprehensively using your expertise:

{question}

Provide a detailed, expert-level response."""

        return self._call_claude(prompt)
    
    def _analyze_weaknesses(self, response: str, topic: str, question: str) -> List[Dict[str, Any]]:
        """Identify exactly 3 specific weaknesses"""
        
        if not self.api_key:
            return self._mock_weaknesses()
        
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

        result = self._call_claude(prompt)
        
        try:
            # Extract JSON from response
            if '```json' in result:
                result = result.split('```json')[1].split('```')[0]
            elif '```' in result:
                result = result.split('```')[1].split('```')[0]
            
            weaknesses = json.loads(result.strip())
            return weaknesses[:3]  # Ensure exactly 3
        except:
            return self._mock_weaknesses()
    
    def _should_stop(self, weaknesses: List[Dict], iterations: List[Dict]) -> bool:
        """Determine if we should stop iterating"""
        
        # If less than 2 iterations, continue
        if len(iterations) < 2:
            return False
        
        # Check average severity
        avg_severity = sum(w.get('severity', 5) for w in weaknesses) / len(weaknesses)
        
        # Stop if weaknesses are minor (average severity < 4)
        if avg_severity < 4:
            return True
        
        # Stop if quality improvement is minimal
        if len(iterations) >= 2:
            quality_delta = iterations[-1]['quality_score'] - iterations[-2]['quality_score']
            if quality_delta < 0.3:
                return True
        
        return False
    
    def _refine(self, current_response: str, weaknesses: List[Dict], topic: str, question: str) -> str:
        """Create improved version addressing weaknesses"""
        
        if not self.api_key:
            return current_response + "\n[Improved version]"
        
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

        return self._call_claude(prompt)
    
    def _measure_improvements(self, old_response: str, new_response: str) -> List[str]:
        """Identify key improvements made"""
        
        if not self.api_key:
            return ["Improved clarity", "Added examples", "Fixed inaccuracies"]
        
        prompt = f"""Compare these two versions and list the key improvements in Version 2:

**Version 1:**
{old_response[:500]}...

**Version 2:**
{new_response[:500]}...

List 3-5 specific, measurable improvements. Be concise.

Return as JSON array of strings: ["improvement 1", "improvement 2", ...]"""

        result = self._call_claude(prompt)
        
        try:
            if '```json' in result:
                result = result.split('```json')[1].split('```')[0]
            elif '```' in result:
                result = result.split('```')[1].split('```')[0]
            
            improvements = json.loads(result.strip())
            return improvements
        except:
            return ["Improved version generated"]
    
    def _quick_quality_score(self, response: str) -> float:
        """Quick heuristic quality score (0-10)"""
        score = 5.0  # Base score
        
        # Length (optimal around 500-2000 chars)
        length = len(response)
        if 500 <= length <= 2000:
            score += 1.0
        elif 300 <= length < 500 or 2000 < length <= 3000:
            score += 0.5
        
        # Structure (paragraphs, lists, etc.)
        if '\n\n' in response:
            score += 0.5
        if any(marker in response for marker in ['\n- ', '\n* ', '\n1. ']):
            score += 0.5
        if any(marker in response for marker in ['**', '__', '##']):
            score += 0.3
        
        # Specificity (numbers, examples, etc.)
        import re
        if re.search(r'\d+%|\$\d+|\d+x', response):
            score += 0.5
        if 'example' in response.lower():
            score += 0.5
        if 'specifically' in response.lower() or 'for instance' in response.lower():
            score += 0.3
        
        # Completeness
        if len(response.split('.')) > 5:  # Multiple sentences
            score += 0.5
        
        # Technical depth (for code/technical content)
        if '```' in response or 'def ' in response or 'function' in response:
            score += 0.5
        
        return min(score, 10.0)
    
    def _call_claude(self, prompt: str) -> str:
        """Call Claude API"""
        
        try:
            response = requests.post(
                self.api_url,
                headers={
                    "Content-Type": "application/json",
                    "x-api-key": self.api_key,
                    "anthropic-version": "2023-06-01"
                },
                json={
                    "model": "claude-sonnet-4-20250514",
                    "max_tokens": 2000,
                    "messages": [{
                        "role": "user",
                        "content": prompt
                    }]
                },
                timeout=30
            )
            
            if response.status_code == 200:
                result = response.json()
                return result['content'][0]['text']
            else:
                print(f"⚠️  API error: {response.status_code}")
                return "[API call failed]"
        
        except Exception as e:
            print(f"⚠️  Error calling Claude API: {e}")
            return "[API call failed]"
    
    def _mock_weaknesses(self) -> List[Dict[str, Any]]:
        """Mock weaknesses for testing without API"""
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
    
    print("\n" + "="*60)
    print("FINAL RESULT:")
    print("="*60)
    print(result['final_response'])
    print("\n" + "="*60)
    print(f"Improvement: +{result['improvement']:.1f} points")
    print(f"Key improvements:")
    for imp in result['key_improvements']:
        print(f"  - {imp}")
