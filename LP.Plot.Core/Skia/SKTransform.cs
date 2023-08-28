using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Skia;

public class SKSpaceTransform
{
    LinearTransform xTransform;
    LinearTransform yTransform;

    public SKSpaceTransform(LPSize imageSize, Span xRange, Span yRange) : this(imageSize.Width, imageSize.Height, xRange, yRange) { }

    public SKSpaceTransform(int imageWidth, int imageHeight, Span xRange, Span yRange)
    {
        this.xTransform = new LinearTransform(xRange, imageWidth);
        this.yTransform = new LinearTransform(yRange.Max, yRange.Min, imageHeight);// swap min/max because 0 is top in skia canvas
    }

    public float ToPixelSpaceX(double x) => xTransform.ToPixelSpace(x);
    public float ToPixelSpaceY(double y) => yTransform.ToPixelSpace(y);
    public double ToDataSpaceX(double x) => xTransform.ToDataSpace(x);
    public double ToDataSpaceY(double y) => yTransform.ToDataSpace(y);
}
