using SkiaSharp;

namespace LP.Plotter.Core.Drawing;

public class ArtDrawer
{
    private Random rand = new Random();

    public void DrawArtisticLines(SKCanvas canvas, SKImageInfo imageInfo)
    {
        // make sure the canvas is blank
        canvas.Clear(SKColors.White);

        // draw some text
        using var black = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
        };
        using var white = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Left,
            TextSize = 20,
        };
        using var font = new SKFont
        {
            Size = 24
        };
        canvas.Clear(SKColor.Parse("#003366"));

        int lineCount = 1000;
        for (int i = 0; i < lineCount; i++)
        {
            float lineWidth = rand.Next(1, 10);
            var lineColor = new SKColor(
                    red: (byte)rand.Next(255),
                    green: (byte)rand.Next(255),
                    blue: (byte)rand.Next(255),
                    alpha: (byte)rand.Next(255));

            var linePaint = new SKPaint
            {
                Color = lineColor,
                StrokeWidth = lineWidth,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            int x1 = rand.Next(imageInfo.Width);
            int y1 = rand.Next(imageInfo.Height);
            int x2 = rand.Next(imageInfo.Width);
            int y2 = rand.Next(imageInfo.Height);
            canvas.DrawLine(x1, y1, x2, y2, linePaint);
        }
    }
}
