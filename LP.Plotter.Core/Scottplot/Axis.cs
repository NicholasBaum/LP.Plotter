using OxyPlot;
using SkiaSharp;
using System.Drawing;
using System.Reflection.Emit;

namespace LP.Plotter.Core.Scottplot;

public class Axis : IAxis
{
    public double Length { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public CoordinateRange Range => throw new NotImplementedException();

    public double Min { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double Max { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public ITickGenerator TickGenerator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Label Label => throw new NotImplementedException();

    public float MajorTickLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float MajorTickWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Color MajorTickColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float MinorTickLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float MinorTickWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Color MinorTickColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public SKFont TickFont => throw new NotImplementedException();

    public LineStyle FrameLineStyle => throw new NotImplementedException();

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Edge Edge => throw new NotImplementedException();

    public bool ShowDebugInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public double GetCoordinate(float pixel, PixelRect dataArea)
    {
        double pxPerUnit = dataArea.Width / Length;
        float pxFromLeftEdge = pixel - dataArea.Left;
        double unitsFromEdge = pxFromLeftEdge / pxPerUnit;
        return Min + unitsFromEdge;
    }

    public double GetCoordinateDistance(double pixelDistance, PixelRect dataArea)
    {
        throw new NotImplementedException();
    }

    public PixelRect GetPanelRect(PixelRect dataRect, float size, float offset)
    {
        throw new NotImplementedException();
    }

    public float GetPixel(double position, PixelRect dataArea)
    {
        throw new NotImplementedException();
    }

    public double GetPixelDistance(double coordinateDistance, PixelRect dataArea)
    {
        throw new NotImplementedException();
    }

    public float Measure()
    {
        throw new NotImplementedException();
    }

    public void Render(SKCanvas canvas, float size, float offset)
    {
        throw new NotImplementedException();
    }
}
