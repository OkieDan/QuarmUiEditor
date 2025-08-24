namespace LayoutEditor.Common.Windows
{
    public interface IWindowProperties
    {
        int BackgroundTintRed { get; }
        int BackgroundTintGreen { get; }
        int BackgroundTintBlue { get; }
        bool Fades { get; }
        int Delay { get; }
        int Duration { get; }
        int Alpha { get; }
        int FadeToAlpha { get; }
        bool Locked { get; }
    }
}