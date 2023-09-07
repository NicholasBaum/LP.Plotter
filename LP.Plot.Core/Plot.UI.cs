using LP.Plot.Core.Layout;
using LP.Plot.Core.Signal;
using LP.Plot.Core.UI;

namespace LP.Plot.Core;

public class AxisControl : ControlBase<Axis>
{
    public AxisControl(Axis content) : base(content)
    {
    }

    public override void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var pos = Content.IsHorizontal ? e.X / Rect.Width : 1 - e.Y / Rect.Height;
        this.Content.ZoomAtRelative(factor, pos);
    }
}

public class SignalPlotControl : ControlBase<ISignalPlot>
{
    public SignalPlotControl(ISignalPlot content) : base(content)
    {
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

public partial class Plot
{
    private bool isPanning = false;
    private bool isZooming = false;
    private (double X, double Y) lastMousePos;

    public void OnMouseDown(LPMouseButtonEventArgs e)
    {
        lastMousePos = (e.X, e.Y);
        if (!isZooming && e.PressedButton == LPButton.Left)
        {
            isPanning = true;
        }
        if (!isPanning && e.PressedButton == LPButton.Right)
        {
            isZooming = true;
            ZoomRect(e.X, e.Y);
        }
    }

    public void OnMouseMove(LPMouseButtonEventArgs e)
    {
        if (isPanning)
        {
            double deltaX = e.X - lastMousePos.X;
            double deltaY = e.Y - lastMousePos.Y;
            var panx = -deltaX / canvasSize.Width;
            var pany = deltaY / canvasSize.Height;
            PanRelative(panx, pany);
            lastMousePos = (e.X, e.Y);
            Invalidate();
        }
        else if (isZooming)
        {
            ZoomRect(e.X, e.Y);
            Invalidate();
        }
    }

    public void OnMouseUp(LPMouseButtonEventArgs e)
    {
        isPanning = false;
        if (isZooming)
        {
            EndZoomRect();
            Invalidate();
        }
        isZooming = false;
    }

    public void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        foreach (var c in new[] { layout.Left as AxisControl, layout.Right as AxisControl, layout.Bottom as AxisControl }
        .Where(x => x is not null))
        {
            if (c!.Rect.Contains((int)e.X, (int)e.Y))
            {
                var (x, y) = c.Transform(e.X, e.Y);
                c.OnMouseWheel(new LPMouseWheelEventArgs(x, y, e.Delta));
                Invalidate();
                return;
            }
        }
        var center = layout.Center as SignalPlotControl;
        if (center.Rect.Contains((int)e.X, (int)e.Y))
        {
            var (x, y) = center.Transform(e.X, e.Y);
            center.OnMouseWheel(new LPMouseWheelEventArgs(x, y, e.Delta));
            Invalidate();
        }
    }
}
