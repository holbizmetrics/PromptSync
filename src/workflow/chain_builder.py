"""
Prompt Chaining & Workflow Builder
Compose prompts into automated pipelines with variable passing
"""

import yaml
import json
from typing import Dict, Any, List, Optional, Callable
from datetime import datetime
import uuid

class WorkflowStep:
    """Single step in a workflow chain"""
    
    def __init__(self, step_id: str, step_type: str, config: Dict[str, Any]):
        self.id = step_id
        self.type = step_type
        self.config = config
        self.output = None
    
    def execute(self, context: Dict[str, Any]) -> Dict[str, Any]:
        """Execute this step with given context"""
        raise NotImplementedError("Subclasses must implement execute()")
    
    def to_dict(self) -> Dict[str, Any]:
        return {
            'id': self.id,
            'type': self.type,
            'config': self.config
        }

class HarvestStep(WorkflowStep):
    """Harvest content from web"""
    
    def execute(self, context: Dict[str, Any]) -> Dict[str, Any]:
        from src.dna.harvester import PromptHarvester
        
        url = self._resolve_variable(self.config.get('url'), context)
        
        harvester = PromptHarvester()
        result = harvester.extract_from_web({'url': url})
        
        return {
            'success': result['success'],
            'content': result.get('content', ''),
            'metadata': result.get('metadata', {})
        }

class IterateStep(WorkflowStep):
    """Run DNA iteration on input"""
    
    def execute(self, context: Dict[str, Any]) -> Dict[str, Any]:
        from src.dna.iterator import PromptIterator
        
        input_text = self._resolve_variable(self.config.get('input'), context)
        max_iters = self.config.get('max_iters', 3)
        
        iterator = PromptIterator(max_iterations=max_iters)
        result = iterator.iterate(
            topic=self.config.get('topic', 'general'),
            question=input_text,
            initial_prompt=input_text
        )
        
        return {
            'final_response': result['final_response'],
            'quality': result['final_quality'],
            'iterations': result['total_iterations']
        }

class QualityScoreStep(WorkflowStep):
    """Score prompt quality"""
    
    def execute(self, context: Dict[str, Any]) -> Dict[str, Any]:
        from src.dna.quality_score import QualityScorer
        
        input_text = self._resolve_variable(self.config.get('input'), context)
        
        scorer = QualityScorer()
        total, breakdown = scorer.score(input_text)
        
        return {
            'total_score': total,
            'breakdown': breakdown,
            'passed': total >= self.config.get('min_score', 7.0)
        }

class SecurityScanStep(WorkflowStep):
    """Security vulnerability scan"""
    
    def execute(self, context: Dict[str, Any]) -> Dict[str, Any]:
        from src.dna.security_check import SecurityChecker
        
        input_text = self._resolve_variable(self.config.get('input'), context)
        
        checker = SecurityChecker()
        result = checker.scan(input_text)
        
        return {
            'safe': result['safe'],
            'risk_score': result['risk_score'],
            'risk_level': result['risk_level'],
            'issues_count': len(result['issues'])
        }

class ConditionalStep(WorkflowStep):
    """Branch based on condition"""
    
    def execute(self, context: Dict[str, Any]) -> Dict[str, Any]:
        condition = self.config.get('condition')
        
        # Evaluate condition
        result = self._evaluate_condition(condition, context)
        
        return {
            'condition_met': result,
            'branch': self.config.get('if_true' if result else 'if_false')
        }
    
    def _evaluate_condition(self, condition: str, context: Dict) -> bool:
        """Safely evaluate condition"""
        # Simple condition parser
        # Example: "{{harvest.confidence}} > 80"
        
        # Extract variable and operator
        import re
        match = re.match(r'{{(.+?)}}\\s*([><=!]+)\\s*(.+)', condition)
        if not match:
            return False
        
        var_path, operator, value = match.groups()
        
        # Resolve variable
        var_value = self._resolve_variable(f"{{{{{var_path}}}}}", context)
        value = float(value) if value.replace('.', '').isdigit() else value.strip('"\'')
        
        # Evaluate
        ops = {
            '>': lambda a, b: a > b,
            '<': lambda a, b: a < b,
            '>=': lambda a, b: a >= b,
            '<=': lambda a, b: a <= b,
            '==': lambda a, b: a == b,
            '!=': lambda a, b: a != b
        }
        
        return ops.get(operator, lambda a, b: False)(var_value, value)

class ChainBuilder:
    """Build and manage workflow chains"""
    
    STEP_TYPES = {
        'harvest_web': HarvestStep,
        'dna_iterate': IterateStep,
        'quality_score': QualityScoreStep,
        'security_scan': SecurityScanStep,
        'conditional': ConditionalStep
    }
    
    def __init__(self):
        self.steps = []
        self.name = None
        self.description = None
    
    def add_step(self, step_type: str, config: Dict[str, Any], step_id: Optional[str] = None) -> str:
        """Add a step to the chain"""
        
        if step_type not in self.STEP_TYPES:
            raise ValueError(f"Unknown step type: {step_type}")
        
        step_id = step_id or f"{step_type}_{len(self.steps)}"
        step_class = self.STEP_TYPES[step_type]
        step = step_class(step_id, step_type, config)
        
        self.steps.append(step)
        return step_id
    
    def build(self, name: str, description: str = "") -> 'Workflow':
        """Build the workflow"""
        self.name = name
        self.description = description
        
        return Workflow(
            workflow_id=str(uuid.uuid4()),
            name=name,
            description=description,
            steps=self.steps
        )
    
    def from_yaml(self, yaml_path: str) -> 'Workflow':
        """Load workflow from YAML"""
        with open(yaml_path, 'r') as f:
            data = yaml.safe_load(f)
        
        self.name = data['name']
        self.description = data.get('description', '')
        
        for step_data in data['steps']:
            self.add_step(
                step_type=step_data['type'],
                config=step_data['config'],
                step_id=step_data.get('id')
            )
        
        return self.build(self.name, self.description)

class Workflow:
    """Executable workflow chain"""
    
    def __init__(self, workflow_id: str, name: str, description: str, steps: List[WorkflowStep]):
        self.id = workflow_id
        self.name = name
        self.description = description
        self.steps = steps
        self.context = {}
        self.execution_log = []
    
    def run(self, inputs: Dict[str, Any]) -> Dict[str, Any]:
        """Execute the workflow"""
        
        print(f"🔗 Running workflow: {self.name}")
        print(f"   Steps: {len(self.steps)}")
        
        self.context = {'inputs': inputs}
        start_time = datetime.now()
        
        for i, step in enumerate(self.steps):
            print(f"\n   Step {i+1}/{len(self.steps)}: {step.type} ({step.id})")
            
            try:
                # Execute step
                step_start = datetime.now()
                output = step.execute(self.context)
                step_duration = (datetime.now() - step_start).total_seconds()
                
                # Store output in context
                self.context[step.id] = output
                
                # Log execution
                log_entry = {
                    'step_id': step.id,
                    'step_type': step.type,
                    'status': 'success',
                    'duration': step_duration,
                    'output': output
                }
                self.execution_log.append(log_entry)
                
                print(f"      ✅ Success ({step_duration:.2f}s)")
                
                # Check for halt conditions
                if step.type == 'security_scan' and not output.get('safe'):
                    print(f"      🛑 Security halt: Risk level {output['risk_level']}")
                    break
                
                if step.type == 'conditional':
                    if not output['condition_met']:
                        print(f"      ↪️  Branching to: {output['branch']}")
                        # Handle branching logic here
                
            except Exception as e:
                print(f"      ❌ Error: {e}")
                log_entry = {
                    'step_id': step.id,
                    'step_type': step.type,
                    'status': 'error',
                    'error': str(e)
                }
                self.execution_log.append(log_entry)
                break
        
        total_duration = (datetime.now() - start_time).total_seconds()
        
        print(f"\n✨ Workflow complete ({total_duration:.2f}s)")
        
        return {
            'workflow_id': self.id,
            'name': self.name,
            'status': 'completed',
            'duration': total_duration,
            'steps_executed': len(self.execution_log),
            'context': self.context,
            'log': self.execution_log
        }
    
    def export(self) -> str:
        """Export workflow as YAML"""
        
        data = {
            'name': self.name,
            'description': self.description,
            'version': '1.0',
            'created_at': datetime.now().isoformat(),
            'steps': [step.to_dict() for step in self.steps]
        }
        
        return yaml.dump(data, default_flow_style=False)
    
    def visualize(self) -> str:
        """Generate ASCII visualization"""
        
        viz = f"\n📊 Workflow: {self.name}\n"
        viz += "="*60 + "\n\n"
        
        for i, step in enumerate(self.steps):
            viz += f"  {i+1}. [{step.type}] {step.id}\n"
            if i < len(self.steps) - 1:
                viz += "      ↓\n"
        
        viz += "\n" + "="*60
        
        return viz

# Helper mixin for variable resolution
class VariableResolver:
    """Mixin for resolving {{variable}} references"""
    
    def _resolve_variable(self, value: Any, context: Dict[str, Any]) -> Any:
        """Resolve {{variable}} references in value"""
        
        if not isinstance(value, str):
            return value
        
        # Pattern: {{step_id.output_key}}
        import re
        pattern = r'{{(.*?)}}'
        
        def replacer(match):
            var_path = match.group(1).strip()
            parts = var_path.split('.')
            
            result = context
            for part in parts:
                result = result.get(part, {})
            
            return str(result) if result else match.group(0)
        
        return re.sub(pattern, replacer, value)

# Add mixin to step classes
for step_class in [HarvestStep, IterateStep, QualityScoreStep, SecurityScanStep, ConditionalStep]:
    step_class.__bases__ += (VariableResolver,)

# Demo usage
if __name__ == '__main__':
    print("🔗 Workflow Builder Demo\n")
    
    # Example 1: Simple chain
    print("Example 1: Content Pipeline")
    print("-"*60)
    
    builder = ChainBuilder()
    
    harvest = builder.add_step('harvest_web', {
        'url': '{{inputs.url}}'
    })
    
    iterate = builder.add_step('dna_iterate', {
        'input': f'{{{{{harvest}.content}}}}',
        'topic': 'marketing',
        'max_iters': 2
    })
    
    score = builder.add_step('quality_score', {
        'input': f'{{{{{iterate}.final_response}}}}',
        'min_score': 7.0
    })
    
    workflow = builder.build(
        name="content-pipeline",
        description="Harvest → Iterate → Score"
    )
    
    print(workflow.visualize())
    
    # Export to YAML
    yaml_export = workflow.export()
    print("\nYAML Export:")
    print(yaml_export)
    
    # Run workflow
    result = workflow.run({
        'url': 'https://example.com'
    })
    
    print(f"\nFinal result:")
    print(f"  Quality score: {result['context'].get(score, {}).get('total_score', 'N/A')}/10")
    print(f"  Total duration: {result['duration']:.2f}s")
    
    # Example 2: Conditional chain
    print("\n\nExample 2: Security-Gated Chain")
    print("-"*60)
    
    builder2 = ChainBuilder()
    
    input_step = builder2.add_step('harvest_web', {'url': '{{inputs.url}}'})
    security = builder2.add_step('security_scan', {'input': f'{{{{{input_step}.content}}}}'})
    
    conditional = builder2.add_step('conditional', {
        'condition': f'{{{{{security}.risk_score}}}} < 20',
        'if_true': 'proceed',
        'if_false': 'halt'
    })
    
    workflow2 = builder2.build(
        name="security-gated",
        description="Only proceed if safe"
    )
    
    print(workflow2.visualize())
