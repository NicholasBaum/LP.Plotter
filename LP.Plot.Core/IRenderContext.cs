using LP.Plot.Primitives;
using SkiaSharp;

namespace LP.Plot;

public interface IRenderContext
{
    SKCanvas Canvas { get; }
    int Width { get; }
    int Height { get; }
    LPSize Size { get; }
    LPRect ClientRect { get; set; }
}