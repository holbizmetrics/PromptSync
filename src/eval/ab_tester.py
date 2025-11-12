"""
A/B Testing & Performance Analytics
Compare prompt variants with empirical evaluation
"""

import time
import statistics
from typing import Dict, List, Any, Optional, Tuple
from datetime import datetime
import json

class PromptVariant:
    """Single prompt variant for testing"""
    
    def __init__(self, variant_id: str, prompt: str, metadata: Dict = None):
        self.id = variant_id
        self.prompt = prompt
        self.metadata = metadata or {}
        self.metrics = {
            'runs': 0,
            'total_cost': 0.0,
            'total_duration': 0.0,
            'scores': [],
            'errors': 0
        }
    
    def record_run(self, cost: float, duration: float, score: float, error: bool = False):
        """Record a test run"""
        self.metrics['runs'] += 1
        self.metrics['total_cost'] += cost
        self.metrics['total_duration'] += duration
        if not error:
            self.metrics['scores'].append(score)
        else:
            self.metrics['errors'] += 1
    
    def get_stats(self) -> Dict[str, Any]:
        """Calculate statistics"""
        scores = self.metrics['scores']
        
        return {
            'id': self.id,
            'runs': self.metrics['runs'],
            'errors': self.metrics['errors'],
            'avg_cost': self.metrics['total_cost'] / max(self.metrics['runs'], 1),
            'avg_duration': self.metrics['total_duration'] / max(self.metrics['runs'], 1),
            'avg_score': statistics.mean(scores) if scores else 0.0,
            'score_std': statistics.stdev(scores) if len(scores) > 1 else 0.0,
            'success_rate': (self.metrics['runs'] - self.metrics['errors']) / max(self.metrics['runs'], 1) * 100
        }

class ABTester:
    """A/B test prompt variants"""
    
    def __init__(self, api_key: Optional[str] = None):
        self.api_key = api_key
        self.variants = {}
        self.test_results = []
    
    def add_variant(self, variant_id: str, prompt: str, metadata: Dict = None):
        """Add a variant to test"""
        self.variants[variant_id] = PromptVariant(variant_id, prompt, metadata)
    
    def run_test(self, 
                 test_inputs: List[Dict[str, Any]], 
                 evaluator: callable,
                 runs_per_variant: int = 5) -> Dict[str, Any]:
        """
        Run A/B test on all variants
        
        Args:
            test_inputs: List of test cases with inputs
            evaluator: Function that scores LLM output (0-100)
            runs_per_variant: Number of runs per variant
        
        Returns:
            Test results with winner
        """
        
        print(f"🧪 Starting A/B Test")
        print(f"   Variants: {len(self.variants)}")
        print(f"   Test cases: {len(test_inputs)}")
        print(f"   Runs per variant: {runs_per_variant}\n")
        
        for variant_id, variant in self.variants.items():
            print(f"Testing variant: {variant_id}")
            
            for i, test_input in enumerate(test_inputs[:runs_per_variant]):
                print(f"  Run {i+1}/{runs_per_variant}...", end='')
                
                try:
                    # Execute prompt
                    start_time = time.time()
                    output = self._execute_prompt(variant.prompt, test_input)
                    duration = time.time() - start_time
                    
                    # Evaluate output
                    score = evaluator(test_input, output)
                    
                    # Calculate cost (simplified)
                    cost = self._estimate_cost(variant.prompt, output)
                    
                    # Record
                    variant.record_run(cost, duration, score, error=False)
                    
                    print(f" ✅ Score: {score:.1f}")
                    
                except Exception as e:
                    print(f" ❌ Error: {e}")
                    variant.record_run(0, 0, 0, error=True)
        
        # Analyze results
        results = self._analyze_results()
        
        return results
    
    def _execute_prompt(self, prompt: str, test_input: Dict) -> str:
        """Execute prompt with LLM (mock for now)"""
        
        if not self.api_key:
            # Mock execution
            return f"Mock output for: {prompt[:50]}..."
        
        # Real LLM call would go here
        # response = call_claude_api(prompt, test_input)
        # return response
        
        return "Mock LLM response"
    
    def _estimate_cost(self, prompt: str, output: str) -> float:
        """Estimate cost based on tokens (simplified)"""
        
        # Rough estimation: $0.003 per 1K input tokens, $0.015 per 1K output
        input_tokens = len(prompt.split()) * 1.3  # Words to tokens approximation
        output_tokens = len(output.split()) * 1.3
        
        input_cost = (input_tokens / 1000) * 0.003
        output_cost = (output_tokens / 1000) * 0.015
        
        return input_cost + output_cost
    
    def _analyze_results(self) -> Dict[str, Any]:
        """Analyze test results and determine winner"""
        
        stats = {vid: v.get_stats() for vid, v in self.variants.items()}
        
        # Determine winner (highest avg score)
        winner_id = max(stats.keys(), key=lambda k: stats[k]['avg_score'])
        winner_stats = stats[winner_id]
        
        # Calculate improvements
        baseline_id = [k for k in stats.keys() if k != winner_id][0] if len(stats) > 1 else winner_id
        baseline_stats = stats[baseline_id]
        
        score_improvement = winner_stats['avg_score'] - baseline_stats['avg_score']
        cost_change = winner_stats['avg_cost'] - baseline_stats['avg_cost']
        
        print("\n" + "="*60)
        print("🏆 A/B Test Results")
        print("="*60)
        
        for vid, vstats in stats.items():
            symbol = "🥇" if vid == winner_id else "  "
            print(f"\n{symbol} Variant: {vid}")
            print(f"   Avg Score: {vstats['avg_score']:.1f}/100")
            print(f"   Avg Cost: ${vstats['avg_cost']:.4f}")
            print(f"   Avg Duration: {vstats['avg_duration']:.2f}s")
            print(f"   Success Rate: {vstats['success_rate']:.1f}%")
            print(f"   Runs: {vstats['runs']} (Errors: {vstats['errors']})")
        
        print(f"\n📊 Improvements (Winner vs Baseline):")
        print(f"   Score: {score_improvement:+.1f} points ({(score_improvement/max(baseline_stats['avg_score'], 1))*100:+.1f}%)")
        print(f"   Cost: ${cost_change:+.4f}")
        print("="*60)
        
        return {
            'winner': winner_id,
            'winner_stats': winner_stats,
            'baseline_stats': baseline_stats,
            'improvement': {
                'score': score_improvement,
                'score_pct': (score_improvement/max(baseline_stats['avg_score'], 1))*100,
                'cost': cost_change
            },
            'all_stats': stats
        }
    
    def generate_report(self, results: Dict) -> str:
        """Generate markdown report"""
        
        report = f"""# A/B Test Report

**Date:** {datetime.now().strftime('%Y-%m-%d %H:%M')}
**Variants Tested:** {len(self.variants)}
**Winner:** {results['winner']}

## Summary

### Winner Performance
- **Average Score:** {results['winner_stats']['avg_score']:.1f}/100
- **Average Cost:** ${results['winner_stats']['avg_cost']:.4f}
- **Average Duration:** {results['winner_stats']['avg_duration']:.2f}s
- **Success Rate:** {results['winner_stats']['success_rate']:.1f}%

### Improvement vs Baseline
- **Score:** {results['improvement']['score']:+.1f} points ({results['improvement']['score_pct']:+.1f}%)
- **Cost:** ${results['improvement']['cost']:+.4f}

## Detailed Results

"""
        
        for vid, stats in results['all_stats'].items():
            report += f"""### Variant: {vid}
- Runs: {stats['runs']}
- Errors: {stats['errors']}
- Avg Score: {stats['avg_score']:.2f} ± {stats['score_std']:.2f}
- Avg Cost: ${stats['avg_cost']:.4f}
- Avg Duration: {stats['avg_duration']:.2f}s
- Success Rate: {stats['success_rate']:.1f}%

"""
        
        report += f"""## Recommendations

"""
        
        if results['improvement']['score_pct'] > 20:
            report += f"✅ **Deploy Winner:** {results['winner']} shows {results['improvement']['score_pct']:.0f}% improvement.\n"
        elif results['improvement']['score_pct'] > 0:
            report += f"⚠️ **Consider Winner:** Marginal improvement of {results['improvement']['score_pct']:.1f}%.\n"
        else:
            report += f"❌ **Keep Baseline:** No significant improvement detected.\n"
        
        if results['improvement']['cost'] > 0.001:
            report += f"💰 **Cost Alert:** Winner is ${results['improvement']['cost']:.4f} more expensive per run.\n"
        
        return report

class PerformanceAnalytics:
    """Track prompt performance over time"""
    
    def __init__(self):
        self.usage_log = []
        self.drift_alerts = []
    
    def log_usage(self, prompt_id: str, context: str, score: float, duration: float, cost: float):
        """Log a prompt usage"""
        
        self.usage_log.append({
            'timestamp': datetime.now().isoformat(),
            'prompt_id': prompt_id,
            'context': context,
            'score': score,
            'duration': duration,
            'cost': cost
        })
    
    def detect_drift(self, prompt_id: str, window: int = 10) -> Optional[Dict]:
        """Detect performance drift"""
        
        # Get recent runs
        recent = [log for log in self.usage_log if log['prompt_id'] == prompt_id][-window*2:]
        
        if len(recent) < window * 2:
            return None
        
        # Split into old and new windows
        old_window = recent[:window]
        new_window = recent[window:]
        
        old_avg = statistics.mean([r['score'] for r in old_window])
        new_avg = statistics.mean([r['score'] for r in new_window])
        
        drift_pct = ((new_avg - old_avg) / old_avg) * 100
        
        # Alert if >5% degradation
        if drift_pct < -5:
            alert = {
                'prompt_id': prompt_id,
                'old_avg': old_avg,
                'new_avg': new_avg,
                'drift_pct': drift_pct,
                'severity': 'HIGH' if drift_pct < -15 else 'MEDIUM'
            }
            self.drift_alerts.append(alert)
            return alert
        
        return None
    
    def get_usage_heatmap(self, prompt_id: str) -> Dict[str, int]:
        """Get usage by context"""
        
        logs = [log for log in self.usage_log if log['prompt_id'] == prompt_id]
        
        heatmap = {}
        for log in logs:
            context = log['context']
            heatmap[context] = heatmap.get(context, 0) + 1
        
        return dict(sorted(heatmap.items(), key=lambda x: x[1], reverse=True))

# Demo usage
if __name__ == '__main__':
    print("📈 A/B Testing Demo\n")
    
    # Create tester
    tester = ABTester()
    
    # Add variants
    tester.add_variant('baseline', 
                      "Analyze this pricing: {{input}}",
                      {'version': '1.0'})
    
    tester.add_variant('improved',
                      """Analyze this SaaS pricing structure:

Input: {{input}}

Provide:
1. Tier breakdown
2. Feature comparison
3. Value analysis
4. Competitive positioning

Format as table.""",
                      {'version': '2.0', 'iterated': True})
    
    # Test inputs
    test_cases = [
        {'input': 'Starter $29/mo, Pro $99/mo, Enterprise custom'},
        {'input': 'Basic free, Premium $15/mo, Business $50/mo'},
        {'input': 'Solo $10/mo, Team $25/user, Enterprise $50/user'}
    ]
    
    # Simple evaluator
    def evaluator(input_data, output):
        # Mock scoring based on output length and structure
        score = min(len(output) / 10, 100)
        if 'table' in output.lower():
            score += 10
        return min(score, 100)
    
    # Run test
    results = tester.run_test(test_cases, evaluator, runs_per_variant=3)
    
    # Generate report
    report = tester.generate_report(results)
    print("\n" + report)
    
    # Demo drift detection
    print("\n📉 Drift Detection Demo")
    print("-"*60)
    
    analytics = PerformanceAnalytics()
    
    # Simulate usage with degradation
    for i in range(20):
        score = 85 - (i * 0.5)  # Gradual degradation
        analytics.log_usage('test-prompt', 'vscode', score, 1.0, 0.02)
    
    drift = analytics.detect_drift('test-prompt', window=10)
    if drift:
        print(f"⚠️ Drift Alert!")
        print(f"   Old average: {drift['old_avg']:.1f}")
        print(f"   New average: {drift['new_avg']:.1f}")
        print(f"   Drift: {drift['drift_pct']:.1f}%")
        print(f"   Severity: {drift['severity']}")
