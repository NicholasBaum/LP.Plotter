namespace LP.Plot.Core.Primitives;

internal readonly struct PixelRange
{
    public double Min { get; }
    public double Max { get; }

    public double Span => Max - Min;

    public PixelRange(double y1, double y2)
    {
        Max = y1;
        Min = y2;
        var tol = 1.01f;
        if (Span < tol)
        {
            Max -= tol / 2;
            Min += tol / 2;
        }
    }
}