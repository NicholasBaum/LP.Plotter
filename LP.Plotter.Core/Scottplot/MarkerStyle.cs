using SkiaSharp;

namespace LP.Plotter.Core.Scottplot;

/// <summary>
/// This configuration object (reference type) permanently lives inside objects which require styling.
/// It is recommended to use this object as an init-only property.
/// </summary>
public class MarkerStyle
{
    public bool IsVisible { get; set; }

    public MarkerShape Shape { get; set; }

    /// <summary>
    /// Diameter of the marker (in pixels)
    /// </summary>
    public float Size { get; set; }

    public IMarker MarkerRenderer { get; set; }

    public FillStyle Fill { get; set; } = new() { Color = SKColors.Gray };

    public MarkerStyle() : this(MarkerShape.FilledCircle, 5, SKColors.Gray)
    {
    }

    public MarkerStyle(MarkerShape shape, float size) : this(shape, size, SKColors.Gray)
    {
    }

    public MarkerStyle(MarkerShape shape, float size, SKColor color)
    {
        Shape = shape;
        MarkerRenderer = shape.GetRenderer();
        IsVisible = shape != MarkerShape.None;
        Fill.Color = color;
        Size = size;
    }

    public static MarkerStyle Default => new(MarkerShape.FilledCircle, 5);

    public static MarkerStyle None => new(MarkerShape.None, 0);

    public void Render(SKCanvas canvas, Pixel pixel)
    {
        Render(canvas, new[] { pixel });
    }

    public void Render(SKCanvas canvas, IEnumerable<Pixel> pixels)
    {
        if (!IsVisible)
            return;

        using SKPaint paint = new() { IsAntialias = true };

        foreach (Pixel pixel in pixels)
        {
            MarkerRenderer.Render(canvas, paint, pixel, Size, Fill);
        }
    }
}

public enum MarkerShape
{
    None,
    FilledCircle,
    OpenCircle,
    FilledSquare,
    OpenSquare,
}

public static class MarkerShapeExtensions
{
    public static IMarker GetRenderer(this MarkerShape shape)
    {
        return shape switch
        {
            MarkerShape.FilledCircle or MarkerShape.OpenCircle or MarkerShape.None => new Circle(),
            MarkerShape.FilledSquare or MarkerShape.OpenSquare => new Square(),
            _ => throw new NotImplementedException(shape.ToString()),
        };
    }

    public static bool IsOutlined(this MarkerShape shape)
    {
        return shape switch
        {
            MarkerShape.OpenCircle or MarkerShape.OpenSquare => true,
            _ => false,
        };
    }
}

public class FillStyle
{
    public SKColor Color { get; set; } = SKColors.Black;
    public SKColor HatchColor { get; set; } = SKColors.Gray;
    //public IHatch? Hatch { get; set; } = null;
    public bool HasValue => Color != SKColors.Transparent;// || Hatch is not null && HatchColor != Colors.Transparent;
}

internal class Circle : IMarker
{
    public void Render(SKCanvas canvas, SKPaint paint, Pixel center, float size, FillStyle fill)
    {
        float radius = size / 2;
        canvas.DrawCircle(center.ToSKPoint(), radius, paint);
    }
}


internal class Square : IMarker
{
    public void Render(SKCanvas canvas, SKPaint paint, Pixel center, float size, FillStyle fill)
    {
        PixelRect rect = new(center: center, radius: size / 2);
        canvas.DrawRect(rect.ToSKRect(), paint);
    }
}


public interface IMarker
{
    void Render(SKCanvas canvas, SKPaint paint, Pixel center, float size, FillStyle fill);
}
