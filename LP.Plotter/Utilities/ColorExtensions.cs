using MudBlazor.Utilities;
using OxyPlot;
using SkiaSharp;

namespace LP.Plotter.Utilities;

public static class ColorExtensions
{
    public static OxyColor ToOxyColor(this MudColor color) => OxyColor.FromArgb(color.A, color.R, color.G, color.B);
    public static MudColor ToMudColor(this OxyColor color) => new MudColor(color.R, color.G, color.B, color.A);
    public static SKColor ToSKColor(this MudColor color) => new SKColor(color.A, color.R, color.G, color.B);
    public static MudColor ToMudColor(this SKColor color) => new MudColor(color.Red, color.Green, color.Blue, color.Alpha);
}
