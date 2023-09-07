namespace LP.Plot.Core.Primitives;

public class LPTransform
{
    private readonly double x0;
    private readonly double y0;
    private double mx;
    private double my;

    public LPTransform(double x0, double x1, double y0, double y1)
    {
        this.x0 = x0;
        this.y0 = y0;
        this.mx = (y1 - y0) / (x1 - x0);
        this.my = (x1 - x0) / (y1 - y0);
    }

    public LPTransform(Span p0, double y0, double y1) : this(p0.Min, p0.Max, y0, y1) { }
    public LPTransform(Span p0, Span p1) : this(p0.Min, p0.Max, p1.Min, p1.Max) { }

    public double Transform(double x)
    {
        return (x - x0) * mx + y0;
    }

    public double Inverse(double y)
    {
        return (y - y0) * my + x0;
    }
}