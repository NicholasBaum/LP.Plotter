using LP.Plot.Primitives;
using SkiaSharp;

namespace LP.Plot.Skia;

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
