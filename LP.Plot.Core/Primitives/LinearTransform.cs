namespace LP.Plot.Core.Primitives;

public class LinearTransform
{
    private readonly int pixelLength;
    private readonly double dataMin;
    private readonly double dataMax;
    private readonly double m;

    public LinearTransform(double min, double max, int pixelLength)
    {
        this.dataMin = min;
        this.dataMax = max;
        this.pixelLength = pixelLength;
        m = pixelLength / (max - min);
    }

    public LinearTransform(Span dataRange, int pixelLength) : this(dataRange.Min, dataRange.Max, pixelLength) { }

    public float ToPixelSpace(double x) => (float)((x - dataMin) * m);
    public double ToDataSpace(double x) => x / m + dataMin;
}