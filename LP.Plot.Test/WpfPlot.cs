using LP.Plot.Core;
using LP.Plot.Core.Signal;
using LP.Plot.Core.Skia;
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
        : this(new List<ISignal> { data }, control, skiaEl) { }

    public WpfPlot(IEnumerable<ISignal> data, Control control, SKElement skiaEl)
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
        renderInfo.PaintRenderInfo(e.Surface.Canvas);
        Debug.WriteLine($"Rendertime {frameTimer.Elapsed.TotalSeconds}");
    }

    private bool isPanning = false;
    private bool isZooming = false;

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!isZooming && e.LeftButton == MouseButtonState.Pressed)
        {
            isPanning = true;
            renderInfo.RestartMeasuring();
            lastMousePos = e.GetPosition(control);
            control.CaptureMouse(); // Capture mouse events to track movement outside the control
        }
        if (!isPanning && e.RightButton == MouseButtonState.Pressed)
        {
            isZooming = true;
            var pos = e.GetPosition(skiaEl);
            plot.ZoomRect(pos.X, pos.Y);
            skiaEl.InvalidateVisual();
            control.CaptureMouse();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!control.IsMouseCaptured) return;
        if (isPanning)
        {
            Point newMousePos = e.GetPosition(control);
            double deltaX = newMousePos.X - lastMousePos.X;
            double deltaY = newMousePos.Y - lastMousePos.Y;
            var panx = -deltaX / skiaEl.ActualWidth;
            var pany = deltaY / skiaEl.ActualHeight;
            plot.PanRelative(panx, pany);
            skiaEl.InvalidateVisual();
            lastMousePos = newMousePos;
        }
        else if (isZooming)
        {
            var pos = e.GetPosition(skiaEl);
            plot.ZoomRect(pos.X, pos.Y);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (control.IsMouseCaptured)
            control.ReleaseMouseCapture();
        isPanning = false;
        if (isZooming)
        {
            plot.EndZoomRect();
            skiaEl.InvalidateVisual();
        }
        isZooming = false;

    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var factor = Math.Sign(e.Delta) > 0 ? 0.9 : 1.1;
        var pos = e.GetPosition(skiaEl);
        var xPos = pos.X / skiaEl.ActualWidth;
        var yPos = 1 - pos.Y / skiaEl.ActualHeight;
        plot.ZoomAtRelative(factor, xPos, yPos);
        skiaEl.InvalidateVisual();
    }
}
