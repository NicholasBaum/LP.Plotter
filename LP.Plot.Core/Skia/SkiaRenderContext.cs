using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Skia;

public class SkiaRenderContext : IRenderContext
{
    public SKCanvas Canvas { get; }
    public int Width { get; }
    public int Height { get; }
    public LPSize Size => new LPSize(Width, Height);
    public LPRect ClientRect { get; set; }

    public SkiaRenderContext(SKCanvas canvas, int width, int height)
    {
        Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        Width = width;
        Height = height;
    }
}
