using LP.Plot.Primitives;
using LP.Plot.Skia;
using SkiaSharp;

namespace LP.Plot.Signal;

public static class SignalRenderer
{
    /// <summary>
    /// Draws only as many points as necessary, max 4 per pixel, theoretically this should have full quality.
    /// </summary>
    /// <param name="yValues"></param>
    /// <param name="xDataRange"></param>
    /// <param name="xAxis"></param>
    /// <param name="yAxis"></param>
    /// <param name="size"></param>
    /// <param name="path"></param>
    public static void FillDecimatedPath(double[] yValues, Span xDataRange, Span xAxis, Span yAxis, LPSize size, SKPath path)
    {
        SKTransform t = new(size, xAxis, yAxis);
        path.Reset();

        // units per sample
        var dx = xDataRange.Length / (yValues.Length - 1);
        // units per pixel
        var dpx = xAxis.Length / size.Width;

        var x = xDataRange.Min;
        var startIndex = 0;
        // restricting to visible range
        if (x < xAxis.Min)
        {
            startIndex = Math.Max(0, (int)Math.Max(0, (xAxis.Min - x) / dx - 8)); // -8 to be save            
            x = xDataRange.Min + startIndex * dx;
        }
        var endIndex = yValues.Length;
        if (xDataRange.Max > xAxis.Max)
        {
            endIndex = Math.Min(yValues.Length, startIndex + (int)(xAxis.Length / dx) + 8);
        }
        var nextPixelLeftBoundary = x + dpx;
        path.MoveTo(t.ToScreenSpaceX(x), t.ToScreenSpaceY(yValues[startIndex]));

        // the method uses the start, min, max and end y-value in a pixel
        var start = new Point(x, yValues[startIndex]);
        var min = start;
        var max = start;

        var reset = false;
        for (var i = startIndex + 1; i < endIndex; i++)
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
                path.LineTo(t.ToScreenSpaceX(start.X), t.ToScreenSpaceY(start.Y));
                path.LineTo(t.ToScreenSpaceX(min.X), t.ToScreenSpaceY(min.Y));
                path.LineTo(t.ToScreenSpaceX(max.X), t.ToScreenSpaceY(max.Y));
                path.LineTo(t.ToScreenSpaceX(x), t.ToScreenSpaceY(yValues[i]));
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

    private readonly ref struct Point
    {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    // TODO: break when pixel space x isn't in screen bounds anymore
    /// <summary>
    /// Maximum quality by using all points.
    /// </summary>
    /// <param name="yValues"></param>
    /// <param name="xDataRange"></param>
    /// <param name="xAxis"></param>
    /// <param name="yAxis"></param>
    /// <param name="size"></param>
    /// <param name="path"></param>
    public static void FillFullPath(double[] yValues, Span xDataRange, Span xAxis, Span yAxis, LPSize size, SKPath path)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKTransform t = new(imageWidth, imageHeight, xAxis, yAxis);
        path.Reset();
        var dx = xDataRange.Length / (yValues.Length - 1);
        path.MoveTo(t.ToScreenSpaceX(xDataRange.Min), t.ToScreenSpaceY(yValues[0]));
        for (int i = 1; i < yValues.Length - 1; i++)
            path.LineTo(t.ToScreenSpaceX(xDataRange.Min + i * dx), t.ToScreenSpaceY(yValues[i]));
    }

    // TODO: need to implement and switch to low density rendermode when necessary
    /// <summary>
    /// If sample density is high this methods only draws some vertically lines for maximum performance.
    /// </summary>
    /// <param name="yValues"></param>
    /// <param name="xDataRange"></param>
    /// <param name="xAxis"></param>
    /// <param name="yAxis"></param>
    /// <param name="canvas"></param>
    /// <param name="paint"></param>
    /// <param name="size"></param>
    public static void DrawVerticalLines(double[] yValues, Span xDataRange, Span xAxis, Span yAxis, SKCanvas canvas, SKPaint paint, LPSize size)
    {
        var verticalBars = GetVerticalBars(yValues, xDataRange, xAxis, yAxis, size);
        for (int i = 0; i < verticalBars.Length; i++)
        {
            float x = i + 0.5f;
            canvas.DrawLine(x, (float)verticalBars[i].Min, x, (float)verticalBars[i].Max, paint);
        }
    }

    private static Span[] GetVerticalBars(double[] yValues, Span xDataRange, Span xAxis, Span yAxis, LPSize size)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKTransform t = new(imageWidth, imageHeight, xAxis, yAxis);
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
                bars[i] = new Span(t.ToScreenSpaceY(min), t.ToScreenSpaceY(max));
            }
        }
        return bars;
    }

    // calculates what samples lie in a pixel only works if this can be calculated easily
    // code is only prototype 
    // i think it creates step lines
    private static void CreateDecimatedPath(double[] yValues, Span xDataRange, Span xAxis, Span yAxis, LPSize size, SKPath path)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKTransform t = new(imageWidth, imageHeight, xAxis, yAxis);
        path.Reset();
        var unitsPerPixel = xAxis.Length / imageWidth;
        var dx = xDataRange.Length / (yValues.Length - 1);
        var offset = (xDataRange.Min - xAxis.Min) / unitsPerPixel;
        var indexOffset = (int)((xDataRange.Min - xAxis.Min) / unitsPerPixel);
        path.MoveTo(t.ToScreenSpaceX(xDataRange.Min), t.ToScreenSpaceY(yValues[0]));
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
            var start = t.ToScreenSpaceY(yValues[leftIndex]);
            var end = t.ToScreenSpaceY(yValues[rightIndex]);
            for (int j = leftIndex + 1; j < rightIndex - 1; j++)
            {
                var y = t.ToScreenSpaceY(yValues[j]);
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
}
