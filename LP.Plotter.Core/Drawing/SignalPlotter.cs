using LP.Plotter.Core.Models;
using SkiaSharp;

namespace LP.Plotter.Core.Drawing;

public class SignalPlotter
{
    private readonly VChannelSet data;
    private List<(float[] yvalues, Axis YAxis, SKPaint Paint)> channels;
    private Axis XAxis;

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
            this.channels.Add((yValues, yaxis, NextPaint()));
        }
        var min = data.SpeedChannel.Points.First().X;
        var max = data.SpeedChannel.Points.Last().X;
        this.XAxis = new Axis() { Min = (float)min, Max = (float)max };
    }

    public void Draw(SKCanvas canvas, SKImageInfo imageInfo)
    {
        canvas.Clear(SKColors.Black);

        foreach (var channel in this.channels)
        {
            FillPath(channel.yvalues, channel.YAxis, channel.Paint, imageInfo);
            canvas.DrawPath(path, channel.Paint);
        }
    }
    private SKPath path = new SKPath();
    private void FillPath(float[] channel, Axis yAxis, SKPaint paint, SKImageInfo imageInfo)
    {
        path.Reset();
        var m = imageInfo.Height / (yAxis.Min - yAxis.Max);
        var dp = imageInfo.Width / (float)(channel.Length - 1);
        var lastx = 0.0f;

        var lasty = (channel[0] - yAxis.Max) * m;
        path.MoveTo(lastx, lasty);
        for (int i = 0; i < channel.Length - 1; i++)
        {
            var newy = (channel[i] - yAxis.Max) * m;
            path.LineTo(i * dp, newy);
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

    private class Axis
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public float Length => Max - Min;
        public Axis Scale(float s) => new Axis() { Min = Min - (s - 1) * Length, Max = Max + (s - 1) * Length };
    }

    int currentPaintIndex = 0;
    List<SKPaint> paints = new List<SKPaint>();
    private SKPaint NextPaint() => paints[currentPaintIndex++ % paints.Count];

    private SKPaint paint1 = new SKPaint()
    {
        Color = SKColors.Orange,
        StrokeWidth = 3,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint2 = new SKPaint()
    {
        Color = SKColors.Blue,
        StrokeWidth = 3,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint3 = new SKPaint()
    {
        Color = SKColors.Red,
        StrokeWidth = 3,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint4 = new SKPaint()
    {
        Color = SKColors.Green,
        StrokeWidth = 3,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    private SKPaint paint5 = new SKPaint()
    {
        Color = SKColors.Orange,
        StrokeWidth = 3,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

}

