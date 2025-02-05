using BlazorThreeJS.Maths;
using FoundryBlazor.Extensions;
using System.Xml.Linq;

namespace FoundryBlazor.Shape;

public readonly struct Point2D : IEquatable<Point2D>
{
    public readonly double U { get; }
    public readonly double V { get; }
    public readonly string Name { get; init; }

    public Point2D(double u, double v)
    {
        U = u;
        V = v;
        Name = "";
    }

    public Point2D(double u, double v, string name)
    {
        U = u;
        V = v;
        Name = name;
    }

    public Vector2 AsVector2() => new Vector2(U, V);

    // Basic vector operations
    public static Point2D operator +(Point2D a, Point2D b) =>
        new Point2D(a.U + b.U, a.V + b.V, a.Name);

    public static Point2D operator -(Point2D a, Point2D b) =>
        new Point2D(a.U - b.U, a.V - b.V, a.Name);

    public static Point2D operator *(Point2D a, double scalar) =>
        new Point2D(a.U * scalar, a.V * scalar, a.Name);

    // Common vector calculations
    public double Magnitude => Math.Sqrt(U * U + V * V);

    public Point2D Normalize()
    {
        var mag = Magnitude;
        return mag > 0 ? this * (1.0 / mag) : new Point2D(0, 0, Name);
    }

    public double DotProduct(Point2D other) =>
        U * other.U + V * other.V;

    // Value equality implementation
    public bool Equals(Point2D other) =>
        U.Equals(other.U) && V.Equals(other.V);

    public override int GetHashCode() =>
        HashCode.Combine(U, V);

    public override string ToString() =>
        $"Point2D({U}, {V})";

    // Convenience methods
    public static Point2D Zero => new Point2D(0, 0);
    public static Point2D UnitU => new Point2D(1, 0);
    public static Point2D UnitV => new Point2D(0, 1);
}