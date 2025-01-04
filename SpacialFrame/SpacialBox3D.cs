
using FoundryBlazor.Extensions;
using System.Drawing;
using System.Xml.Linq;


namespace FoundryBlazor.Shape;

public class SpacialBox3D 
{
    public string Units { get; set; } = "m";
    public double Width { get; set; } = 1.0;
    public double Height { get; set; } = 1.0;
    public double Depth { get; set; } = 1.0;

    public Point3D Pivot { get; set; } = new Point3D(0, 0, 0);

    public double Volume => Width * Height * Depth;

    public double SurfaceArea => 2 * (Width * Height + Height * Depth + Width * Depth);

    public SpacialBox3D(double width, double height, double depth, string units = "m")
    {
        Width = width;
        Height = height;
        Depth = depth;
        Units = units;
    }

    public Point3D Center => new Point3D(Width / 2, Height / 2, Depth / 2);

    public Point3D TopLeftFront => new Point3D(0, Height, Depth);
    public Point3D TopRightFront => new Point3D(Width, Height, Depth);
    public Point3D BottomLeftFront => new Point3D(0, 0, Depth);
    public Point3D BottomRightFront => new Point3D(Width, 0, Depth);
    public Point3D TopLeftBack => new Point3D(0, Height, 0);
    public Point3D TopRightBack => new Point3D(Width, Height, 0);
    public Point3D BottomLeftBack => new Point3D(0, 0, 0);
    public Point3D BottomRightBack => new Point3D(Width, 0, 0);

    public List<Point3D> Vertices => new List<Point3D>
    {
        TopLeftFront,
        TopRightFront,
        BottomLeftFront,
        BottomRightFront,
        TopLeftBack,
        TopRightBack,
        BottomLeftBack,
        BottomRightBack
    };

    public Point3D FrontFaceCenter => new Point3D(Width / 2, Height / 2, 0);
    public Point3D RearFaceCenter => new Point3D(Width / 2, Height / 2, Depth);
    public Point3D LeftFaceCenter => new Point3D(0, Height / 2, Depth / 2);
    public Point3D RightFaceCenter => new Point3D(Width, Height / 2, Depth / 2);
    public Point3D TopFaceCenter => new Point3D(Width / 2, Height, Depth / 2);
    public Point3D BottomFaceCenter => new Point3D(Width / 2, 0, Depth / 2);

    public List<Point3D> FaceCenters => new List<Point3D>
    {
        FrontFaceCenter,
        RearFaceCenter,
        LeftFaceCenter,
        RightFaceCenter,
        TopFaceCenter,
        BottomFaceCenter
    };

    public Point3D EdgeCenterTopFront => new Point3D(Width / 2, Height, Depth);
    public Point3D EdgeCenterTopBack => new Point3D(Width / 2, Height, 0);

    public Point3D EdgeCenterTopLeft => new Point3D(0, Height, Depth / 2);
    public Point3D EdgeCenterTopRight => new Point3D(Width, Height, Depth / 2);

    public Point3D EdgeCenterBottomFront => new Point3D(Width / 2, 0, Depth);
    public Point3D EdgeCenterBottomBack => new Point3D(Width / 2, 0, 0);

    public Point3D EdgeCenterBottomLeft => new Point3D(0, 0, Depth / 2);
    public Point3D EdgeCenterBottomRight => new Point3D(Width, 0, Depth / 2);

    public Point3D EdgeCenterFrontLeft => new Point3D(0, Height / 2, Depth);
    public Point3D EdgeCenterFrontRight => new Point3D(Width, Height / 2, Depth);

    public Point3D EdgeCenterBackLeft => new Point3D(0, Height / 2, 0);
    public Point3D EdgeCenterBackRight => new Point3D(Width, Height / 2, 0);

    public Point3D EdgeCenterTop => new Point3D(Width / 2, Height, Depth / 2);
    public Point3D EdgeCenterBottom => new Point3D(Width / 2, 0, Depth / 2);

    public Point3D EdgeCenterFront => new Point3D(Width / 2, Height / 2, Depth);
    public Point3D EdgeCenterBack => new Point3D(Width / 2, Height / 2, 0);

    public List<Point3D> EdgeCenters => new List<Point3D>
    {
        EdgeCenterTopFront,
        EdgeCenterTopBack,
        EdgeCenterTopLeft,
        EdgeCenterTopRight,
        EdgeCenterBottomFront,
        EdgeCenterBottomBack,
        EdgeCenterBottomLeft,
        EdgeCenterBottomRight,
        EdgeCenterFrontLeft,
        EdgeCenterFrontRight,
        EdgeCenterBackLeft,
        EdgeCenterBackRight,
        EdgeCenterTop,
        EdgeCenterBottom,
        EdgeCenterFront,
        EdgeCenterBack
    };

}