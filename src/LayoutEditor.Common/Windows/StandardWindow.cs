using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LayoutEditor.Common.Windows
{
    public class StandardWindow : UiWindowBase, IWindowProperties, IPositionable, IDimensionable, IToggleable
    {
        private readonly Dictionary<string, ResolutionPosition> _positions = new();
        
        // IWindowProperties
        public int BackgroundTintRed => GetIntProperty("BGTint.red", 255);
        public int BackgroundTintGreen => GetIntProperty("BGTint.green", 255);
        public int BackgroundTintBlue => GetIntProperty("BGTint.blue", 255);
        public bool Fades => GetBoolProperty("Fades", true);
        public int Delay => GetIntProperty("Delay", 2000);
        public int Duration => GetIntProperty("Duration", 500);
        public int Alpha => GetIntProperty("Alpha", 255);
        public int FadeToAlpha => GetIntProperty("FadeToAlpha", 128);
        public bool Locked => GetBoolProperty("Locked", false);
        
        // IToggleable
        public bool Visible => GetBoolProperty("Show", false);
        
        // IDimensionable
        public int Width => GetIntProperty("Width");
        public int Height => GetIntProperty("Height");
        
        public override void LoadProperties(Dictionary<string, string> properties)
        {
            base.LoadProperties(properties);
            
            // Extract resolution positions
            var positionPattern = new Regex(@"^(XPos|YPos)(\d+x\d+)$");
            
            foreach (var kvp in properties)
            {
                var match = positionPattern.Match(kvp.Key);
                if (match.Success)
                {
                    string type = match.Groups[1].Value;
                    string resolution = match.Groups[2].Value;
                    int value = int.Parse(kvp.Value);
                    
                    if (!_positions.TryGetValue(resolution, out var position))
                    {
                        position = new ResolutionPosition { Resolution = resolution };
                        _positions[resolution] = position;
                    }
                    
                    if (type == "XPos")
                        position.X = value;
                    else
                        position.Y = value;
                }
            }
        }
        
        public ResolutionPosition GetPosition(string resolution)
        {
            return _positions.TryGetValue(resolution, out var position) ? position : null;
        }
        
        public void SetPosition(string resolution, int x, int y)
        {
            if (!_positions.TryGetValue(resolution, out var position))
            {
                position = new ResolutionPosition { Resolution = resolution };
                _positions[resolution] = position;
            }
            
            position.X = x;
            position.Y = y;
            
            SetProperty($"XPos{resolution}", x.ToString());
            SetProperty($"YPos{resolution}", y.ToString());
        }
        
        public void SetWidth(int width)
        {
            SetProperty("Width", width.ToString());
        }
        
        public void SetHeight(int height)
        {
            SetProperty("Height", height.ToString());
        }
        
        public void SetVisible(bool visible)
        {
            SetProperty("Show", visible ? "1" : "0");
        }
        
        public void SetWindowProperties(int red, int green, int blue, bool fades, 
                                       int delay, int duration, int alpha, int fadeToAlpha, bool locked)
        {
            SetProperty("BGTint.red", red.ToString());
            SetProperty("BGTint.green", green.ToString());
            SetProperty("BGTint.blue", blue.ToString());
            SetProperty("Fades", fades.ToString().ToLower());
            SetProperty("Delay", delay.ToString());
            SetProperty("Duration", duration.ToString());
            SetProperty("Alpha", alpha.ToString());
            SetProperty("FadeToAlpha", fadeToAlpha.ToString());
            SetProperty("Locked", locked.ToString().ToLower());
        }
    }
}