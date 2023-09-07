using System.Runtime.CompilerServices;

namespace LP.Plot.Core.Primitives;

public record struct LPRect : IEquatable<LPRect>
{
    public static readonly LPRect Empty;

    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }

    public readonly int MidX => Left + Width / 2;

    public readonly int MidY => Top + Height / 2;

    public readonly int Width => Right - Left;

    public readonly int Height => Bottom - Top;

    public readonly bool IsEmpty => Width == 0 || Height == 0;

    public LPSize Size => new LPSize(Width, Height);



    public LPRect(int left, int top, int right, int bottom)
    {
        this.Left = left;
        this.Right = right;
        this.Top = top;
        this.Bottom = bottom;
    }

    public static LPRect Inflate(LPRect rect, int x, int y)
    {
        LPRect result = new LPRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        result.Inflate(x, y);
        return result;
    }

    public void Inflate(LPSize size)
    {
        Inflate(size.Width, size.Height);
    }

    public void Inflate(int width, int height)
    {
        Left -= width;
        Top -= height;
        Right += width;
        Bottom += height;
    }

    public static LPRect Intersect(LPRect a, LPRect b)
    {
        if (!a.IntersectsWithInclusive(b))
        {
            return Empty;
        }

        return new LPRect(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
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
        if (x >= Left && x < Right && y >= Top)
        {
            return y < Bottom;
        }

        return false;
    }

    public readonly bool Contains(LPRect rect)
    {
        if (Left <= rect.Left && Right >= rect.Right && Top <= rect.Top)
        {
            return Bottom >= rect.Bottom;
        }

        return false;
    }

    public readonly bool IntersectsWith(LPRect rect)
    {
        if (Left < rect.Right && Right > rect.Left && Top < rect.Bottom)
        {
            return Bottom > rect.Top;
        }

        return false;
    }

    public readonly bool IntersectsWithInclusive(LPRect rect)
    {
        if (Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom)
        {
            return Bottom >= rect.Top;
        }

        return false;
    }

    public void Offset(int x, int y)
    {
        Left += x;
        Top += y;
        Right += x;
        Bottom += y;
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
        return new SkiaSharp.SKRect(Left, Top, Right, Bottom);
    }
}
