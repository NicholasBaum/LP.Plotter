namespace LP.Plot.Core.UI;

public class AxisControl : PanControlBase<Axis>
{
    public AxisControl(Axis content) : base(content)
    {
    }

    public override void OnPan(double relativeX, double relativeY)
    {
        Content.PanRelative(Content.IsHorizontal ? relativeX : relativeY);
    }

    public override void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var pos = Content.IsHorizontal ? e.X / Rect.Width : 1 - e.Y / Rect.Height;
        Content.ZoomAtRelative(factor, pos);
    }
}
