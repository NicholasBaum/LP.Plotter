using LP.Plot.Core.Primitives;
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
        //CreateDecimatedPath(data.YValues, data.XRange, XAxis, YAxis, ctx.ClientRect.Size);
        CreateDecimatedPath2(data.YValues, data.XRange, XAxis, YAxis, ctx.ClientRect.Size);
        //CreatePath(data.YValues, data.XRange, XAxis, YAxis, ctx.ClientRect.Size);
        ctx.Canvas.DrawPath(path, Paint);
        //RenderHighDensity(data.YValues, data.XRange, YAxis, ctx.Canvas, Paint, ctx.ClientRect.Size);
        ctx.Canvas.Restore();
    }

    // TODO: need to implement and switch to low density rendermode when necessary
    private void RenderHighDensity(double[] yValues, Span xDataRange, Axis yAxis, SKCanvas canvas, SKPaint paint, LPSize size)
    {
        var verticalBars = GetVerticalBars(yValues, xDataRange, XAxis, yAxis, size);
        for (int i = 0; i < verticalBars.Length; i++)
        {
            float x = i + 0.5f;
            canvas.DrawLine(x, (float)verticalBars[i].Min, x, (float)verticalBars[i].Max, paint);
        }
    }

    private static Span[] GetVerticalBars(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        var bars = new Span[imageWidth];
        var unitsPerPixel = xAxis.Length / imageWidth;
        var dx = xDataRange.Length / (yValues.Length - 1);
        var canvasLeft = t.ToDataSpaceX(0);
        for (var i = 0; i < imageWidth - 1; i++)
        {
            var xLeft = canvasLeft + i * unitsPerPixel;
            var xRight = xLeft + unitsPerPixel;
            if (xRight > xDataRange.Max)
                break;
            if (xDataRange.Min <= xLeft && xRight <= xDataRange.Max)
            {
                var leftIndex = (int)(xLeft / dx);
                var rightIndex = (int)(xRight / dx) + 1;
                var min = double.MaxValue;
                var max = double.MinValue;
                for (int j = leftIndex; j < rightIndex; j++)
                {
                    var y = yValues[j];
                    if (min > y)
                        min = y;
                    if (max < y)
                        max = y;
                }
                bars[i] = new Span(t.ToPixelSpaceY(min), t.ToPixelSpaceY(max));
            }
        }
        return bars;
    }

    // tranlates pixels to coordinate space and cycles through the samples taking 4 samples per pixel
    private void CreateDecimatedPath2(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        path.Reset();
        var dx = xDataRange.Length / (yValues.Length - 1); // units per sample
        var x = xDataRange.Min;
        // the method uses the start, min, max and end y-value in a pixel
        var start = new Point(0, yValues[0]);
        var min = start;
        var max = start;
        var dpx = XAxis.Length / imageWidth; // units per pixel
        var nextPixelLeftBoundary = xDataRange.Min + dpx;
        path.MoveTo(t.ToPixelSpaceX(xDataRange.Min), t.ToPixelSpaceY(yValues[0]));
        var reset = false;
        for (var i = 1; i < yValues.Length; i++)
        {
            x += dx;
            if (reset)
            {
                start = new Point(x, yValues[i]);
                min = start;
                max = start;
                nextPixelLeftBoundary += dpx;
                reset = false;
            }
            else if (x + dx > nextPixelLeftBoundary)
            {
                path.LineTo(t.ToPixelSpaceX(start.X), t.ToPixelSpaceY(start.Y));
                path.LineTo(t.ToPixelSpaceX(min.X), t.ToPixelSpaceY(min.Y));
                path.LineTo(t.ToPixelSpaceX(max.X), t.ToPixelSpaceY(max.Y));
                path.LineTo(t.ToPixelSpaceX(x), t.ToPixelSpaceY(yValues[i]));
                //var p = t.ToPixelSpaceX(nextPixelLeftBoundary);
                //path.LineTo(p - 1, t.ToPixelSpaceY(start.Y));
                //path.LineTo(p - 0.75f, t.ToPixelSpaceY(min.Y));
                //path.LineTo(p - 0.5f, t.ToPixelSpaceY(max.Y));
                //path.LineTo(p - 0.25f, t.ToPixelSpaceY(yValues[i]));
                reset = true;
            }
            else
            {
                if (min.Y > yValues[i])
                    min = new Point(x, yValues[i]);
                if (max.Y < yValues[i])
                    max = new Point(x, yValues[i]);
            }
        }
    }

    private ref struct Point
    {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    // calculates what samples lie in a pixel only works if this can be calculated easily
    // code is only prototype 
    // i think it creates step lines
    private void CreateDecimatedPath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        path.Reset();
        var unitsPerPixel = XAxis.Length / imageWidth;
        var dx = xDataRange.Length / (yValues.Length - 1);
        var offset = (xDataRange.Min - XAxis.Min) / unitsPerPixel;
        var indexOffset = (int)((xDataRange.Min - XAxis.Min) / unitsPerPixel);
        path.MoveTo(t.ToPixelSpaceX(xDataRange.Min), t.ToPixelSpaceY(yValues[0]));
        for (var i = 0; i < imageWidth - 1 - indexOffset; i++)
        {
            var xLeft = i * unitsPerPixel;
            var xRight = xLeft + unitsPerPixel;
            var leftIndex = (int)(xLeft / dx);
            var rightIndex = (int)(xRight / dx) + 1;
            var min = float.MaxValue;
            var max = float.MinValue;
            if (rightIndex >= yValues.Length)
                break;
            var start = t.ToPixelSpaceY(yValues[leftIndex]);
            var end = t.ToPixelSpaceY(yValues[rightIndex]);
            for (int j = leftIndex + 1; j < rightIndex - 1; j++)
            {
                var y = t.ToPixelSpaceY(yValues[j]);
                if (min > y)
                    min = y;
                if (max < y)
                    max = y;
            }
            if (i + indexOffset >= imageWidth)
                break;

            path.LineTo(i + (float)offset, start);
            path.LineTo(i + (float)offset + 0.25f, min);
            path.LineTo(i + (float)offset + 0.75f, max);
            path.LineTo(i + (float)offset + 1f, end);
        }
    }

    // TODO: break when pixel space x isn't in screen bounds anymore
    private void CreatePath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        path.Reset();
        var dx = xDataRange.Length / (yValues.Length - 1);
        path.MoveTo(t.ToPixelSpaceX(xDataRange.Min), t.ToPixelSpaceY(yValues[0]));
        for (int i = 1; i < yValues.Length - 1; i++)
            path.LineTo(t.ToPixelSpaceX(xDataRange.Min + i * dx), t.ToPixelSpaceY(yValues[i]));
    }
}

public class LinearTransform
{
    private readonly int pixelLength;
    private readonly double dataMin;
    private readonly double dataMax;
    private readonly double m;

    public LinearTransform(double min, double max, int pixelLength)
    {
        this.dataMin = min;
        this.dataMax = max;
        this.pixelLength = pixelLength;
        m = pixelLength / (max - min);
    }

    public LinearTransform(Span dataRange, int pixelLength) : this(dataRange.Min, dataRange.Max, pixelLength) { }

    public float ToPixelSpace(double x) => (float)((x - dataMin) * m);
    public double ToDataSpace(double x) => x / m + dataMin;
}

public class SKSpaceTransform
{
    LinearTransform xTransform;
    LinearTransform yTransform;

    public SKSpaceTransform(int imageWidth, int imageHeight, Span xRange, Span yRange)
    {
        this.xTransform = new LinearTransform(xRange, imageWidth);
        this.yTransform = new LinearTransform(yRange.Max, yRange.Min, imageHeight);// swap min/max because 0 is top in skia canvas
    }

    public float ToPixelSpaceX(double x) => xTransform.ToPixelSpace(x);
    public float ToPixelSpaceY(double y) => yTransform.ToPixelSpace(y);
    public double ToDataSpaceX(double x) => xTransform.ToDataSpace(x);
    public double ToDataSpaceY(double y) => yTransform.ToDataSpace(y);
}

