﻿using LP.Plot.Signal;

namespace LP.Plot.UI;

public class SignalPlotControl : PanControlBase<ISignalPlot>
{
    private readonly PlotModel plot;

    public SignalPlotControl(ISignalPlot content, PlotModel plot) : base(content)
    {
        this.plot = plot;
    }

    public override void OnPan(double relativeX, double relativeY)
    {
        Content.PanRelativeX(relativeX);
        Content.PanRelative(relativeY);
        plot.Invalidate();
    }

    public override void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        if (Rect.IsEmpty) return;
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var xPos = e.X / Rect.Width;
        var yPos = 1 - e.Y / Rect.Height;
        Content.ZoomAtRelativeX(factor, xPos);
        Content.ZoomAtRelative(factor, yPos);
        plot.Invalidate();
    }
}
