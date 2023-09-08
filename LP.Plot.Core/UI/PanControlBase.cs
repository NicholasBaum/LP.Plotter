using LP.Plot.Primitives;

namespace LP.Plot.UI;

public abstract class PanControlBase<T> : ControlBase<T> where T : IRenderable
{
    protected bool isPanning = false;
    protected DPoint lastMousePos;

    protected PanControlBase(T content) : base(content)
    {
    }

    public override void OnMouseDown(LPMouseButtonEventArgs e)
    {
        HasMouseCapture = true;
        lastMousePos = new(e.X, e.Y);
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
            lastMousePos = new(e.X, e.Y);
            var panx = -deltaX / Rect.Width;
            var pany = deltaY / Rect.Height;
            OnPan(panx, pany);
        }
    }

    public override void OnMouseUp(LPMouseButtonEventArgs e)
    {
        HasMouseCapture = false;
        isPanning = false;
    }

    public virtual void OnPan(double relativeX, double relativeY) { }
}
