using LP.Plot.Primitives;

namespace LP.Plot.Skia;

public class SKTransform
{
    LinarTransform xTransform;
    LinarTransform yTransform;

    public SKTransform(LPSize imageSize, Span xRange, Span yRange) : this(imageSize.Width, imageSize.Height, xRange, yRange) { }

    public SKTransform(int imageWidth, int imageHeight, Span xRange, Span yRange)
    {
        this.xTransform = new LinarTransform(xRange, 0, imageWidth);
        this.yTransform = new LinarTransform(yRange.Max, yRange.Min, 0, imageHeight);// swap min/max because 0 is top in skia canvas
    }

    public float ToScreenSpaceX(double x) => (float)xTransform.Transform(x);
    public float ToScreenSpaceY(double y) => (float)yTransform.Transform(y);
    public double ToDataSpaceX(double x) => xTransform.Inverse(x);
    public double ToDataSpaceY(double y) => yTransform.Inverse(y);
}
