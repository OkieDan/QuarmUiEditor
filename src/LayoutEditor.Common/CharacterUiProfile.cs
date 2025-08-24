using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LayoutEditor.Common.Windows;

namespace LayoutEditor.Common
{
    public class CharacterUiProfile
    {
        private readonly Dictionary<string, UiWindowBase> _windows = new();
        private readonly List<string> _sectionOrder = new();
        
        public string UiSkin => (_windows.TryGetValue("Main", out var mainWindow) && 
                               mainWindow is MainWindow main) ? main.UiSkin : string.Empty;
        
        public IReadOnlyCollection<string> WindowNames => _windows.Keys;

        public T GetWindow<T>(string name) where T : UiWindowBase
        {
            if (_windows.TryGetValue(name, out var window) && window is T typedWindow)
                return typedWindow;
            
            throw new InvalidOperationException($"Window '{name}' not found or is not of type {typeof(T).Name}");
        }

        public T GetOrCreateWindow<T>(string name) where T : UiWindowBase, new()
        {
            if (!_windows.TryGetValue(name, out var window))
            {
                window = new T { Name = name };
                _windows[name] = window;
                _sectionOrder.Add(name);
            }
            
            if (window is T typedWindow)
                return typedWindow;
            
            throw new InvalidOperationException($"Window '{name}' exists but is not of type {typeof(T).Name}");
        }

        public bool TryGetWindow<T>(string name, out T window) where T : UiWindowBase
        {
            window = default;
            
            if (_windows.TryGetValue(name, out var baseWindow) && baseWindow is T typedWindow)
            {
                window = typedWindow;
                return true;
            }
            
            return false;
        }

        public static CharacterUiProfile LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("INI file not found", filePath);
                
            string[] lines = File.ReadAllLines(filePath);
            return ParseContent(lines);
        }
        
        public static CharacterUiProfile ParseContent(string[] lines)
        {
            var profile = new CharacterUiProfile();
            string currentSection = null;
            var properties = new Dictionary<string, string>();
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;
                    
                if (Regex.IsMatch(trimmedLine, @"^\[.*\]$"))
                {
                    // Process previous section if exists
                    if (currentSection != null)
                    {
                        profile.AddWindow(currentSection, properties);
                        properties = new Dictionary<string, string>();
                    }
                    
                    // Start new section
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    profile._sectionOrder.Add(currentSection);
                }
                else if (currentSection != null && trimmedLine.Contains('='))
                {
                    // Property
                    int equalsPos = trimmedLine.IndexOf('=');
                    string key = trimmedLine.Substring(0, equalsPos);
                    string value = trimmedLine.Substring(equalsPos + 1);
                    
                    properties[key] = value;
                }
            }
            
            // Process last section
            if (currentSection != null)
            {
                profile.AddWindow(currentSection, properties);
            }
            
            return profile;
        }
        
        private void AddWindow(string name, Dictionary<string, string> properties)
        {
            UiWindowBase window;
            
            // Special sections
            if (name == "Main")
            {
                window = new MainWindow { Name = name };
            }
            else if (name == "ChatManager")
            {
                window = new ChatManagerWindow { Name = name };
            }
            else if (name.StartsWith("BagBank"))
            {
                window = new BagWindow { Name = name };
            }
            else if (name.StartsWith("BagInv"))
            {
                window = new BagWindow { Name = name };
            }
            else if (name.StartsWith("BagViewPC"))
            {
                window = new BagWindow { Name = name };
            }
            else if (name.StartsWith("Chat "))
            {
                window = new ChatWindow { Name = name };
            }
            else if (name == "RaidWindow")
            {
                window = new RaidWindow { Name = name };
            }
            // Default window with common properties
            else
            {
                window = new StandardWindow { Name = name };
            }
            
            window.LoadProperties(properties);
            _windows[name] = window;
        }
        
        public string ToIniString()
        {
            // Convert objects to INI lines without any sorting
            var lines = new List<string>();
            
            foreach (var windowName in _windows.Keys)
            {
                // Add section header
                lines.Add($"[{windowName}]");
                
                // Add window properties
                var windowLines = _windows[windowName].ToIniLines().ToList();
                lines.AddRange(windowLines);
            }
            
            // Use IniSorter's sorting logic
            return IniSorter.SortIniContent(lines.ToArray());
        }
        
        public void SaveToFile(string filePath)
        {
            string content = ToIniString();
            File.WriteAllText(filePath, content);
        }
    }
}