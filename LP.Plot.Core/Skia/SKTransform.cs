using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Skia;

public class SKSpaceTransform
{
    LPTransform xTransform;
    LPTransform yTransform;

    public SKSpaceTransform(LPSize imageSize, Span xRange, Span yRange) : this(imageSize.Width, imageSize.Height, xRange, yRange) { }

    public SKSpaceTransform(int imageWidth, int imageHeight, Span xRange, Span yRange)
    {
        this.xTransform = new LPTransform(xRange, 0, imageWidth);
        this.yTransform = new LPTransform(yRange.Max, yRange.Min, 0, imageHeight);// swap min/max because 0 is top in skia canvas
    }

    public float ToPixelSpaceX(double x) => (float)xTransform.Transform(x);
    public float ToPixelSpaceY(double y) => (float)yTransform.Transform(y);
    public double ToDataSpaceX(double x) => xTransform.Inverse(x);
    public double ToDataSpaceY(double y) => yTransform.Inverse(y);
}
