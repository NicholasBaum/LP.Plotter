using LP.Plot.Core.Primitives;
using System.ComponentModel.DataAnnotations;

namespace LP.Plot.Core;

public class Axis
{
    public Axis() { }
    public Axis(Span range) => (Min, Max) = (range.Min, range.Max);
    public Span Range => new Span(Min, Max);
    public double Min { get; set; } = float.MaxValue;
    public double Max { get; set; } = float.MinValue;
    public double Length => Max - Min;
    public Axis Scale(double s) => new Axis() { Min = Min - (s - 1) / 2 * Length, Max = Max + (s - 1) / 2 * Length };
}