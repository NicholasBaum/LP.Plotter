using SkiaSharp;

namespace LP.Plot.UI;

public static class IControlExtension
{
    public static IDisposable TransformedCanvas(this IControl control, SKCanvas canvas)
    {
        canvas.Save();
        canvas.ClipRect(control.Rect.ToSkia());
        canvas.Translate(control.Rect.Left, control.Rect.Top);
        return new TransformedCanvasObj(canvas);
    }

    internal class TransformedCanvasObj : IDisposable
    {
        public readonly SKCanvas Canvas;

        public TransformedCanvasObj(SKCanvas canvas)
        {
            Canvas = canvas;
        }

        public void Dispose()
        {
            Canvas.Restore();
        }
    }
}
