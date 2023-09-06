using System.Runtime.CompilerServices;

namespace LP.Plot.Core.Primitives;

public record struct LPRect : IEquatable<LPRect>
{
    public static readonly LPRect Empty;

    public int Left
    {
        get => left;
        set => left = value;
    }

    public int Top
    {
        get => top;
        set => top = value;
    }

    public int Right
    {
        get => right;
        set => right = value;
    }

    public int Bottom
    {
        get => bottom;
        set => bottom = value;
    }

    private int left;
    private int top;
    private int right;
    private int bottom;

    public readonly int MidX => left + Width / 2;

    public readonly int MidY => top + Height / 2;

    public readonly int Width => right - left;

    public readonly int Height => bottom - top;

    public readonly bool IsEmpty => Width == 0 || Height == 0;

    public LPSize Size => new LPSize(Width, Height);



    public LPRect(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }

    public static LPRect Inflate(LPRect rect, int x, int y)
    {
        LPRect result = new LPRect(rect.left, rect.top, rect.right, rect.bottom);
        result.Inflate(x, y);
        return result;
    }

    public void Inflate(LPSize size)
    {
        Inflate(size.Width, size.Height);
    }

    public void Inflate(int width, int height)
    {
        left -= width;
        top -= height;
        right += width;
        bottom += height;
    }

    public static LPRect Intersect(LPRect a, LPRect b)
    {
        if (!a.IntersectsWithInclusive(b))
        {
            return Empty;
        }

        return new LPRect(Math.Max(a.left, b.left), Math.Max(a.top, b.top), Math.Min(a.right, b.right), Math.Min(a.bottom, b.bottom));
    }

    public void Intersect(LPRect rect)
    {
        this = Intersect(this, rect);
    }

    public static LPRect Union(LPRect a, LPRect b)
    {
        return new LPRect(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
    }

    public void Union(LPRect rect)
    {
        this = Union(this, rect);
    }

    public readonly bool Contains(int x, int y)
    {
        if (x >= left && x < right && y >= top)
        {
            return y < bottom;
        }

        return false;
    }

    public readonly bool Contains(LPRect rect)
    {
        if (left <= rect.left && right >= rect.right && top <= rect.top)
        {
            return bottom >= rect.bottom;
        }

        return false;
    }

    public readonly bool IntersectsWith(LPRect rect)
    {
        if (left < rect.right && right > rect.left && top < rect.bottom)
        {
            return bottom > rect.top;
        }

        return false;
    }

    public readonly bool IntersectsWithInclusive(LPRect rect)
    {
        if (left <= rect.right && right >= rect.left && top <= rect.bottom)
        {
            return bottom >= rect.top;
        }

        return false;
    }

    public void Offset(int x, int y)
    {
        left += x;
        top += y;
        right += x;
        bottom += y;
    }

    public override readonly string ToString()
    {
        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 4);
        defaultInterpolatedStringHandler.AppendLiteral("{Left=");
        defaultInterpolatedStringHandler.AppendFormatted(Left);
        defaultInterpolatedStringHandler.AppendLiteral(",Top=");
        defaultInterpolatedStringHandler.AppendFormatted(Top);
        defaultInterpolatedStringHandler.AppendLiteral(",Width=");
        defaultInterpolatedStringHandler.AppendFormatted(Width);
        defaultInterpolatedStringHandler.AppendLiteral(",Height=");
        defaultInterpolatedStringHandler.AppendFormatted(Height);
        defaultInterpolatedStringHandler.AppendLiteral("}");
        return defaultInterpolatedStringHandler.ToStringAndClear();
    }

    public static LPRect Create(LPSize size)
    {
        return Create(0, 0, size.Width, size.Height);
    }

    public static LPRect Create(int width, int height)
    {
        return new LPRect(0, 0, width, height);
    }

    public static LPRect Create(int x, int y, int width, int height)
    {
        return new LPRect(x, y, x + width, y + height);
    }

    internal SkiaSharp.SKRect ToSkia()
    {
        return new SkiaSharp.SKRect(left, top, right, bottom);
    }
}
