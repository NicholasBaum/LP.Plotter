using LP.Plot.Core.Primitives;
using SkiaSharp;

namespace LP.Plot.Core.Layout;

public interface IControl : IRenderable
{
    public IControl? Parent { get; }
    public LPSize DesiredSize { get; }
    public LPRect Rect { get; }
    public void SetRect(LPRect rect);

    public IDisposable TransformedCanvas(SKCanvas canvas)
    {
        canvas.Save();
        canvas.ClipRect(Rect.ToSkia());
        canvas.Translate(Rect.Left, Rect.Top);
        return new TransformedCanvasObj(canvas);
    }

    internal class TransformedCanvasObj : IDisposable
    {
        public readonly SKCanvas Canvas;

        public TransformedCanvasObj(SKCanvas canvas)
        {
            this.Canvas = canvas;
        }

        public void Dispose()
        {
            Canvas.Restore();
        }
    }
}
