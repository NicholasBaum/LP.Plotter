using LP.Plotter.Core.Utilities;
using SkiaSharp;

namespace LP.Plotter.Core.Scottplot;

public class SignalRenderer
{
    public bool IsVisible { get; set; } = true;
    public Axes Axes { get; set; } = new Axes();

    public readonly ISignalSource Data;

    public MarkerStyle Marker { get; set; } = new(MarkerShape.FilledCircle, 5);
    public string? Label { get; set; }
    public SKPaint Paint { get; set; } = SKPaints.Black;
    public SignalRenderer(ISignalSource data)
    {
        Data = data;
    }

    /// <summary>
    /// Return Y data limits for each pixel column in the data area
    /// </summary>
    private PixelRangeY[] GetVerticalBars()
    {
        PixelRangeY[] verticalBars = new PixelRangeY[(int)Axes.DataRect.Width];

        double xUnitsPerPixel = Axes.XAxis.Width / Axes.DataRect.Width;

        // for each vertical column of pixels in the data area
        for (int i = 0; i < verticalBars.Length; i++)
        {
            // determine how wide this column of pixels is in coordinate units
            float xPixel = i + Axes.DataRect.Left;
            double colX1 = Axes.GetCoordinateX(xPixel);
            double colX2 = colX1 + xUnitsPerPixel;
            CoordinateRange xRange = new(colX1, colX2);

            // determine how much vertical space the data of this pixel column occupies
            CoordinateRange yRange = Data.GetYRange(xRange);
            float yMin = Axes.GetPixelY(yRange.Min);
            float yMax = Axes.GetPixelY(yRange.Max);
            verticalBars[i] = new PixelRangeY(yMin, yMax);
        }

        return verticalBars;
    }

    private CoordinateRange GetVisibleXRange(PixelRect dataRect)
    {
        // TODO: put GetRange in axis translator
        double xViewLeft = Axes.GetCoordinateX(dataRect.Left);
        double xViewRight = Axes.GetCoordinateX(dataRect.Right);
        return new CoordinateRange(xViewLeft, xViewRight);
    }

    private double PointsPerPixel()
    {
        return GetVisibleXRange(Axes.DataRect).Span / Axes.DataRect.Width / Data.Period;
    }

    public void Render(SKCanvas canvas)
    {
        if (PointsPerPixel() < 1)
        {
            RenderLowDensity(canvas);
        }
        else
        {
            RenderHighDensity(canvas);
        }
    }

    /// <summary>
    /// Renders each point connected by a single line, like a scatter plot.
    /// Call this when zoomed in enough that no pixel could contain two points.
    /// </summary>
    private void RenderLowDensity(SKCanvas canvas)
    {
        CoordinateRange visibleXRange = GetVisibleXRange(Axes.DataRect);
        int i1 = Data.GetIndex(visibleXRange.Min, true);
        int i2 = Data.GetIndex(visibleXRange.Max + Data.Period, true);

        IReadOnlyList<double> Ys = Data.GetYs();

        List<Pixel> points = new();
        for (int i = i1; i <= i2; i++)
        {
            float x = Axes.GetPixelX(Data.GetX(i));
            float y = Axes.GetPixelY(Ys[i] + Data.YOffset);
            Pixel px = new(x, y);
            points.Add(px);
        }

        using SKPath path = new();
        path.MoveTo(points[0].ToSKPoint());
        foreach (Pixel point in points)
            path.LineTo(point.ToSKPoint());

        canvas.DrawPath(path, Paint);

        double pointsPerPx = PointsPerPixel();

        if (pointsPerPx < 1)
        {
            Paint.IsStroke = false;
            float radius = (float)Math.Min(Math.Sqrt(.2 / pointsPerPx), 4);
            Marker.Size = radius;
            Marker.Render(canvas, points);
        }
    }

    /// <summary>
    /// Renders the plot by filling-in pixel columns according the extremes of Y data ranges.
    /// Call this when zoomed out enough that one X pixel column may contain two or more points.
    /// </summary>
    private void RenderHighDensity(SKCanvas canvas)
    {
        PixelRangeY[] verticalBars = GetVerticalBars();
        for (int i = 0; i < verticalBars.Length; i++)
        {
            float x = Axes.DataRect.Left + i;
            canvas.DrawLine(x, verticalBars[i].Bottom, x, verticalBars[i].Top, Paint);
        }
    }
}
