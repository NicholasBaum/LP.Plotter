using LP.Plot.Signal;
using LP.Plot.Skia;
using LP.Plot.UI;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Windows.Controls;
using System.Windows.Input;

namespace LP.Plot.Test;

public class WpfPlotView
{
    private Control control;
    private SKElement skiaEl;
    public readonly Plot Plot;

    public WpfPlotView(ISignal data, Control control, SKElement skiaEl)
        : this(new List<ISignal> { data }, control, skiaEl) { }

    public WpfPlotView(IEnumerable<ISignal> data, Control control, SKElement skiaEl)
    {
        Plot = new(data, "Time");

        skiaEl.PaintSurface += OnPaintSurface;
        control.MouseDown += OnMouseDown;
        control.MouseMove += OnMouseMove;
        control.MouseUp += OnMouseUp;
        control.MouseWheel += OnMouseWheel;
        this.control = control;
        this.skiaEl = skiaEl;
        Plot.Changed += (_, _) => skiaEl.InvalidateVisual();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        Plot.Render(new SkiaRenderContext(e.Surface.Canvas, e.Info.Width, e.Info.Height));
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            control.CaptureMouse();
        Plot.OnMouseDown(Create(e));
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!control.IsMouseCaptured) return;
        Plot.OnMouseMove(Create(e));
        control.CaptureMouse();
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (control.IsMouseCaptured)
            control.ReleaseMouseCapture();
        Plot.OnMouseUp(Create(e));
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(skiaEl);
        Plot.OnMouseWheel(new(pos.X, pos.Y, e.Delta));
    }

    private LPMouseButtonEventArgs Create(MouseEventArgs e)
    {
        var pos = e.GetPosition(skiaEl);
        LPButton button = LPButton.None;
        if (e.LeftButton == MouseButtonState.Pressed)
            button = LPButton.Left;
        else if (e.RightButton == MouseButtonState.Pressed)
            button = LPButton.Right;
        return new LPMouseButtonEventArgs(pos.X, pos.Y, button);
    }
}
