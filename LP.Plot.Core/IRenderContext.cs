using LP.Plot.Primitives;
using SkiaSharp;

public interface IRenderContext
{
    SKCanvas Canvas { get; }
    int Width { get; }
    int Height { get; }
    LPSize Size { get; }
    LPRect ClientRect { get; set; }
}