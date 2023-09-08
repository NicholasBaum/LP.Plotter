namespace LP.Plot.Primitives;

public readonly record struct DPoint
{
    public DPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; init; }
    public double Y { get; init; }
}
