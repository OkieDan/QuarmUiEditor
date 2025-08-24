namespace LayoutEditor.Common.Windows
{
    public interface IDimensionable
    {
        int Width { get; }
        int Height { get; }
        void SetWidth(int width);
        void SetHeight(int height);
    }
}