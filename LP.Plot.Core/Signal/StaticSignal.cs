using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Signal;

public class StaticSignal : ISignalSource
{
    public double[] YValues { get; }
    public double Period { get; }
    public Span YRange { get; }
    public Span XRange { get; }

    public StaticSignal(double[] yValues, Span xRange)
    {
        this.YValues = yValues;
        XRange = xRange;
        YRange = new Span(yValues.Min(), yValues.Max());
        Period = XRange.Length / yValues.Length;
    }

    //public Span GetYRange(Span xRange)
    //{
    //    throw new NotImplementedException();
    //    //int i1 = GetIndex(xRange.Min, true);
    //    //int i2 = GetIndex(xRange.Max, true);

    //    //Span yRange = new(Ys[i1], Ys[i1]);

    //    //for (int i = i1; i <= i2; i++)
    //    //{
    //    //    yRange.Expand(Ys[i]);
    //    //}

    //    //if (YOffset != 0)
    //    //    yRange.Pan(YOffset);

    //    //return yRange;
    //}

    //private int GetIndex(double x, bool clamp)
    //{
    //    throw new NotImplementedException();
    //    //int i = (int)((x - XOffset) / Period);
    //    //if (clamp)
    //    //{
    //    //    i = Math.Max(i, 0);
    //    //    i = Math.Min(i, Ys.Count - 1);
    //    //}
    //    //return i;
    //}

    //public double GetX(int index)
    //{
    //    return (index * Period) + XOffset;
    //}     
}
