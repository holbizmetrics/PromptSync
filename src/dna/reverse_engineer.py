"""
Reverse Engineering Module
Extract prompts from outputs (text or images)
"""

import json
import base64
from pathlib import Path
import requests

class ReverseEngineer:
    """Reverse engineer prompts from outputs"""
    
    def __init__(self, api_key=None):
        self.api_key = api_key
        self.api_url = "https://api.anthropic.com/v1/messages"
    
    def from_image(self, image_path):
        """Extract prompt from an image"""
        print(f"📸 Analyzing image: {image_path}")
        
        # Read and encode image
        with open(image_path, 'rb') as f:
            image_data = base64.b64encode(f.read()).decode()
        
        # Determine media type
        suffix = Path(image_path).suffix.lower()
        media_type_map = {
            '.png': 'image/png',
            '.jpg': 'image/jpeg',
            '.jpeg': 'image/jpeg',
            '.gif': 'image/gif',
            '.webp': 'image/webp'
        }
        media_type = media_type_map.get(suffix, 'image/png')
        
        # Call Claude API for analysis
        analysis = self._analyze_image_with_claude(image_data, media_type)
        
        # Construct prompt from analysis
        prompt = self._construct_prompt(analysis)
        
        return {
            'source_image': str(image_path),
            'analysis': analysis,
            'extracted_prompt': prompt,
            'confidence': self._calculate_confidence(analysis)
        }
    
    def from_text(self, output_text):
        """Extract prompt from text output"""
        print(f"📝 Analyzing text output ({len(output_text)} chars)")
        
        # Analyze text characteristics
        analysis = self._analyze_text_output(output_text)
        
        # Construct prompt
        prompt = self._construct_prompt_from_text(analysis)
        
        return {
            'source_text': output_text[:200] + '...',
            'analysis': analysis,
            'extracted_prompt': prompt
        }
    
    def _analyze_image_with_claude(self, image_data, media_type):
        """Use Claude Vision API to analyze image"""
        
        if not self.api_key:
            # Fallback: Basic analysis without API
            return self._basic_image_analysis()
        
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
                    "max_tokens": 1500,
                    "messages": [{
                        "role": "user",
                        "content": [
                            {
                                "type": "image",
                                "source": {
                                    "type": "base64",
                                    "media_type": media_type,
                                    "data": image_data
                                }
                            },
                            {
                                "type": "text",
                                "text": """Analyze this image and extract characteristics for prompt engineering:

Return JSON with:
{
  "type": "chart|infographic|diagram|illustration|photo|design|ui",
  "style": "modern|minimalist|corporate|playful|artistic|technical",
  "color_palette": ["#hex1", "#hex2", ...],
  "mood": "professional|friendly|serious|energetic|calm",
  "layout": "description of composition and structure",
  "content": "what information/elements are present",
  "purpose": "what this is trying to communicate",
  "audience": "who this is for",
  "technical": {
    "format": "suggested format",
    "resolution": "quality level",
    "tools": "likely creation tools"
  }
}"""
                            }
                        ]
                    }]
                }
            )
            
            if response.status_code == 200:
                result = response.json()
                # Extract JSON from response
                text = result['content'][0]['text']
                # Handle markdown code blocks
                if '```json' in text:
                    text = text.split('```json')[1].split('```')[0]
                elif '```' in text:
                    text = text.split('```')[1].split('```')[0]
                return json.loads(text.strip())
            else:
                print(f"⚠️  API error: {response.status_code}")
                return self._basic_image_analysis()
        
        except Exception as e:
            print(f"⚠️  Error calling Claude API: {e}")
            return self._basic_image_analysis()
    
    def _basic_image_analysis(self):
        """Fallback analysis without API"""
        return {
            "type": "unknown",
            "style": "modern",
            "color_palette": ["#000000", "#FFFFFF"],
            "mood": "neutral",
            "layout": "standard composition",
            "content": "visual content",
            "purpose": "communication",
            "audience": "general",
            "technical": {
                "format": "PNG",
                "resolution": "high",
                "tools": "digital design tool"
            }
        }
    
    def _analyze_text_output(self, text):
        """Analyze text output to understand its characteristics"""
        
        analysis = {
            'length': len(text),
            'structure': self._detect_structure(text),
            'tone': self._detect_tone(text),
            'format': self._detect_format(text),
            'domain': self._detect_domain(text),
            'complexity': self._measure_complexity(text)
        }
        
        return analysis
    
    def _detect_structure(self, text):
        """Detect document structure"""
        if '##' in text or '###' in text:
            return 'markdown_headers'
        elif '\n- ' in text or '\n* ' in text:
            return 'bullet_list'
        elif '\n1. ' in text or '\n2. ' in text:
            return 'numbered_list'
        elif '|' in text and '---' in text:
            return 'table'
        else:
            return 'prose'
    
    def _detect_tone(self, text):
        """Detect writing tone"""
        formal_words = ['therefore', 'furthermore', 'consequently', 'moreover']
        casual_words = ['hey', 'cool', 'awesome', 'gonna', 'wanna']
        
        formal_count = sum(1 for word in formal_words if word in text.lower())
        casual_count = sum(1 for word in casual_words if word in text.lower())
        
        if formal_count > casual_count:
            return 'formal'
        elif casual_count > formal_count:
            return 'casual'
        else:
            return 'neutral'
    
    def _detect_format(self, text):
        """Detect content format"""
        if 'def ' in text or 'function' in text or 'class ' in text:
            return 'code'
        elif len(text.split('\n\n')) > 3:
            return 'article'
        elif '@' in text and 'Subject:' in text:
            return 'email'
        else:
            return 'general'
    
    def _detect_domain(self, text):
        """Detect subject domain"""
        domains = {
            'technical': ['code', 'function', 'database', 'API', 'algorithm'],
            'business': ['revenue', 'market', 'customer', 'strategy', 'ROI'],
            'creative': ['story', 'character', 'scene', 'narrative', 'plot'],
            'academic': ['research', 'study', 'hypothesis', 'methodology', 'analysis']
        }
        
        scores = {}
        for domain, keywords in domains.items():
            scores[domain] = sum(1 for kw in keywords if kw.lower() in text.lower())
        
        return max(scores.items(), key=lambda x: x[1])[0] if max(scores.values()) > 0 else 'general'
    
    def _measure_complexity(self, text):
        """Measure text complexity"""
        words = text.split()
        avg_word_length = sum(len(w) for w in words) / len(words) if words else 0
        
        if avg_word_length > 6:
            return 'high'
        elif avg_word_length > 4:
            return 'medium'
        else:
            return 'low'
    
    def _construct_prompt(self, analysis):
        """Build prompt template from image analysis"""
        
        prompt = f"""Create a {analysis['type']} in {analysis['style']} style.

**Visual Specifications:**
- Color palette: {', '.join(analysis.get('color_palette', ['default colors']))}
- Layout: {analysis['layout']}
- Mood: {analysis['mood']}

**Content:**
{analysis['content']}

**Purpose:**
{analysis['purpose']}

**Target Audience:**
{analysis['audience']}

**Format:**
{analysis['technical'].get('format', 'PNG')} at {analysis['technical'].get('resolution', 'high resolution')}
"""
        
        return prompt.strip()
    
    def _construct_prompt_from_text(self, analysis):
        """Build prompt from text analysis"""
        
        prompt = f"""Generate {analysis['format']} content in {analysis['tone']} tone.

**Requirements:**
- Structure: {analysis['structure']}
- Domain: {analysis['domain']}
- Complexity: {analysis['complexity']}
- Target length: ~{analysis['length']} characters

Ensure the output follows the same structural patterns and maintains consistent tone throughout.
"""
        
        return prompt.strip()
    
    def _calculate_confidence(self, analysis):
        """Calculate confidence score for extraction"""
        score = 0
        
        # Check completeness of analysis
        if analysis.get('type') and analysis['type'] != 'unknown':
            score += 20
        if analysis.get('style'):
            score += 15
        if analysis.get('color_palette') and len(analysis['color_palette']) > 1:
            score += 15
        if analysis.get('content'):
            score += 25
        if analysis.get('purpose'):
            score += 25
        
        return min(score, 100)

# Demo usage
if __name__ == '__main__':
    import sys
    
    re = ReverseEngineer()
    
    if len(sys.argv) > 1:
        image_path = sys.argv[1]
        result = re.from_image(image_path)
        
        print("\n" + "="*60)
        print("EXTRACTED PROMPT:")
        print("="*60)
        print(result['extracted_prompt'])
        print("\n" + "="*60)
        print(f"Confidence: {result['confidence']}%")
    else:
        print("Usage: python reverse_engineer.py <image_path>")
