using LP.Plot.Core.Signal;

namespace LP.Plot.Core.UI;

public class SignalPlotControl : PanControlBase<ISignalPlot>
{
    public SignalPlotControl(ISignalPlot content) : base(content)
    {
    }

    public override void OnPan(double relativeX, double relativeY)
    {
        Content.PanRelativeX(relativeX);
        Content.PanRelative(relativeY);
    }

    public override void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        if (Rect.IsEmpty) return;
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var xPos = e.X / Rect.Width;
        var yPos = 1 - e.Y / Rect.Height;
        Content.ZoomAtRelativeX(factor, xPos);
        Content.ZoomAtRelative(factor, yPos);
    }
}
