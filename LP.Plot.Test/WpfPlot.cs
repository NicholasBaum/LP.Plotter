using LP.Plot.Core;
using LP.Plot.Core.Signal;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LP.Plot.Test;

public class WpfPlot
{
    private RenderInfo renderInfo = new();
    private Stopwatch frameTimer = new Stopwatch();
    private Point lastMousePos;
    private Control control;
    private SKElement skiaEl;
    private Core.Plot plot;

    public WpfPlot(ISignal data, Control control, SKElement skiaEl)
    {
        plot = Core.Plot.CreateSignal(data);

        skiaEl.PaintSurface += OnPaintSurface;
        control.MouseDown += OnMouseDown;
        control.MouseMove += OnMouseMove;
        control.MouseUp += OnMouseUp;
        control.MouseWheel += OnMouseWheel;
        this.control = control;
        this.skiaEl = skiaEl;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        frameTimer.Restart();
        plot.Render(new SkiaRenderContext(e.Surface.Canvas, e.Info.Width, e.Info.Height));
        renderInfo.PaintRenderInfo(e.Surface.Canvas, e.Info);
        Debug.WriteLine($"Rendertime {frameTimer.Elapsed.TotalSeconds}:0.00");
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        renderInfo.Restart();
        lastMousePos = e.GetPosition(control);
        control.CaptureMouse(); // Capture mouse events to track movement outside the control
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (control.IsMouseCaptured)
        {
            Point newMousePos = e.GetPosition(control);
            double deltaX = newMousePos.X - lastMousePos.X;
            double deltaY = newMousePos.Y - lastMousePos.Y;
            var panx = -deltaX / skiaEl.ActualWidth;
            var pany = deltaY / skiaEl.ActualHeight;
            plot.Pan(panx, pany);
            skiaEl.InvalidateVisual();
            lastMousePos = newMousePos;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (control.IsMouseCaptured)
            control.ReleaseMouseCapture();
        var diff = lastMousePos - e.GetPosition(control);
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var factor = Math.Sign(e.Delta) > 0 ? 0.9 : 1.1;
        var pos = e.GetPosition(skiaEl);
        var xPos = pos.X / skiaEl.ActualWidth;
        var yPos = 1 - pos.Y / skiaEl.ActualHeight;
        plot.ZoomAt(factor, xPos, yPos);
        skiaEl.InvalidateVisual();
    }
}
