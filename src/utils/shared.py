"""
Shared Utilities
Common functionality extracted to avoid duplication (DRY principle)
"""

import json
import re
import requests
from typing import Dict, Any, Optional
from functools import wraps
import time

# ============================================================================
# API Client Base Class (Single Responsibility + DRY)
# ============================================================================

class ClaudeAPIClient:
    """
    Centralized Claude API client
    
    Single source of truth for all Claude API interactions.
    Follows SRP: This class does ONE thing - API communication.
    """
    
    def __init__(self, api_key: Optional[str] = None):
        self.api_key = api_key
        self.api_url = "https://api.anthropic.com/v1/messages"
        self.default_model = "claude-sonnet-4-20250514"
    
    def call(self, 
             prompt: str, 
             max_tokens: int = 1500,
             temperature: float = 1.0,
             system: Optional[str] = None) -> Dict[str, Any]:
        """
        Make a Claude API call
        
        DRY: All API calls use this single method
        
        Args:
            prompt: The prompt text
            max_tokens: Max response tokens
            temperature: Randomness (0-1)
            system: Optional system prompt
        
        Returns:
            Dict with 'success', 'content', and optional 'error'
        """
        
        if not self.api_key:
            return {
                'success': False,
                'error': 'No API key provided',
                'content': None
            }
        
        try:
            payload = {
                "model": self.default_model,
                "max_tokens": max_tokens,
                "temperature": temperature,
                "messages": [{"role": "user", "content": prompt}]
            }
            
            if system:
                payload["system"] = system
            
            response = requests.post(
                self.api_url,
                headers={
                    "Content-Type": "application/json",
                    "x-api-key": self.api_key,
                    "anthropic-version": "2023-06-01"
                },
                json=payload,
                timeout=30
            )
            
            if response.status_code == 200:
                result = response.json()
                return {
                    'success': True,
                    'content': result['content'][0]['text'],
                    'usage': result.get('usage', {})
                }
            else:
                return {
                    'success': False,
                    'error': f"API error: {response.status_code}",
                    'content': None
                }
        
        except Exception as e:
            return {
                'success': False,
                'error': str(e),
                'content': None
            }

# ============================================================================
# JSON Utilities (DRY for parsing)
# ============================================================================

class JSONParser:
    """
    Centralized JSON parsing utilities
    
    DRY: All JSON extraction uses these methods
    """
    
    @staticmethod
    def extract_from_markdown(text: str) -> str:
        """
        Extract JSON from markdown code blocks
        
        Handles: ```json ... ```, ``` ... ```, or plain JSON
        
        Args:
            text: Text that may contain JSON
        
        Returns:
            Cleaned JSON string
        """
        # Remove markdown code blocks
        if '```json' in text:
            text = text.split('```json')[1].split('```')[0]
        elif '```' in text:
            text = text.split('```')[1].split('```')[0]
        
        return text.strip()
    
    @staticmethod
    def safe_parse(text: str, default: Any = None) -> Any:
        """
        Safely parse JSON with fallback
        
        Args:
            text: JSON string
            default: Return value if parsing fails
        
        Returns:
            Parsed JSON or default
        """
        try:
            cleaned = JSONParser.extract_from_markdown(text)
            return json.loads(cleaned)
        except (json.JSONDecodeError, ValueError, AttributeError):
            return default

# ============================================================================
# Response Formatter (Consistency + DRY)
# ============================================================================

class ResponseFormatter:
    """
    Standardized response format
    
    DRY: All modules return responses in the same format
    Encapsulation: Hide complexity of response structure
    """
    
    @staticmethod
    def success(data: Any, message: str = None) -> Dict[str, Any]:
        """
        Format a success response
        
        Args:
            data: The response data
            message: Optional success message
        
        Returns:
            Standardized success dict
        """
        response = {
            'success': True,
            'data': data
        }
        if message:
            response['message'] = message
        return response
    
    @staticmethod
    def error(error_msg: str, 
              error_type: str = 'general',
              recoverable: bool = True,
              details: Dict = None) -> Dict[str, Any]:
        """
        Format an error response
        
        Args:
            error_msg: Human-readable error message
            error_type: Category of error
            recoverable: Can user retry?
            details: Additional error context
        
        Returns:
            Standardized error dict
        """
        response = {
            'success': False,
            'error': error_msg,
            'error_type': error_type,
            'recoverable': recoverable
        }
        if details:
            response['details'] = details
        return response

# ============================================================================
# Variable Resolver (DRY for template variables)
# ============================================================================

class VariableResolver:
    """
    Resolve {{variable}} references in text
    
    DRY: Used by workflow steps, prompts, and templates
    """
    
    @staticmethod
    def resolve(text: str, context: Dict[str, Any]) -> str:
        """
        Replace {{variable}} with values from context
        
        Supports dot notation: {{step.output.key}}
        
        Args:
            text: Text with {{variables}}
            context: Dict with values
        
        Returns:
            Text with variables replaced
        """
        if not isinstance(text, str):
            return text
        
        pattern = r'{{(.*?)}}'
        
        def replacer(match):
            var_path = match.group(1).strip()
            parts = var_path.split('.')
            
            result = context
            for part in parts:
                if isinstance(result, dict):
                    result = result.get(part, {})
                else:
                    return match.group(0)  # Return original if can't resolve
            
            return str(result) if result else match.group(0)
        
        return re.sub(pattern, replacer, text)
    
    @staticmethod
    def extract_variables(text: str) -> list:
        """
        Extract all {{variable}} names from text
        
        Args:
            text: Text to scan
        
        Returns:
            List of variable names
        """
        pattern = r'{{(.*?)}}'
        return re.findall(pattern, text)

# ============================================================================
# Rate Limiter (Reusable decorator)
# ============================================================================

def rate_limit(calls_per_minute: int = 10):
    """
    Rate limiting decorator
    
    DRY: Apply to any function that needs rate limiting
    
    Args:
        calls_per_minute: Maximum calls allowed per minute
    
    Usage:
        @rate_limit(calls_per_minute=20)
        def api_call():
            ...
    """
    min_interval = 60.0 / calls_per_minute
    last_called = [0.0]
    
    def decorator(func):
        @wraps(func)
        def wrapper(*args, **kwargs):
            elapsed = time.time() - last_called[0]
            if elapsed < min_interval:
                time.sleep(min_interval - elapsed)
            result = func(*args, **kwargs)
            last_called[0] = time.time()
            return result
        return wrapper
    return decorator

# ============================================================================
# File Operations (DRY for file handling)
# ============================================================================

class FileOperations:
    """
    Common file operations
    
    DRY: Centralized file I/O with consistent error handling
    """
    
    @staticmethod
    def read_text(file_path: str, encoding: str = 'utf-8') -> Dict[str, Any]:
        """
        Read text file with error handling
        
        Returns:
            ResponseFormatter-style dict
        """
        try:
            with open(file_path, 'r', encoding=encoding) as f:
                content = f.read()
            return ResponseFormatter.success(content)
        except FileNotFoundError:
            return ResponseFormatter.error(
                f"File not found: {file_path}",
                error_type='file_not_found'
            )
        except Exception as e:
            return ResponseFormatter.error(
                str(e),
                error_type='file_read_error'
            )
    
    @staticmethod
    def write_text(file_path: str, content: str, encoding: str = 'utf-8') -> Dict[str, Any]:
        """
        Write text file with error handling
        
        Returns:
            ResponseFormatter-style dict
        """
        try:
            # Ensure directory exists
            import os
            os.makedirs(os.path.dirname(file_path), exist_ok=True)
            
            with open(file_path, 'w', encoding=encoding) as f:
                f.write(content)
            return ResponseFormatter.success({'path': file_path})
        except Exception as e:
            return ResponseFormatter.error(
                str(e),
                error_type='file_write_error'
            )

# ============================================================================
# Validation Utilities (DRY for common checks)
# ============================================================================

class Validators:
    """
    Common validation functions
    
    DRY: Reusable validation logic
    """
    
    @staticmethod
    def is_url(text: str) -> bool:
        """Check if text is a valid URL"""
        url_pattern = re.compile(
            r'^https?://'  # http:// or https://
            r'(?:(?:[A-Z0-9](?:[A-Z0-9-]{0,61}[A-Z0-9])?\.)+[A-Z]{2,6}\.?|'  # domain
            r'localhost|'  # localhost
            r'\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})'  # IP
            r'(?::\d+)?'  # optional port
            r'(?:/?|[/?]\S+)$', re.IGNORECASE)
        return url_pattern.match(text) is not None
    
    @staticmethod
    def is_github_repo(text: str) -> bool:
        """Check if text is GitHub repo format (username/repo)"""
        return bool(re.match(r'^[\w-]+/[\w-]+$', text))
    
    @staticmethod
    def has_frontmatter(text: str) -> bool:
        """Check if markdown has YAML frontmatter"""
        return text.strip().startswith('---')

# ============================================================================
# Example Usage
# ============================================================================

if __name__ == '__main__':
    print("🔧 Utilities Demo\n")
    
    # 1. API Client
    print("1. Claude API Client")
    print("-" * 60)
    client = ClaudeAPIClient()
    result = client.call("Say hello in 5 words")
    print(f"API call: {result['success']}")
    
    # 2. JSON Parser
    print("\n2. JSON Parser")
    print("-" * 60)
    markdown_json = """Here's the result:
```json
{"score": 85, "status": "good"}
```
"""
    parsed = JSONParser.safe_parse(markdown_json)
    print(f"Parsed: {parsed}")
    
    # 3. Response Formatter
    print("\n3. Response Formatter")
    print("-" * 60)
    success_resp = ResponseFormatter.success({'result': 'done'}, 'Operation completed')
    error_resp = ResponseFormatter.error('Something failed', error_type='validation')
    print(f"Success: {success_resp}")
    print(f"Error: {error_resp}")
    
    # 4. Variable Resolver
    print("\n4. Variable Resolver")
    print("-" * 60)
    template = "Hello {{user.name}}, your score is {{score}}"
    context = {'user': {'name': 'Alice'}, 'score': 95}
    resolved = VariableResolver.resolve(template, context)
    print(f"Resolved: {resolved}")
    
    # 5. Validators
    print("\n5. Validators")
    print("-" * 60)
    print(f"Is URL: {Validators.is_url('https://example.com')}")
    print(f"Is GitHub repo: {Validators.is_github_repo('user/repo')}")
    
    print("\n✅ All utilities working!")
