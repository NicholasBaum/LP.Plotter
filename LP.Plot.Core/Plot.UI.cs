using LP.Plot.Core.Layout;
using LP.Plot.Core.Signal;
using LP.Plot.Core.UI;

namespace LP.Plot.Core;

public class AxisControl : ControlBase<Axis>
{
    private bool isPanning = false;
    private (double X, double Y) lastMousePos;

    public AxisControl(Axis content) : base(content)
    {
    }

    public override void OnMouseDown(LPMouseButtonEventArgs e)
    {
        this.HasMouseCapture = true;
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
        this.HasMouseCapture = false;
        isPanning = false;
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
    private bool isPanning = false;
    private (double X, double Y) lastMousePos;

    public SignalPlotControl(ISignalPlot content) : base(content)
    {
    }

    public override void OnMouseDown(LPMouseButtonEventArgs e)
    {
        this.HasMouseCapture = true;
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
        this.HasMouseCapture = false;
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

public partial class Plot
{
    private bool isZooming = false;
    private (double X, double Y) lastMousePos;

    public void OnMouseDown(LPMouseButtonEventArgs e)
    {
        if (GetHitControl((int)e.X, (int)e.Y) is IInteraction c)
        {
            if (e.PressedButton == LPButton.Right && (c == layout.Bottom || c == layout.Center))
            {
                lastMousePos = (e.X, e.Y);
                isZooming = true;
                ZoomRect(e.X, e.Y);
            }
            else
            {
                var (x, y) = c.Transform(e.X, e.Y);
                c.OnMouseDown(new LPMouseButtonEventArgs(x, y, e.PressedButton));
            }
        }
    }

    public void OnMouseMove(LPMouseButtonEventArgs e)
    {
        if (GetHitControl((int)e.X, (int)e.Y) is IInteraction c)
        {
            if (isZooming)
            {
                lastMousePos = (e.X, e.Y);
                ZoomRect(e.X, e.Y);

            }
            else
            {
                var (x, y) = c.Transform(e.X, e.Y);
                c.OnMouseMove(new LPMouseButtonEventArgs(x, y, e.PressedButton));
            }
            Invalidate();
        }
    }

    public void OnMouseUp(LPMouseButtonEventArgs e)
    {
        if (GetHitControl((int)e.X, (int)e.Y) is IInteraction c)
        {
            if (isZooming)
            {
                lastMousePos = (e.X, e.Y);
                EndZoomRect();

            }
            else
            {
                var (x, y) = c.Transform(e.X, e.Y);
                c.OnMouseUp(new LPMouseButtonEventArgs(x, y, e.PressedButton));
            }
            isZooming = false;
            Invalidate();
        }
    }

    public void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        if (GetHitControl((int)e.X, (int)e.Y) is IInteraction c)
        {
            var (x, y) = c.Transform(e.X, e.Y);
            c.OnMouseWheel(new LPMouseWheelEventArgs(x, y, e.Delta));
            Invalidate();
        }
    }

    public IControl? GetHitControl(int x, int y)
    {
        var controls = new[]
        {
            layout.Left ,
            layout.Right ,
            layout.Bottom ,
            layout.Center
        }
        .Where(x => x is not null);

        var captured = controls.FirstOrDefault(x => x is IInteraction { HasMouseCapture: true });
        if (captured is not null) return captured;

        foreach (var c in controls)
        {
            if (c!.Rect.Contains(x, y))
                return c;
        }
        return null;
    }
}
