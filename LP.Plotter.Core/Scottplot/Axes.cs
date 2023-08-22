using OxyPlot;
using SkiaSharp;
using System.Drawing;
using System.Reflection.Emit;

namespace LP.Plotter.Core.Scottplot;

public enum Edge
{
    Left,
    Right,
    Bottom,
    Top
}

public struct Tick
{
    public readonly double Position;
    public readonly string Label;
    public readonly bool IsMajor;

    public Tick(double position, string label, bool isMajor)
    {
        Position = position;
        Label = label;
        IsMajor = isMajor;
    }

    public static Tick Major(double position, string label)
    {
        return new Tick(position, label, true);
    }

    public static Tick Minor(double position)
    {
        return new Tick(position, string.Empty, false);
    }
}

public readonly struct PixelLength
{
    public readonly float Length;

    public PixelLength(float length)
    {
        Length = length;
    }

    public static implicit operator PixelLength(float length)
    {
        return new PixelLength(length);
    }

    public override string ToString()
    {
        return $"{Length} pixels";
    }
}

public interface ITickGenerator
{
    /// <summary>
    /// Ticks to display the next time the axis is rendered
    /// </summary>
    Tick[] Ticks { get; set; }

    /// <summary>
    /// Do not automatically generate more ticks than this
    /// </summary>
    int MaxTickCount { get; set; }

    /// <summary>
    /// Logic for generating ticks automatically.
    /// Generated ticks are stored in <see cref="Ticks"/>.
    /// </summary>
    void Regenerate(CoordinateRange range, Edge edge, PixelLength size);
}

public interface IPanel
{
    /// <summary>
    /// If false, the panel will not be displayed or report any size
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Return the size (in pixels) of the panel in the dimension perpendicular to the edge it lays on
    /// </summary>
    /// <returns></returns>
    float Measure();

    /// <summary>
    /// Indicates which edge of the data rectangle this panel lays on
    /// </summary>
    Edge Edge { get; }

    /// <summary>
    /// Draw this panel on a canvas
    /// </summary>
    /// <param name="surface">contains the canvas to draw on</param>
    /// <param name="dataRect">dimensions of the data area (pixel units)</param>
    /// <param name="size">size of this panel (pixel units)</param>
    /// <param name="offset">distance from the edge of this panel to the edge of the data area</param>
    void Render(SKCanvas canvas, float size, float offset);

    /// <summary>
    /// Enable this to display extra information on the axis to facilitate development
    /// </summary>
    bool ShowDebugInformation { get; set; }

    /// <summary>
    /// Return the rectangle for this panel
    /// </summary>
    PixelRect GetPanelRect(PixelRect dataRect, float size, float offset);
}

public interface IAxis : IPanel
{
    public double Length { get; set; }
    public double Width => Length;
    public double Height => Length;

    /// <summary>
    /// Min/Max range currently displayed by this axis
    /// </summary>
    CoordinateRange Range { get; }

    double Min { get; set; }
    double Max { get; set; }

    /// <summary>
    /// Get the pixel position of a coordinate given the location and size of the data area
    /// </summary>
    float GetPixel(double position, PixelRect dataArea);

    /// <summary>
    /// Get the coordinate of a pixel position given the location and size of the data area
    /// </summary>
    double GetCoordinate(float pixel, PixelRect dataArea);

    /// <summary>
    /// Given a distance in coordinate space, converts to pixel space
    /// </summary>
    /// <param name="coordinateDistance">A distance in coordinate units</param>
    /// <param name="dataArea">The rectangle onto which the coordinates are mapped</param>
    /// <returns>The same distance in pixel units</returns>
    double GetPixelDistance(double coordinateDistance, PixelRect dataArea);

    /// <summary>
    /// Given a distance in pixel space, converts to coordinate space
    /// </summary>
    /// <param name="coordinateDistance">A distance in pixel units</param>
    /// <param name="dataArea">The rectangle onto which the coordinates are mapped</param>
    /// <returns>The same distance in coordinate units</returns>
    double GetCoordinateDistance(double pixelDistance, PixelRect dataArea);

    /// <summary>
    /// Logic for determining tick positions and formatting tick labels
    /// </summary>
    ITickGenerator TickGenerator { get; set; }

    /// <summary>
    /// The label is the text displayed distal to the ticks
    /// </summary>
    Label Label { get; }
    float MajorTickLength { get; set; }
    float MajorTickWidth { get; set; }
    Color MajorTickColor { get; set; }
    float MinorTickLength { get; set; }
    float MinorTickWidth { get; set; }
    Color MinorTickColor { get; set; }
    SKFont TickFont { get; }
    LineStyle FrameLineStyle { get; }
}

public interface IAxes
{
    PixelRect DataRect { get; set; }

    // Note: Axes (not just the translation logic) are here so ticks are accessible to plottables.
    // Note: At render time any null axes will be set to the default axes for the plot
    IAxis XAxis { get; set; }
    IAxis YAxis { get; set; }

    PixelRect GetPixelRect(CoordinateRect rect);

    Pixel GetPixel(Coordinates coordinates);
    float GetPixelX(double xCoordinate);
    float GetPixelY(double yCoordinate);

    Coordinates GetCoordinates(Pixel pixel);
    double GetCoordinateX(float pixel);
    double GetCoordinateY(float pixel);
}

public class Axes : IAxes
{
    public IAxis XAxis { get; set; } = null!;
    public IAxis YAxis { get; set; } = null!;
    public PixelRect DataRect { get; set; }

    public Coordinates GetCoordinates(Pixel pixel)
    {
        double x = XAxis.GetCoordinate(pixel.X, DataRect);
        double y = YAxis.GetCoordinate(pixel.Y, DataRect);
        return new Coordinates(x, y);
    }

    public double GetCoordinateX(float pixel) => XAxis.GetCoordinate(pixel, DataRect);

    public double GetCoordinateY(float pixel) => YAxis.GetCoordinate(pixel, DataRect);

    public Pixel GetPixel(Coordinates coordinates)
    {
        float x = XAxis.GetPixel(coordinates.X, DataRect);
        float y = YAxis.GetPixel(coordinates.Y, DataRect);
        return new Pixel(x, y);
    }

    public float GetPixelX(double xCoordinate) => XAxis.GetPixel(xCoordinate, DataRect);

    public float GetPixelY(double yCoordinate) => YAxis.GetPixel(yCoordinate, DataRect);

    public PixelRect GetPixelRect(CoordinateRect rect)
    {
        return new PixelRect(
            left: GetPixelX(rect.Left),
            right: GetPixelX(rect.Right),
            bottom: GetPixelY(rect.Bottom),
            top: GetPixelY(rect.Top));
    }
}