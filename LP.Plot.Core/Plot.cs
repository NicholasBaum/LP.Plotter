
using SkiaSharp;

namespace LP.Plotter.Core;

public class Plot : IRenderable
{
    List<IRenderable> Plotables { get; set; } = null!;

    public void Render(IRenderContext ctx)
    {
        foreach (var r in Plotables)
            r.Render(ctx);
    }
}

public interface IRenderable
{
    void Render(IRenderContext ctx);
}

public interface IRenderContext
{
    SKCanvas Canvas { get; }
    int Width { get; }
    int Height { get; }
}

public class SkiaRenderContext : IRenderContext
{
    public SKCanvas Canvas { get; }
    public int Width { get; }
    public int Height { get; }

    public SkiaRenderContext(SKCanvas canvas, int width, int height)
    {
        Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        Width = width;
        Height = height;
    }
}
