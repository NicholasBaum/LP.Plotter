using LP.Plot.Core.UI;

namespace LP.Plot.Core;

public partial class Plot
{
    private bool isZooming = false;

    public void OnMouseDown(LPMouseButtonEventArgs e)
    {
        if (GetHitControl((int)e.X, (int)e.Y) is IControl c)
        {
            if (e.PressedButton == LPButton.Right && (c == layout.Bottom || c == layout.Center))
            {
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
        if (GetHitControl((int)e.X, (int)e.Y) is IControl c)
        {
            if (isZooming)
            {
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
        if (GetHitControl((int)e.X, (int)e.Y) is IControl c)
        {
            if (isZooming)
            {
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
        if (GetHitControl((int)e.X, (int)e.Y) is IControl c)
        {
            var (x, y) = c.Transform(e.X, e.Y);
            c.OnMouseWheel(new LPMouseWheelEventArgs(x, y, e.Delta));
            Invalidate();
        }
    }

    private IControl? GetHitControl(int x, int y)
    {
        IEnumerable<IControl> controls = new[]
        {
            layout.Left ,
            layout.Right ,
            layout.Bottom ,
            layout.Center
        }
        .Where(x => x is not null)!;

        if (controls.FirstOrDefault(x => x.HasMouseCapture) is IControl captured)
            return captured;

        foreach (var c in controls)
        {
            if (c.Rect.Contains(x, y))
                return c;
        }
        return null;
    }
}
