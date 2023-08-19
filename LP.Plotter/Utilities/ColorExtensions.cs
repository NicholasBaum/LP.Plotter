using MudBlazor.Utilities;
using OxyPlot;

namespace LP.Plotter.Utilities;

public static class ColorExtensions
{
    public static OxyColor ToOxyColor(this MudColor color) => OxyColor.FromArgb(color.A, color.R, color.G, color.B);
    public static MudColor ToMudColor(this OxyColor color) => new MudColor(color.R, color.G, color.B, color.A);
}
