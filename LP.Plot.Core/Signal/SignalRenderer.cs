using LP.Plot.Core.Primitives;
using LP.Plot.Skia;
using LP.Plotter.Core;
using SkiaSharp;
using System.Runtime.Intrinsics.X86;

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
        this.YAxis = new Axis(data.YRange).Scale(1.1);
    }

    public void Render(IRenderContext ctx)
    {
        ctx.Canvas.Clear(SKColors.Black);

        CreateDecimatedPath(data.YValues, data.XRange, XAxis, YAxis, ctx.Width, ctx.Height);
        ctx.Canvas.DrawPath(path, Paint);
        //RenderHighDensity(data.YValues, data.XRange, YAxis, ctx.Canvas, Paint, ctx.Width, ctx.Height);

    }

    private void RenderHighDensity(double[] yValues, Span xDataRange, Axis yAxis, SKCanvas canvas, SKPaint paint, int imageWidth, int imageHeight)
    {
        PixelRange[] verticalBars = GetVerticalBars(yValues, xDataRange, yAxis, imageWidth, imageHeight);
        for (int i = 0; i < verticalBars.Length; i++)
        {
            float x = i + 0.5f;
            canvas.DrawLine(x, (float)verticalBars[i].Min, x, (float)verticalBars[i].Max, paint);
        }
    }

    private PixelRange[] GetVerticalBars(double[] yValues, Span xDataRange, Axis yAxis, int imageWidth, int imageHeight)
    {
        var m = imageHeight / (yAxis.Min - yAxis.Max);
        var bars = new PixelRange[imageWidth];
        var unitsPerPixel = XAxis.Length / imageWidth;
        var unitsPerIndex = xDataRange.Length / (yValues.Length - 1);
        var indexOffset = (int)((xDataRange.Min - XAxis.Min) / unitsPerPixel);
        for (var i = 0; i < imageWidth - 1 - indexOffset; i++)
        {
            var xLeft = i * unitsPerPixel;
            var xRight = xLeft + unitsPerPixel;
            var leftIndex = (int)(xLeft / unitsPerIndex);
            var rightIndex = (int)(xRight / unitsPerIndex) + 1;
            var min = double.MaxValue;
            var max = double.MinValue;
            if (rightIndex >= yValues.Length)
                break;
            for (int j = leftIndex; j < rightIndex; j++)
            {
                var y = (yValues[j] - yAxis.Max) * m;
                if (min > y)
                    min = y;
                if (max < y)
                    max = y;
            }
            if (i + indexOffset >= imageWidth)
                break;
            bars[i + indexOffset] = new PixelRange(min, max);
        }

        return bars;
    }


    private void CreateDecimatedPath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, int imageWidth, int imageHeight)
    {
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        path.Reset();
        var unitsPerPixel = XAxis.Length / imageWidth;
        var unitsPerIndex = xDataRange.Length / (yValues.Length - 1);
        var offset = (xDataRange.Min - XAxis.Min) / unitsPerPixel;
        var indexOffset = (int)((xDataRange.Min - XAxis.Min) / unitsPerPixel);
        path.MoveTo(t.ToPixelSpaceX(xDataRange.Min), t.ToPixelSpaceY(yValues[0]));
        for (var i = 0; i < imageWidth - 1 - indexOffset; i++)
        {
            var xLeft = i * unitsPerPixel;
            var xRight = xLeft + unitsPerPixel;
            var leftIndex = (int)(xLeft / unitsPerIndex);
            var rightIndex = (int)(xRight / unitsPerIndex) + 1;
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
    private void CreatePath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, int imageWidth, int imageHeight)
    {
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        path.Reset();
        var dx = data.Period;
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
}

