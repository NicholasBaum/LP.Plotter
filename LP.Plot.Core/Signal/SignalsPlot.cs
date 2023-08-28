﻿using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public class SignalsPlot : IRenderable
{
    private List<ISignal> data = new();
    public Axis XAxis = new();
    public Axis DefaultYAxis = new();
    private SKPath path = new SKPath();
    private SKPaint DefaultPaint = SKPaints.White;


    public SignalsPlot(ISignal data)
    {
        this.data.Add(data);
        this.XAxis = new Axis(data.XRange);
        this.DefaultYAxis = new Axis(data.YRange);
        this.DefaultYAxis.ZoomAtCenter(1.1);
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Black);
        foreach (ISignal signal in data)
            RenderSignal(signal, signal.YAxis ?? DefaultYAxis, signal.Paint ?? DefaultPaint, ctx.Canvas, ctx.ClientRect.Size);
        ctx.Canvas.Restore();
    }

    private void RenderSignal(ISignal data, Axis yAxis, SKPaint paint, SKCanvas canvas, LPSize imageSize)
    {
        SignalRenderer.FillDecimatedPath(data.YValues, data.XRange, XAxis, yAxis, imageSize, path);
        //SignalRenderer.FillFullPath(data.YValues, data.XRange, XAxis, yAxis, imageSize, path);
        canvas.DrawPath(path, paint);
        //SignalRenderer.DrawVerticalLines(data.YValues, data.XRange, XAxis, yAxis, canvas, paint, imageSize);
    }
}