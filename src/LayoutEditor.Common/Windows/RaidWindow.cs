using System.Collections.Generic;

namespace LayoutEditor.Common.Windows
{
    public class RaidWindow : StandardWindow
    {
        private Dictionary<int, int> _classColors = new();
        
        public override void LoadProperties(Dictionary<string, string> properties)
        {
            base.LoadProperties(properties);
            
            // Extract class colors
            foreach (var kvp in properties)
            {
                if (kvp.Key.StartsWith("ClassColor") && int.TryParse(kvp.Key.Substring(10), out int index))
                {
                    if (int.TryParse(kvp.Value, out int color))
                    {
                        _classColors[index] = color;
                    }
                }
            }
        }
        
        public int GetClassColor(int classIndex)
        {
            return _classColors.TryGetValue(classIndex, out int color) ? color : 0;
        }
        
        public void SetClassColor(int classIndex, int color)
        {
            _classColors[classIndex] = color;
            SetProperty($"ClassColor{classIndex}", color.ToString());
        }
    }
}