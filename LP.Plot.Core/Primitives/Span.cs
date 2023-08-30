namespace LP.Plot.Core.Primitives;

public readonly struct Span
{
    public double Min { get; }
    public double Max { get; }

    public double Length => Max - Min;

    public Span(double min, double max)
    {
        Min = min;
        Max = max;
    }

    public double Mid => (Min + Max) / 2;
    public bool Contains(Span other)
        => Min <= other.Min && other.Max <= Max;

    public override string ToString()
    {
        return $"[Min: {Min}, Max: {Max}, Length: {Length}]";
    }

    public Span ScaleAtCenter(double factor)
    {
        var mid = Mid;
        var newLeftSide = (mid - Min) * factor;
        var newRightSide = (Max - mid) * factor;
        var min = mid - newLeftSide;
        var max = mid + newRightSide;
        return new Span(min, max);
    }
}
