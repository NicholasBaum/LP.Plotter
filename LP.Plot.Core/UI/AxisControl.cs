namespace LP.Plot.UI;

public class AxisControl : PanControlBase<Axis>
{
    private readonly Plot plot;

    public AxisControl(Axis content, Plot plot) : base(content)
    {
        this.plot = plot;
    }

    public override void OnPan(double relativeX, double relativeY)
    {
        Content.PanRelative(Content.IsHorizontal ? relativeX : relativeY);
        plot.Invalidate();
    }

    public override void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var pos = Content.IsHorizontal ? e.X / Rect.Width : 1 - e.Y / Rect.Height;
        Content.ZoomAtRelative(factor, pos);
        plot.Invalidate();
    }
}
