namespace LayoutEditor.Common.Windows
{
    public class MainWindow : UiWindowBase
    {
        public string UiSkin => GetProperty("UISkin");
        
        public void SetUiSkin(string skinName)
        {
            SetProperty("UISkin", skinName);
        }
    }
}