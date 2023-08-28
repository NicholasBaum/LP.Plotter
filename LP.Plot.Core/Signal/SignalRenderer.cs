using LP.Plot.Core.Primitives;
using LP.Plot.Core.Skia;
using SkiaSharp;

namespace LP.Plot.Core.Signal;

public static class SignalRenderer
{
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
    public static void FillFullPath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size, SKPath path)
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

    // tranlates pixels to coordinate space and cycles through the samples taking 4 samples per pixel
    /// <summary>
    /// Draws only as many points as necessary, max 4 per pixel, theoretically this should have full quality.
    /// </summary>
    /// <param name="yValues"></param>
    /// <param name="xDataRange"></param>
    /// <param name="xAxis"></param>
    /// <param name="yAxis"></param>
    /// <param name="size"></param>
    /// <param name="path"></param>
    public static void FillDecimatedPath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size, SKPath path)
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
        var dpx = xAxis.Length / imageWidth; // units per pixel
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
    public static void DrawVerticalLines(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, SKCanvas canvas, SKPaint paint, LPSize size)
    {
        var verticalBars = GetVerticalBars(yValues, xDataRange, xAxis, yAxis, size);
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

    // calculates what samples lie in a pixel only works if this can be calculated easily
    // code is only prototype 
    // i think it creates step lines
    private static void CreateDecimatedPath(double[] yValues, Span xDataRange, Axis xAxis, Axis yAxis, LPSize size, SKPath path)
    {
        var imageWidth = size.Width;
        var imageHeight = size.Height;
        SKSpaceTransform t = new(imageWidth, imageHeight, xAxis.Range, yAxis.Range);
        path.Reset();
        var unitsPerPixel = xAxis.Length / imageWidth;
        var dx = xDataRange.Length / (yValues.Length - 1);
        var offset = (xDataRange.Min - xAxis.Min) / unitsPerPixel;
        var indexOffset = (int)((xDataRange.Min - xAxis.Min) / unitsPerPixel);
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
}
