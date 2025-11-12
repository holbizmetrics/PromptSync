"""
Prompt Harvesting Module
Extract and create prompts from web sources with ethical safeguards
"""

import requests
from bs4 import BeautifulSoup
from urllib.parse import urlparse, urljoin
from urllib.robotparser import RobotFileParser
import re
from typing import Dict, Any, Optional, List
from datetime import datetime
import hashlib

class PromptHarvester:
    """Harvest prompts from web sources ethically"""
    
    def __init__(self, api_key: Optional[str] = None):
        self.api_key = api_key
        self.user_agent = "PromptSync/0.1 (Ethical Prompt Harvester; +https://promptsync.dev)"
        self.session = requests.Session()
        self.session.headers.update({'User-Agent': self.user_agent})
        
        # Cache for robots.txt checks
        self.robots_cache = {}
    
    def extract_from_web(self, source: Dict[str, Any]) -> Dict[str, Any]:
        """
        Extract content from web source
        
        Args:
            source: Dict with 'url', 'selected' (optional), 'mode'
        
        Returns:
            Dict with extracted content and metadata
        """
        url = source['url']
        
        # Check if scraping is allowed
        if not self._is_scraping_allowed(url):
            return {
                'success': False,
                'error': 'Scraping not allowed by robots.txt',
                'url': url
            }
        
        try:
            # Fetch page
            response = self.session.get(url, timeout=10)
            response.raise_for_status()
            
            # Parse HTML
            soup = BeautifulSoup(response.content, 'html.parser')
            
            # Extract based on selection or full page
            if source.get('selected'):
                content = self._extract_selection(soup, source['selected'])
            else:
                content = self._extract_smart(soup)
            
            # Generate metadata
            metadata = self._generate_metadata(url, soup, content)
            
            return {
                'success': True,
                'url': url,
                'content': content,
                'metadata': metadata,
                'extracted_at': datetime.now().isoformat()
            }
        
        except Exception as e:
            return {
                'success': False,
                'error': str(e),
                'url': url
            }
    
    def extract_from_selection(self, text: str, url: Optional[str] = None) -> Dict[str, Any]:
        """
        Extract content from user-selected text (e.g., browser extension)
        
        Args:
            text: Selected text
            url: Optional source URL
        
        Returns:
            Dict with content and metadata
        """
        return {
            'success': True,
            'content': text,
            'metadata': {
                'source': url or 'user-selection',
                'extraction_method': 'direct-selection',
                'length': len(text),
                'word_count': len(text.split())
            },
            'extracted_at': datetime.now().isoformat()
        }
    
    def create_prompt_from_harvest(self, extracted: Dict, prompt_type: str = 'auto') -> Dict[str, Any]:
        """
        Create a prompt from harvested content
        
        Args:
            extracted: Output from extract_from_web or extract_from_selection
            prompt_type: 'auto', 'competitive', 'inspiration', 'tutorial', etc.
        
        Returns:
            Dict with generated prompt and metadata
        """
        content = extracted['content']
        url = extracted.get('url', 'unknown')
        
        # Determine prompt type if auto
        if prompt_type == 'auto':
            prompt_type = self._detect_prompt_type(content, extracted.get('metadata', {}))
        
        # Generate prompt based on type
        prompt_generators = {
            'competitive': self._generate_competitive_prompt,
            'inspiration': self._generate_inspiration_prompt,
            'tutorial': self._generate_tutorial_prompt,
            'api_docs': self._generate_api_prompt,
            'design': self._generate_design_prompt,
            'general': self._generate_general_prompt
        }
        
        generator = prompt_generators.get(prompt_type, self._generate_general_prompt)
        prompt_data = generator(content, url, extracted.get('metadata', {}))
        
        # Add harvesting metadata
        prompt_data['frontmatter'].update({
            'source_url': url,
            'harvested_at': extracted['extracted_at'],
            'harvest_method': extracted.get('metadata', {}).get('extraction_method', 'web'),
            'confidence': self._calculate_confidence(content, prompt_data)
        })
        
        return prompt_data
    
    def _is_scraping_allowed(self, url: str) -> bool:
        """Check if scraping is allowed via robots.txt"""
        
        parsed = urlparse(url)
        robots_url = f"{parsed.scheme}://{parsed.netloc}/robots.txt"
        
        # Check cache
        if robots_url in self.robots_cache:
            rp = self.robots_cache[robots_url]
        else:
            # Fetch and parse robots.txt
            rp = RobotFileParser()
            rp.set_url(robots_url)
            try:
                rp.read()
                self.robots_cache[robots_url] = rp
            except:
                # If can't fetch robots.txt, assume allowed but warn
                print(f"⚠️  Could not fetch robots.txt from {robots_url}")
                return True
        
        # Check if our user agent can fetch this URL
        return rp.can_fetch(self.user_agent, url)
    
    def _extract_selection(self, soup: BeautifulSoup, selector: str) -> str:
        """Extract specific selection from page"""
        
        # Try different selection methods
        element = None
        
        # CSS selector
        try:
            element = soup.select_one(selector)
        except:
            pass
        
        # ID or class
        if not element:
            element = soup.find(id=selector) or soup.find(class_=selector)
        
        if element:
            return element.get_text(strip=True, separator='\n')
        else:
            return soup.get_text(strip=True, separator='\n')
    
    def _extract_smart(self, soup: BeautifulSoup) -> str:
        """Smart extraction - main content only"""
        
        # Remove noise
        for element in soup(['script', 'style', 'nav', 'footer', 'header', 'aside']):
            element.decompose()
        
        # Try to find main content
        main = soup.find('main') or soup.find('article') or soup.find('div', class_=re.compile('content|main'))
        
        if main:
            return main.get_text(strip=True, separator='\n')
        else:
            return soup.get_text(strip=True, separator='\n')
    
    def _generate_metadata(self, url: str, soup: BeautifulSoup, content: str) -> Dict:
        """Generate metadata about the source"""
        
        title = soup.find('title')
        description = soup.find('meta', attrs={'name': 'description'})
        
        return {
            'title': title.string if title else urlparse(url).netloc,
            'description': description['content'] if description else '',
            'domain': urlparse(url).netloc,
            'content_length': len(content),
            'word_count': len(content.split())
        }
    
    def _detect_prompt_type(self, content: str, metadata: Dict) -> str:
        """Auto-detect what type of prompt to generate"""
        
        content_lower = content.lower()
        
        # Competitive analysis indicators
        if any(word in content_lower for word in ['pricing', 'features', 'plans', 'compare', 'vs']):
            return 'competitive'
        
        # Tutorial indicators
        if any(word in content_lower for word in ['step', 'guide', 'how to', 'tutorial', 'learn']):
            return 'tutorial'
        
        # API docs indicators
        if any(word in content_lower for word in ['api', 'endpoint', 'request', 'response', 'authentication']):
            return 'api_docs'
        
        # Design indicators
        if any(word in content_lower for word in ['design', 'layout', 'ui', 'ux', 'mockup']):
            return 'design'
        
        return 'general'
    
    def _generate_competitive_prompt(self, content: str, url: str, metadata: Dict) -> Dict:
        """Generate competitive analysis prompt"""
        
        return {
            'title': f"Competitive Analysis - {metadata.get('domain', 'Unknown')}",
            'category': 'competitive',
            'prompt': f"""Analyze this competitive intelligence from {metadata.get('domain')}:

{content[:500]}...

Generate:
1. Feature comparison table
2. Pricing tier analysis
3. Unique selling points
4. Gaps and opportunities
5. Strategic recommendations

Compare to: [YOUR_PRODUCT]
Focus on: [SPECIFIC_ASPECTS]
""",
            'frontmatter': {
                'title': f"Competitive Analysis - {metadata.get('domain')}",
                'tags': ['competitive', 'analysis', 'market-research'],
                'category': 'business'
            }
        }
    
    def _generate_inspiration_prompt(self, content: str, url: str, metadata: Dict) -> Dict:
        """Generate inspiration/style-based prompt"""
        
        return {
            'title': f"Style Inspiration - {metadata.get('title', 'Untitled')}",
            'category': 'inspiration',
            'prompt': f"""Using this content as inspiration:

{content[:300]}...

Create similar content that:
- Captures the same tone and style
- Addresses: [YOUR_TOPIC]
- Target audience: [YOUR_AUDIENCE]
- Length: [YOUR_LENGTH]

Maintain the voice while making it original.
""",
            'frontmatter': {
                'title': f"Style Inspiration - {metadata.get('title')}",
                'tags': ['inspiration', 'writing', 'style'],
                'category': 'creative'
            }
        }
    
    def _generate_tutorial_prompt(self, content: str, url: str, metadata: Dict) -> Dict:
        """Generate tutorial-based prompt"""
        
        return {
            'title': f"Tutorial Guide - {metadata.get('title', 'Untitled')}",
            'category': 'tutorial',
            'prompt': f"""Create a tutorial based on this structure:

Reference: {content[:400]}...

Build a step-by-step guide for: [YOUR_TOPIC]

Include:
- Prerequisites
- Step-by-step instructions
- Code examples (if applicable)
- Common pitfalls
- Expected outcomes

Make it beginner-friendly.
""",
            'frontmatter': {
                'title': f"Tutorial Guide - {metadata.get('title')}",
                'tags': ['tutorial', 'how-to', 'guide'],
                'category': 'education'
            }
        }
    
    def _generate_api_prompt(self, content: str, url: str, metadata: Dict) -> Dict:
        """Generate API integration prompt"""
        
        return {
            'title': f"API Integration - {metadata.get('domain', 'Unknown')}",
            'category': 'api_docs',
            'prompt': f"""Based on this API documentation:

{content[:400]}...

Generate:
1. Integration guide for [YOUR_LANGUAGE]
2. Sample requests with authentication
3. Error handling patterns
4. Common use cases
5. Rate limiting considerations

Format: Code snippets + explanations
""",
            'frontmatter': {
                'title': f"API Integration - {metadata.get('domain')}",
                'tags': ['api', 'integration', 'development'],
                'category': 'development'
            }
        }
    
    def _generate_design_prompt(self, content: str, url: str, metadata: Dict) -> Dict:
        """Generate design-based prompt"""
        
        return {
            'title': f"Design Pattern - {metadata.get('title', 'Untitled')}",
            'category': 'design',
            'prompt': f"""Create a design based on this reference:

Source: {url}
Key elements observed: {content[:200]}...

Design specifications:
- Style: [MINIMALIST/BOLD/MODERN/etc]
- Colors: [YOUR_PALETTE]
- Layout: [GRID/FLUID/FIXED]
- Components needed: [LIST]

Include Figma/Sketch mockup description.
""",
            'frontmatter': {
                'title': f"Design Pattern - {metadata.get('title')}",
                'tags': ['design', 'ui', 'mockup'],
                'category': 'design'
            }
        }
    
    def _generate_general_prompt(self, content: str, url: str, metadata: Dict) -> Dict:
        """Generate general-purpose prompt"""
        
        return {
            'title': f"Reference - {metadata.get('title', 'Untitled')}",
            'category': 'general',
            'prompt': f"""Reference content from {metadata.get('domain')}:

{content[:400]}...

Task: [DESCRIBE_YOUR_TASK]
Context: [YOUR_CONTEXT]
Requirements: [YOUR_REQUIREMENTS]
Output format: [DESIRED_FORMAT]
""",
            'frontmatter': {
                'title': f"Reference - {metadata.get('title')}",
                'tags': ['reference', 'general'],
                'category': 'general'
            }
        }
    
    def _calculate_confidence(self, content: str, prompt_data: Dict) -> int:
        """Calculate confidence score for generated prompt"""
        
        score = 50  # Base score
        
        # Content length
        word_count = len(content.split())
        if word_count > 100:
            score += 20
        elif word_count > 50:
            score += 10
        
        # Has structure
        if '\n' in content:
            score += 10
        
        # Prompt has variables
        if '{{' in prompt_data['prompt'] or '[' in prompt_data['prompt']:
            score += 10
        
        # Has category
        if prompt_data.get('category') != 'general':
            score += 10
        
        return min(score, 100)
    
    def format_for_github(self, prompt_data: Dict) -> str:
        """Format prompt data as markdown for GitHub"""
        
        frontmatter = prompt_data['frontmatter']
        
        # Build YAML frontmatter
        yaml_lines = ['---']
        for key, value in frontmatter.items():
            if isinstance(value, list):
                yaml_lines.append(f"{key}: {value}")
            else:
                yaml_lines.append(f"{key}: {value}")
        yaml_lines.append('---')
        
        # Combine with prompt
        markdown = '\n'.join(yaml_lines) + '\n\n' + prompt_data['prompt']
        
        return markdown

# Demo usage
if __name__ == '__main__':
    harvester = PromptHarvester()
    
    # Example 1: Extract from URL
    print("Example 1: Extracting from URL")
    print("="*60)
    
    source = {
        'url': 'https://example.com',  # Replace with actual URL
        'mode': 'smart'
    }
    
    extracted = harvester.extract_from_web(source)
    
    if extracted['success']:
        print(f"✅ Extracted {extracted['metadata']['word_count']} words")
        print(f"Title: {extracted['metadata']['title']}")
        
        # Generate prompt
        prompt = harvester.create_prompt_from_harvest(extracted, 'auto')
        print(f"\n📝 Generated Prompt ({prompt['category']}):")
        print("-"*60)
        print(prompt['prompt'][:300] + "...")
        print(f"\nConfidence: {prompt['frontmatter']['confidence']}%")
    else:
        print(f"❌ Error: {extracted['error']}")
    
    # Example 2: From selection
    print("\n\nExample 2: From user selection")
    print("="*60)
    
    selection = """
    Our pricing starts at $29/month for the starter plan,
    includes 10,000 API calls, 5 team members, and basic support.
    """
    
    extracted = harvester.extract_from_selection(
        selection,
        url="https://competitor.com/pricing"
    )
    
    prompt = harvester.create_prompt_from_harvest(extracted, 'competitive')
    
    markdown = harvester.format_for_github(prompt)
    print("Formatted for GitHub:")
    print("-"*60)
    print(markdown[:400] + "...")
