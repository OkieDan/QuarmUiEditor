using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LayoutEditor.Common.Windows
{
    public class ChatManagerWindow : UiWindowBase
    {
        private readonly Dictionary<int, Dictionary<string, string>> _chatWindowProperties = new();
        private readonly Dictionary<int, int> _channelMaps = new();
        private readonly Dictionary<int, int> _hitModes = new();
        
        public int NumWindows => GetIntProperty("NumWindows");
        
        public override void LoadProperties(Dictionary<string, string> properties)
        {
            base.LoadProperties(properties);
            
            var chatWindowPattern = new Regex(@"^ChatWindow(\d+)_(.+)$");
            var channelMapPattern = new Regex(@"^ChannelMap(\d+)$");
            var hitModePattern = new Regex(@"^HitMode(\d+)$");
            
            foreach (var kvp in properties)
            {
                // Process chat window properties
                var match = chatWindowPattern.Match(kvp.Key);
                if (match.Success)
                {
                    int windowIndex = int.Parse(match.Groups[1].Value);
                    string propName = match.Groups[2].Value;
                    
                    if (!_chatWindowProperties.TryGetValue(windowIndex, out var props))
                    {
                        props = new Dictionary<string, string>();
                        _chatWindowProperties[windowIndex] = props;
                    }
                    
                    props[propName] = kvp.Value;
                    continue;
                }
                
                // Process channel maps
                match = channelMapPattern.Match(kvp.Key);
                if (match.Success)
                {
                    int channelIndex = int.Parse(match.Groups[1].Value);
                    int value = int.Parse(kvp.Value);
                    _channelMaps[channelIndex] = value;
                    continue;
                }
                
                // Process hit modes
                match = hitModePattern.Match(kvp.Key);
                if (match.Success)
                {
                    int hitModeIndex = int.Parse(match.Groups[1].Value);
                    int value = int.Parse(kvp.Value);
                    _hitModes[hitModeIndex] = value;
                }
            }
        }
        
        public string GetChatWindowProperty(int windowIndex, string propertyName)
        {
            if (_chatWindowProperties.TryGetValue(windowIndex, out var props) && 
                props.TryGetValue(propertyName, out var value))
                return value;
                
            return null;
        }
        
        public void SetChatWindowProperty(int windowIndex, string propertyName, string value)
        {
            if (!_chatWindowProperties.TryGetValue(windowIndex, out var props))
            {
                props = new Dictionary<string, string>();
                _chatWindowProperties[windowIndex] = props;
            }
            
            props[propertyName] = value;
            SetProperty($"ChatWindow{windowIndex}_{propertyName}", value);
        }
        
        public int GetChannelMap(int channelIndex)
        {
            return _channelMaps.TryGetValue(channelIndex, out int value) ? value : 0;
        }
        
        public void SetChannelMap(int channelIndex, int value)
        {
            _channelMaps[channelIndex] = value;
            SetProperty($"ChannelMap{channelIndex}", value.ToString());
        }
        
        public int GetHitMode(int hitModeIndex)
        {
            return _hitModes.TryGetValue(hitModeIndex, out int value) ? value : 0;
        }
        
        public void SetHitMode(int hitModeIndex, int value)
        {
            _hitModes[hitModeIndex] = value;
            SetProperty($"HitMode{hitModeIndex}", value.ToString());
        }
        
        public void SetNumWindows(int numWindows)
        {
            SetProperty("NumWindows", numWindows.ToString());
        }
    }
}