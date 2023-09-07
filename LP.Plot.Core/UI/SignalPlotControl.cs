using LP.Plot.Core.Signal;

namespace LP.Plot.Core.UI;

public class SignalPlotControl : ControlBase<ISignalPlot>
{
    private bool isPanning = false;
    private (double X, double Y) lastMousePos;

    public SignalPlotControl(ISignalPlot content) : base(content)
    {
    }

    public override void OnMouseDown(LPMouseButtonEventArgs e)
    {
        HasMouseCapture = true;
        lastMousePos = (e.X, e.Y);
        if (e.PressedButton == LPButton.Left)
        {
            isPanning = true;
        }
    }

    public override void OnMouseMove(LPMouseButtonEventArgs e)
    {
        if (isPanning)
        {
            double deltaX = e.X - lastMousePos.X;
            double deltaY = e.Y - lastMousePos.Y;
            var panx = -deltaX / Rect.Width;
            var pany = deltaY / Rect.Height;
            Content.PanRelativeX(panx);
            Content.PanRelative(pany);
            lastMousePos = (e.X, e.Y);
        }
    }

    public override void OnMouseUp(LPMouseButtonEventArgs e)
    {
        HasMouseCapture = false;
        isPanning = false;
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
