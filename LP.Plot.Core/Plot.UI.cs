namespace LP.Plot.Core;

public enum LPButton
{
    None, Left, Right
}

public class LPMouseButtonEventArgs
{
    public LPMouseButtonEventArgs(double x, double y, LPButton pressedButton)
    {
        PressedButton = pressedButton;
        X = x;
        Y = y;
    }

    public LPButton PressedButton { get; }
    public double X { get; }
    public double Y { get; }
}

public class LPMouseWheelEventArgs
{
    public LPMouseWheelEventArgs(double x, double y, double delta)
    {
        Delta = delta;
        X = x;
        Y = y;
    }

    public double Delta { get; }
    public double X { get; }
    public double Y { get; }
}

public partial class Plot
{
    private bool isPanning = false;
    private bool isZooming = false;
    private (double X, double Y) lastMousePos;

    //TODO: this is a terrible implementation, just use events :-D
    public Action Invalidate { get; set; } = () => { };

    public void OnMouseDown(LPMouseButtonEventArgs e)
    {
        lastMousePos = (e.X, e.Y);
        if (!isZooming && e.PressedButton == LPButton.Left)
        {
            renderInfo.RestartMeasuring();
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
        var factor = e.Delta > 0 ? 0.9 : 1.1;
        var xPos = e.X / canvasSize.Width;
        var yPos = 1 - e.Y / canvasSize.Height;
        ZoomAtRelative(factor, xPos, yPos);
        Invalidate();
    }
}
