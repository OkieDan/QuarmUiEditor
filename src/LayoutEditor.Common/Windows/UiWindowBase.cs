using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutEditor.Common.Windows
{
    public abstract class UiWindowBase
    {
        private readonly Dictionary<string, string> _properties = new();
        private readonly List<string> _propertyOrder = new();
        
        public string Name { get; init; }
        
        public virtual void LoadProperties(Dictionary<string, string> properties)
        {
            foreach (var kvp in properties)
            {
                SetProperty(kvp.Key, kvp.Value);
            }
        }
        
        protected void SetProperty(string key, string value)
        {
            if (!_properties.ContainsKey(key))
                _propertyOrder.Add(key);
                
            _properties[key] = value;
        }
        
        protected string GetProperty(string key, string defaultValue = null)
        {
            return _properties.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        protected int GetIntProperty(string key, int defaultValue = 0)
        {
            return _properties.TryGetValue(key, out var value) && int.TryParse(value, out var result) 
                ? result : defaultValue;
        }
        
        protected bool GetBoolProperty(string key, bool defaultValue = false)
        {
            if (!_properties.TryGetValue(key, out var value))
                return defaultValue;
                
            if (bool.TryParse(value, out bool result))
                return result;
                
            if (int.TryParse(value, out int intResult))
                return intResult != 0;
                
            return defaultValue;
        }
        
        public virtual string ToIniString()
        {
            var sb = new StringBuilder();
            
            foreach (var key in _propertyOrder)
            {
                if (_properties.TryGetValue(key, out var value))
                {
                    sb.AppendLine($"{key}={value}");
                }
            }
            
            return sb.ToString();
        }
        
        // Return individual lines for each property
        public virtual IEnumerable<string> ToIniLines()
        {
            foreach (var key in _propertyOrder)
            {
                if (_properties.TryGetValue(key, out var value))
                {
                    yield return $"{key}={value}";
                }
            }
        }
    }
}