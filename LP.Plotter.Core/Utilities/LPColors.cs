using OxyPlot;
using SkiaSharp;

namespace LP.Plotter.Core.Utilities;

public static class SKPaints
{
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
        StrokeWidth = 1,
        IsAntialias = true,
    };
    public static SKPaint Orange = new SKPaint()
    {
        Color = SKColors.Orange,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Blue = new SKPaint()
    {
        Color = SKColors.Blue,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Red = new SKPaint()
    {
        Color = SKColors.Red,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Green = new SKPaint()
    {
        Color = SKColors.Green,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };

    public static SKPaint Yellow = new SKPaint()
    {
        Color = SKColors.Yellow,
        StrokeWidth = 1,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };
}

public static class LPColors
{
    private static int currentColorIndex;

    private static IList<OxyColor> DefaultColors = new List<OxyColor>
                                {
                                    OxyColor.FromRgb(0x4E, 0x9A, 0x06),
                                    OxyColor.FromRgb(0xC8, 0x8D, 0x00),
                                    OxyColor.FromRgb(0xCC, 0x00, 0x00),
                                    OxyColor.FromRgb(0x20, 0x4A, 0x87),
                                    OxyColors.Red,
                                    OxyColors.Orange,
                                    OxyColors.Yellow,
                                    OxyColors.Green,
                                    OxyColors.Blue,
                                    OxyColors.Indigo,
                                    OxyColors.Violet,
                                    OxyColor.FromRgb(0xFF, 0xC0, 0xCB), // Pink
                                    OxyColor.FromRgb(0xFF, 0x69, 0xB4), // Hot Pink
                                    OxyColor.FromRgb(0xFF, 0xA0, 0x7A), // Light Salmon
                                    OxyColor.FromRgb(0xFF, 0xD7, 0x00), // Gold
                                    OxyColor.FromRgb(0x8A, 0x2B, 0xE2), // Blue Violet
                                    OxyColor.FromRgb(0x40, 0xE0, 0xD0), // Turquoise
                                    OxyColor.FromRgb(0xFF, 0x8C, 0x00), // Dark Orange
                                    OxyColor.FromRgb(0x00, 0x64, 0x00), // Dark Green
                                    OxyColor.FromRgb(0xFF, 0x45, 0x00), // Red-Orange
                                    OxyColor.FromRgb(0x1E, 0x90, 0xFF), // Dodger Blue
                                    OxyColor.FromRgb(0x8B, 0x00, 0x8B)  // Dark Magenta
                                };

    public static OxyColor GetNextColor()
    {
        return DefaultColors[currentColorIndex++ % DefaultColors.Count];
    }
}
