#!/usr/bin/env python3
"""
PromptSync - Simple GUI
A working prompt manager with actual UI
"""

import tkinter as tk
from tkinter import ttk, messagebox
import os
from pathlib import Path
import pyperclip
from pynput import keyboard
import threading

class PromptSyncUI:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("PromptSync")
        self.root.geometry("700x500")
        
        # Load prompts
        self.prompts = self.load_prompts()
        
        # Build UI
        self.build_ui()
        
        # Setup hotkey
        self.setup_hotkey()
    
    def load_prompts(self):
        """Load prompts from prompts/ folder"""
        prompts = []
        prompts_dir = Path("prompts")
        
        # Create if doesn't exist
        if not prompts_dir.exists():
            prompts_dir.mkdir()
            # Create example prompt
            example = prompts_dir / "example.md"
            example.write_text("""---
title: Example Prompt
tags: [example, test]
---

This is an example prompt.

Try editing files in the prompts/ folder!
""")
        
        # Load all .md files
        for file in prompts_dir.glob("**/*.md"):
            try:
                content = file.read_text(encoding='utf-8')
                
                # Parse frontmatter
                title = file.stem.replace('-', ' ').replace('_', ' ').title()
                tags = []
                prompt_text = content
                
                if content.startswith('---'):
                    parts = content.split('---', 2)
                    if len(parts) >= 3:
                        # Parse YAML frontmatter
                        frontmatter = parts[1]
                        for line in frontmatter.split('\n'):
                            if line.startswith('title:'):
                                title = line.split('title:', 1)[1].strip()
                            elif line.startswith('tags:'):
                                tags_str = line.split('tags:', 1)[1].strip()
                                tags = [t.strip(' []') for t in tags_str.split(',')]
                        
                        prompt_text = parts[2].strip()
                
                prompts.append({
                    'title': title,
                    'tags': tags,
                    'content': prompt_text,
                    'file': str(file)
                })
            except Exception as e:
                print(f"Error loading {file}: {e}")
        
        return prompts
    
    def build_ui(self):
        """Build the user interface"""
        
        # Title
        title_frame = tk.Frame(self.root, bg='#4F46E5', height=60)
        title_frame.pack(fill=tk.X)
        title_frame.pack_propagate(False)
        
        title_label = tk.Label(
            title_frame,
            text="🧬 PromptSync",
            font=('Arial', 20, 'bold'),
            bg='#4F46E5',
            fg='white'
        )
        title_label.pack(pady=15)
        
        # Search frame
        search_frame = tk.Frame(self.root, bg='white', pady=10)
        search_frame.pack(fill=tk.X, padx=10)
        
        tk.Label(
            search_frame,
            text="🔍 Search:",
            font=('Arial', 11),
            bg='white'
        ).pack(side=tk.LEFT, padx=(10, 5))
        
        self.search_var = tk.StringVar()
        self.search_var.trace('w', self.on_search)
        
        search_entry = tk.Entry(
            search_frame,
            textvariable=self.search_var,
            font=('Arial', 11),
            width=40
        )
        search_entry.pack(side=tk.LEFT, fill=tk.X, expand=True, padx=5)
        search_entry.focus()
        
        # Prompts list
        list_frame = tk.Frame(self.root)
        list_frame.pack(fill=tk.BOTH, expand=True, padx=10, pady=5)
        
        # Scrollbar
        scrollbar = tk.Scrollbar(list_frame)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
        
        # Listbox
        self.listbox = tk.Listbox(
            list_frame,
            font=('Arial', 11),
            yscrollcommand=scrollbar.set,
            selectmode=tk.SINGLE,
            height=15
        )
        self.listbox.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        scrollbar.config(command=self.listbox.yview)
        
        # Populate list
        self.refresh_list()
        
        # Bind events
        self.listbox.bind('<Double-Button-1>', self.on_select)
        self.listbox.bind('<Return>', self.on_select)
        
        # Preview frame
        preview_frame = tk.LabelFrame(self.root, text="Preview", font=('Arial', 10, 'bold'))
        preview_frame.pack(fill=tk.BOTH, expand=True, padx=10, pady=5)
        
        self.preview_text = tk.Text(
            preview_frame,
            font=('Consolas', 9),
            wrap=tk.WORD,
            height=6,
            bg='#F9FAFB'
        )
        self.preview_text.pack(fill=tk.BOTH, expand=True, padx=5, pady=5)
        
        # Bind selection change
        self.listbox.bind('<<ListboxSelect>>', self.show_preview)
        
        # Buttons
        button_frame = tk.Frame(self.root)
        button_frame.pack(fill=tk.X, padx=10, pady=10)
        
        tk.Button(
            button_frame,
            text="📋 Copy & Close",
            font=('Arial', 11, 'bold'),
            bg='#4F46E5',
            fg='white',
            command=self.copy_and_close,
            cursor='hand2'
        ).pack(side=tk.LEFT, padx=5)
        
        tk.Button(
            button_frame,
            text="🔄 Refresh",
            font=('Arial', 11),
            command=self.refresh_prompts,
            cursor='hand2'
        ).pack(side=tk.LEFT, padx=5)
        
        # Info label
        self.info_label = tk.Label(
            button_frame,
            text=f"{len(self.prompts)} prompts loaded",
            font=('Arial', 9),
            fg='#6B7280'
        )
        self.info_label.pack(side=tk.RIGHT, padx=5)
    
    def refresh_list(self, filter_text=''):
        """Refresh the prompt list"""
        self.listbox.delete(0, tk.END)
        
        filter_lower = filter_text.lower()
        
        for prompt in self.prompts:
            # Filter by title or tags
            if (filter_lower in prompt['title'].lower() or
                any(filter_lower in tag.lower() for tag in prompt['tags'])):
                
                display_text = prompt['title']
                if prompt['tags']:
                    display_text += f"  [{', '.join(prompt['tags'][:3])}]"
                
                self.listbox.insert(tk.END, display_text)
        
        # Select first item
        if self.listbox.size() > 0:
            self.listbox.selection_set(0)
            self.show_preview(None)
    
    def on_search(self, *args):
        """Handle search"""
        self.refresh_list(self.search_var.get())
    
    def show_preview(self, event):
        """Show preview of selected prompt"""
        selection = self.listbox.curselection()
        if not selection:
            return
        
        idx = selection[0]
        display_text = self.listbox.get(idx)
        
        # Find the prompt
        title = display_text.split('[')[0].strip()
        
        for prompt in self.prompts:
            if prompt['title'] == title:
                self.preview_text.delete(1.0, tk.END)
                preview = prompt['content'][:300]
                if len(prompt['content']) > 300:
                    preview += "..."
                self.preview_text.insert(1.0, preview)
                break
    
    def on_select(self, event):
        """Handle prompt selection"""
        self.copy_and_close()
    
    def copy_and_close(self):
        """Copy selected prompt and close"""
        selection = self.listbox.curselection()
        if not selection:
            messagebox.showwarning("No Selection", "Please select a prompt first!")
            return
        
        idx = selection[0]
        display_text = self.listbox.get(idx)
        title = display_text.split('[')[0].strip()
        
        # Find and copy the prompt
        for prompt in self.prompts:
            if prompt['title'] == title:
                pyperclip.copy(prompt['content'])
                messagebox.showinfo("Copied!", f"'{title}' copied to clipboard!")
                self.root.destroy()
                return
    
    def refresh_prompts(self):
        """Reload prompts from disk"""
        self.prompts = self.load_prompts()
        self.refresh_list(self.search_var.get())
        self.info_label.config(text=f"{len(self.prompts)} prompts loaded")
        messagebox.showinfo("Refreshed", f"Loaded {len(self.prompts)} prompts")
    
    def setup_hotkey(self):
        """Setup Ctrl+Shift+P hotkey"""
        def on_activate():
            self.root.deiconify()
            self.root.lift()
            self.root.focus_force()
        
        def for_canonical(f):
            return lambda k: f(listener.canonical(k))
        
        hotkey = keyboard.HotKey(
            keyboard.HotKey.parse('<ctrl>+<shift>+p'),
            on_activate
        )
        
        listener = keyboard.Listener(
            on_press=for_canonical(hotkey.press),
            on_release=for_canonical(hotkey.release)
        )
        
        listener_thread = threading.Thread(target=listener.start, daemon=True)
        listener_thread.start()
    
    def run(self):
        """Run the application"""
        self.root.mainloop()

if __name__ == '__main__':
    app = PromptSyncUI()
    app.run()
