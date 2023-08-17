using MudBlazor.Utilities;
using OxyPlot;
using System.IO;

namespace LP.Plotter.Utilities;

public static class ColorExtensions
{
    public static OxyColor ToOxyColor(this MudColor color) => OxyColor.FromArgb(color.A, color.R, color.G, color.B);
    public static MudColor ToMudColor(this OxyColor color) => new MudColor(color.R, color.G, color.B, color.A);


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

public class CustomIcons
{
    public const string ZoomInHorizontal = @"<g><g><rect fill=""none"" height=""24"" width=""24"" /></g><g><g><g><path d=""M 3,9 5.3,6.7 8.17,9.59 9.59,8.17 6.7,5.3 9,3 H 3 Z"" transform=""translate(14,0) rotate(-45, 12, 12)""/><path d=""M 3,9 5.3,6.7 8.17,9.59 9.59,8.17 6.7,5.3 9,3 H 3 Z"" transform=""translate(-14,0) rotate(135, 12, 12)"" /></g></g></g></g>";
    public const string ZoomOutHorizontal = @"<g><g><rect fill=""none"" height=""24"" width=""24"" /></g><g><g><g><path d=""M 3,9 5.3,6.7 8.17,9.59 9.59,8.17 6.7,5.3 9,3 H 3 Z"" transform=""translate(3,0) rotate(-45, 12, 12)""/><path d=""M 3,9 5.3,6.7 8.17,9.59 9.59,8.17 6.7,5.3 9,3 H 3 Z"" transform=""translate(-3,0) rotate(135, 12, 12)"" /></g></g></g></g>";
}