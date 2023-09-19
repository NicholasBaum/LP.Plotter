using LP.Plot.Primitives;
using LP.Plot.Signal;
using LP.Plot.Skia;

namespace LP.Plot.Core.Data;

public class SciChartDemo
{
    static Random rnd = new(1337);

    public static List<StaticSignal> LoadPoints(int SERIES = 500, int POINTS = 500)
    {
        var xValuesArray = new List<double[]>(SERIES);
        var yValuesArray = new List<double[]>(SERIES);
        for (var i = 0; i < SERIES; i++)
        {
            xValuesArray.Add(new double[POINTS]);
            yValuesArray.Add(new double[POINTS]);

            var prevYValue = 0.0;
            for (var j = 0; j < POINTS; j++)
            {
                var curYValue = rnd.NextDouble() * 10 - 5;

                xValuesArray[i][j] = j;
                yValuesArray[i][j] = prevYValue + curYValue;

                prevYValue += curYValue;
            }
        }
        var min = yValuesArray.Min(x => x.Min());
        var max = yValuesArray.Max(x => x.Max());
        var yrange = new Span(min, max).ScaleAtCenter(1.1);
        var signals = yValuesArray.Select(x => new StaticSignal(x, new(0, POINTS), new Axis(yrange), SKPaints.NextPaint(), "500x500")).ToList();
        return signals;
    }
}
