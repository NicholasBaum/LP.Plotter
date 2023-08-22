using LP.Plotter.Core.Models;
using OxyPlot;
using SkiaSharp;
using System.Diagnostics;

namespace LP.Plotter.Core.Drawing;

public class SignalPlotter
{
    private readonly VChannelSet data;
    private List<(float[] yvalues, Axis YAxis, SKPaint Paint)> channels;
    public Axis XAxis = new();
    public Axis TempAxis = new();
    public Axis XDataRange = new();
    public SignalPlotter(VChannelSet data)
    {
        this.data = data;

        paints.Add(paint1);
        paints.Add(paint2);
        paints.Add(paint3);
        paints.Add(paint4);
        paints.Add(paint5);

        this.channels = new();
        foreach (var c in data.Channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TT")))
        {
            var yValues = c.Points.Select(x => (float)x.Y).ToArray();
            var yaxis = new Axis() { Min = (float)yValues.Min(), Max = (float)yValues.Max() }.Scale(1.1f);
            if (c.Name.Contains("TT"))
            {
                TempAxis.Min = Math.Min(TempAxis.Min, yaxis.Min);
                TempAxis.Max = Math.Max(TempAxis.Max, yaxis.Max);
                yaxis = TempAxis;
            }
            this.channels.Add((yValues, yaxis, NextPaint()));
        }
        var min = data.SpeedChannel.Points.First().X;
        var max = data.SpeedChannel.Points.Last().X;
        this.XAxis = new Axis() { Min = (float)min, Max = (float)max };
        this.XDataRange = new Axis() { Min = (float)min, Max = (float)max };
    }

    public void Draw(SKCanvas canvas, SKImageInfo imageInfo)
    {
        canvas.Clear(SKColors.Black);

        foreach (var channel in this.channels)
        {
            FillPath2(channel.yvalues, this.XDataRange, channel.YAxis, imageInfo);
            canvas.DrawPath(path, channel.Paint);
            //RenderHighDensity(channel.yvalues, this.XDataRange, channel.YAxis, canvas, channel.Paint, imageInfo);
        }
    }

    private void RenderHighDensity(float[] yValues, Axis xDataRange, Axis yAxis, SKCanvas canvas, SKPaint paint, SKImageInfo imageInfo)
    {
        PixelRange[] verticalBars = GetVerticalBars(yValues, xDataRange, yAxis, imageInfo.Width, imageInfo.Height);
        for (int i = 0; i < verticalBars.Length; i++)
        {
            float x = i + 0.5f;
            canvas.DrawLine(x, verticalBars[i].Bottom, x, verticalBars[i].Top, paint);
        }
    }

    private PixelRange[] GetVerticalBars(float[] yValues, Axis xDataRange, Axis yAxis, int imageWidth, int imageHeight)
    {
        var m = imageHeight / (yAxis.Min - yAxis.Max);
        var bars = new PixelRange[imageWidth];
        var unitsPerPixel = this.XAxis.Length / imageWidth;
        var unitsPerIndex = xDataRange.Length / (yValues.Length - 1);
        var indexOffset = (int)((xDataRange.Min - XAxis.Min) / unitsPerPixel);
        for (var i = 0; i < (imageWidth - 1 - indexOffset); i++)
        {
            var xLeft = i * unitsPerPixel;
            var xRight = xLeft + unitsPerPixel;
            var leftIndex = (int)(xLeft / unitsPerIndex);
            var rightIndex = (int)(xRight / unitsPerIndex) + 1;
            var min = float.MaxValue;
            var max = float.MinValue;
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

    private struct PixelRange
    {
        public float Top { get; private set; }
        public float Bottom { get; private set; }
        public float Span => Bottom - Top;

        public PixelRange(float y1, float y2)
        {
            Top = y1;
            Bottom = y2;
            var tol = 1.01f;
            if (Span < tol)
            {
                Top -= tol / 2;
                Bottom += tol / 2;
            }
        }
    }

    private SKPath path = new SKPath();

    // Axis xDataRange
    private void FillPath(float[] yValues, Axis xDataRange, Axis yAxis, SKImageInfo imageInfo)
    {
        path.Reset();
        var m = imageInfo.Height / (yAxis.Min - yAxis.Max);

        var imageWidth = imageInfo.Width;
        var unitsPerPixel = this.XAxis.Length / imageWidth;
        var unitsPerIndex = xDataRange.Length / (yValues.Length - 1);
        var offset = (xDataRange.Min - XAxis.Min) / unitsPerPixel;
        var indexOffset = (int)((xDataRange.Min - XAxis.Min) / unitsPerPixel);
        for (var i = 0; i < (imageWidth - 1 - indexOffset); i++)
        {
            var xLeft = i * unitsPerPixel;
            var xRight = xLeft + unitsPerPixel;
            var leftIndex = (int)(xLeft / unitsPerIndex);
            var rightIndex = (int)(xRight / unitsPerIndex) + 1;
            var min = float.MaxValue;
            var max = float.MinValue;
            if (rightIndex >= yValues.Length)
                break;
            var start = (yValues[leftIndex] - yAxis.Max) * m;
            var end = (yValues[rightIndex] - yAxis.Max) * m;
            for (int j = leftIndex + 1; j < rightIndex - 1; j++)
            {
                var y = (yValues[j] - yAxis.Max) * m;
                if (min > y)
                    min = y;
                if (max < y)
                    max = y;
            }
            if (i + indexOffset >= imageWidth)
                break;

            path.LineTo(i + offset, start);
            path.LineTo(i + offset + 0.25f, min);
            path.LineTo(i + offset + 0.75f, max);
            path.LineTo(i + offset + 1f, end);
        }
    }

    private void FillPath2(float[] yValues, Axis xDataRange, Axis yAxis, SKImageInfo imageInfo)
    {
        path.Reset();
        var m = imageInfo.Height / (yAxis.Min - yAxis.Max);
        var imageWidth = imageInfo.Width;
        var unitsPerPixel = this.XAxis.Length / imageWidth;
        var dp = imageInfo.Width / (float)(yValues.Length - 1);
        var lastx = 0.0f;
        var offset = (xDataRange.Min - XAxis.Min) / unitsPerPixel;
        var lasty = (yValues[0] - yAxis.Max) * m;
        path.MoveTo(lastx, lasty);
        for (int i = 0; i < yValues.Length - 1; i++)
        {
            var newy = (yValues[i] - yAxis.Max) * m;
            path.LineTo(i * dp + offset, newy);
        }
    }

    private void DrawSignal(float[] channel, Axis yAxis, SKPaint paint, SKCanvas canvas, SKImageInfo imageInfo)
    {
        var m = imageInfo.Height / (yAxis.Min - yAxis.Max);
        var dp = imageInfo.Width / (float)(channel.Length - 1);
        var lastx = 0.0f;

        var lasty = (channel[0] - yAxis.Max) * m;

        for (int i = 0; i < channel.Length - 1; i++)
        {
            var newX = i * dp;
            var newy = (channel[i] - yAxis.Max) * m;
            canvas.DrawLine(lastx, lasty, newX, newy, paint);
            lastx = newX;
            lasty = newy;
        }
    }

    public class Axis
    {
        public float Min { get; set; } = float.MaxValue;
        public float Max { get; set; } = float.MinValue;
        public float Length => Max - Min;
        public Axis Scale(float s) => new Axis() { Min = Min - (s - 1) * Length, Max = Max + (s - 1) * Length };
    }

    int currentPaintIndex = 0;
    List<SKPaint> paints = new List<SKPaint>();
    private SKPaint NextPaint() => paints[currentPaintIndex++ % paints.Count];

    private SKPaint paint1 = new SKPaint()
    {
        Color = SKColors.Orange,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint2 = new SKPaint()
    {
        Color = SKColors.Blue,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint3 = new SKPaint()
    {
        Color = SKColors.Red,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint4 = new SKPaint()
    {
        Color = SKColors.Green,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint5 = new SKPaint()
    {
        Color = SKColors.Orange,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

}

