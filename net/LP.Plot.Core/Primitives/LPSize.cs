using System.Runtime.CompilerServices;

namespace LP.Plot.Primitives;

public struct LPSize : IEquatable<LPSize>
{
    public static readonly LPSize Empty;

    public readonly bool IsEmpty => this == Empty;

    public int Width { get; private set; }

    public int Height { get; private set; }

    public LPSize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override readonly string ToString()
    {
        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
        defaultInterpolatedStringHandler.AppendLiteral("{Width=");
        defaultInterpolatedStringHandler.AppendFormatted(Width);
        defaultInterpolatedStringHandler.AppendLiteral(", Height=");
        defaultInterpolatedStringHandler.AppendFormatted(Height);
        defaultInterpolatedStringHandler.AppendLiteral("}");
        return defaultInterpolatedStringHandler.ToStringAndClear();
    }

    public static LPSize Add(LPSize sz1, LPSize sz2)
    {
        return sz1 + sz2;
    }

    public static LPSize Subtract(LPSize sz1, LPSize sz2)
    {
        return sz1 - sz2;
    }

    public static LPSize operator +(LPSize sz1, LPSize sz2)
    {
        return new LPSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
    }

    public static LPSize operator -(LPSize sz1, LPSize sz2)
    {
        return new LPSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
    }

    public readonly bool Equals(LPSize obj)
    {
        if (Width == obj.Width)
        {
            return Height == obj.Height;
        }

        return false;
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj is LPSize obj2)
        {
            return Equals(obj2);
        }

        return false;
    }

    public static bool operator ==(LPSize left, LPSize right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LPSize left, LPSize right)
    {
        return !left.Equals(right);
    }

    public override readonly int GetHashCode()
    {
        HashCode hashCode = default(HashCode);
        hashCode.Add(Width);
        hashCode.Add(Height);
        return hashCode.ToHashCode();
    }
}