namespace LP.Plot.UI;

public class AxisControl : PanControlBase<Axis>
{
    private readonly PlotModel plot;

    public AxisControl(PlotModel plot)
    {
        this.plot = plot;
    }

    public AxisControl(Axis content, PlotModel plot) : base(content)
    {
        this.plot = plot;
    }

    public override void OnPan(double relativeX, double relativeY)
    {
        if (Content == null) return;
        Content.PanRelative(Content.IsHorizontal ? relativeX : relativeY);
        plot.Invalidate();
    }

    public override void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        if (Content == null) return;
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var pos = Content.IsHorizontal ? e.X / Rect.Width : 1 - e.Y / Rect.Height;
        Content.ZoomAtRelative(factor, pos);
        plot.Invalidate();
    }
}
