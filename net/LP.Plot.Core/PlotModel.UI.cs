﻿using LP.Plot.Primitives;
using LP.Plot.UI;
using SkiaSharp;

namespace LP.Plot;

public partial class PlotModel
{
    public event EventHandler<EventArgs>? Changed;
    public void Invalidate() => Changed?.Invoke(this, EventArgs.Empty);

    private bool isZooming = false;

    public void OnMouseDown(LPMouseButtonEventArgs e)
    {
        if (GetHitControl(e.Point) is IControl c)
        {
            if (e.PressedButton == LPButton.Right
                && (c == layout.Bottom || c == layout.Center))
            {
                isZooming = true;
                UpdateZoomRect(e.X, e.Y);
            }
            else
            {
                c.OnMouseDown(new LPMouseButtonEventArgs(c.Transform(e.X, e.Y), e.PressedButton));
            }
        }
    }

    public void OnMouseMove(LPMouseButtonEventArgs e)
    {
        if (isZooming)
        {
            UpdateZoomRect(e.X, e.Y);
            Invalidate();
        }
        else if (GetHitControl(e.Point) is IControl c)
        {
            c.OnMouseMove(new LPMouseButtonEventArgs(c.Transform(e.X, e.Y), e.PressedButton));
        }
    }

    public void OnMouseUp(LPMouseButtonEventArgs e)
    {
        if (isZooming)
        {
            ApplyZoomRect();
            Invalidate();
            isZooming = false;
        }
        else if (GetHitControl(e.Point) is IControl c)
        {
            c.OnMouseUp(new LPMouseButtonEventArgs(c.Transform(e.X, e.Y), e.PressedButton));
        }
    }

    public void OnMouseWheel(LPMouseWheelEventArgs e)
    {
        if (GetHitControl(e.Point) is IControl c)
        {
            c.OnMouseWheel(new LPMouseWheelEventArgs(c.Transform(e.X, e.Y), e.Delta));
        }
    }

    private IControl? GetHitControl(DPoint p)
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
            if (c.Rect.Contains((int)p.X, (int)p.Y))
                return c;
        }
        return null;
    }

    private Span? currentZoom = null;
    private Span pixelZoom = new();
    private void UpdateZoomRect(double x, double y)
    {
        var tx = new LinarTransform(signalPlot.XAxis.Range, leftAxisWidth, canvasSize.Width);
        var xx = tx.Inverse(x);

        if (currentZoom == null)
        {
            currentZoom = new(xx, xx);
            pixelZoom = new(x, x);
        }
        else
        {
            currentZoom = new(currentZoom.Value.Min, xx);
            pixelZoom = new(pixelZoom.Min, x);
        }
    }

    private void ApplyZoomRect()
    {
        if (currentZoom != null && Math.Abs(pixelZoom.Length) > 0.1)
        {
            var z = currentZoom.Value.Length > 0 ? currentZoom.Value : new Span(currentZoom.Value.Max, currentZoom.Value.Min);
            signalPlot.ZoomX(z);
        }
        currentZoom = null;
    }

    private void DrawZoomRect(IRenderContext ctx)
    {
        if (currentZoom == null || layout.Center == null)
            return;
        using (var paint = new SKPaint())
        {
            paint.Color = new SKColor(81, 170, 165, 128);
            paint.IsAntialias = true;
            SKRect rect = new SKRect(50, 50, 250, 250);
            ctx.Canvas.DrawRect((float)pixelZoom.Min, 0, (float)pixelZoom.Length, canvasSize.Height, paint);
        }
    }
}
