using MudBlazor.Utilities;
using SkiaSharp;

namespace LP.Plot.Utilities;

public static class ColorExtensions
{
    public static SKColor ToSKColor(this MudColor color) => new SKColor(color.R, color.G, color.B, color.A);
    public static MudColor ToMudColor(this SKColor color) => new MudColor(color.Red, color.Green, color.Blue, color.Alpha);
}
