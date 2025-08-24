namespace LayoutEditor.Common.Windows
{
    public interface IPositionable
    {
        ResolutionPosition GetPosition(string resolution);
        void SetPosition(string resolution, int x, int y);
    }
}