using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public class SignalRenderer : IRenderable
{
    private ISignalSource data;
    SKPaint Paint { get; set; } = SKPaints.White;
    public Axis XAxis = new();
    public Axis YAxis = new();
    private SKPath path = new SKPath();


    public SignalRenderer(ISignalSource data)
    {
        this.data = data;
        this.XAxis = new Axis(data.XRange);
        this.YAxis = new Axis(data.YRange);
        this.YAxis.ZoomAtCenter(1.1);
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Save();
        ctx.Canvas.ClipRect(ctx.ClientRect.ToSkia());
        ctx.Canvas.Translate(ctx.ClientRect.Left, ctx.ClientRect.Top);
        ctx.Canvas.Clear(SKColors.Black);
        RenderMethods.CreateDecimatedPath(data.YValues, data.XRange, XAxis, YAxis, ctx.ClientRect.Size, path);
        RenderMethods.CreateDecimatedPath2(data.YValues, data.XRange, XAxis, YAxis, ctx.ClientRect.Size, path);
        RenderMethods.CreatePath(data.YValues, data.XRange, XAxis, YAxis, ctx.ClientRect.Size, path);
        ctx.Canvas.DrawPath(path, Paint);
        RenderMethods.RenderHighDensity(data.YValues, data.XRange, XAxis, YAxis, ctx.Canvas, Paint, ctx.ClientRect.Size);
        ctx.Canvas.Restore();
    }
}