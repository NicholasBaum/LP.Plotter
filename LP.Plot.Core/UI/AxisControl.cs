namespace LP.Plot.Core.UI;

public class AxisControl : ControlBase<Axis>
{
    private bool isPanning = false;
    private (double X, double Y) lastMousePos;

    public AxisControl(Axis content) : base(content)
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
            Content.PanRelative(Content.IsHorizontal ? panx : pany);
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
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var pos = Content.IsHorizontal ? e.X / Rect.Width : 1 - e.Y / Rect.Height;
        Content.ZoomAtRelative(factor, pos);
    }
}
