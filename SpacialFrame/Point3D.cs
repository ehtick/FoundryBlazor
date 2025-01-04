
using BlazorThreeJS.Maths;
using FoundryBlazor.Extensions;
using System.Xml.Linq;


namespace FoundryBlazor.Shape;

public readonly struct Point3D : IEquatable<Point3D>
{
    public readonly double X { get; }
    public readonly double Y { get; }
    public readonly double Z { get; }

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 AsVector3() => new Vector3(X, Y, Z);

    // Basic vector operations
    public static Point3D operator +(Point3D a, Point3D b) =>
        new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Point3D operator -(Point3D a, Point3D b) =>
        new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Point3D operator *(Point3D a, double scalar) =>
        new Point3D(a.X * scalar, a.Y * scalar, a.Z * scalar);

    // Common vector calculations
    public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

    public Point3D Normalize()
    {
        var mag = Magnitude;
        return mag > 0 ? this * (1.0 / mag) : new Point3D(0, 0, 0);
    }

    public double DotProduct(Point3D other) =>
        X * other.X + Y * other.Y + Z * other.Z;

    public Point3D CrossProduct(Point3D other) =>
        new Point3D(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );

    // Value equality implementation

    public bool Equals(Point3D other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);


    public override int GetHashCode() =>
        HashCode.Combine(X, Y, Z);

    public override string ToString() =>
        $"Point3D({X}, {Y}, {Z})";

    // Convenience methods
    public static Point3D Zero => new Point3D(0, 0, 0);
    public static Point3D UnitX => new Point3D(1, 0, 0);
    public static Point3D UnitY => new Point3D(0, 1, 0);
    public static Point3D UnitZ => new Point3D(0, 0, 1);
}
