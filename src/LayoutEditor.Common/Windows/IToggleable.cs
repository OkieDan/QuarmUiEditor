namespace LayoutEditor.Common.Windows
{
    public interface IToggleable
    {
        bool Visible { get; }
        void SetVisible(bool visible);
    }
}