using SkiaSharp;

namespace LP.Plot.Skia;

public static class SKPaints
{
    private static float StrokeWidth = 2;
    static SKPaints()
    {
        paints.Add(Orange);
        paints.Add(Green);
        paints.Add(Blue);
        paints.Add(Red);
        paints.Add(Yellow);
    }

    private static int currentPaintIndex = 0;
    private static List<SKPaint> paints = new List<SKPaint>();

    public static SKPaint NextPaint() => paints[currentPaintIndex++ % paints.Count];

    public static SKPaint Black { get; } = new SKPaint()
    {
        Color = SKColors.Black,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
    };
    public static SKPaint Orange = new SKPaint()
    {
        Color = SKColors.Orange,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Blue = new SKPaint()
    {
        Color = SKColors.Blue,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Red = new SKPaint()
    {
        Color = SKColors.Red,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Green = new SKPaint()
    {
        Color = SKColors.Green,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Yellow = new SKPaint()
    {
        Color = SKColors.Yellow,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint White = new SKPaint()
    {
        Color = SKColors.White,
        StrokeWidth = StrokeWidth,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };
}