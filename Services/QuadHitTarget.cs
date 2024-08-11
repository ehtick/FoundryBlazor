

using FoundryBlazor.Shape;
using SkiaSharp;
using System.Drawing;

public enum HitShape
{
    None,
    Rectangle,
    Circle,
    LineSegment,
}
public class QuadHitTarget 
{
    public Rectangle rect;
    public Point point;
    public FoGlyph2D target;
    public HitShape hitShape = HitShape.None;

    public QuadHitTarget(Rectangle rect, FoGlyph2D target)
    {
        Update(rect, target);
        this.target = target;
    }

    public QuadHitTarget Update(Rectangle rect, FoGlyph2D target)
    {
        this.rect = rect;
        this.point = new Point(rect.X + rect.Width, rect.Y + rect.Height);
        this.target = target;
        this.hitShape = HitShape.Rectangle;
        return this;
    }

    public QuadHitTarget(Point point1, Point point2, FoGlyph2D target)
    {
        Update(point1, point2, target);
        this.target = target;
    }

    public QuadHitTarget Update(Point point1, Point point2, FoGlyph2D target)
    {
        var width = point2.X - point1.X;
        var height = point2.Y - point1.Y;
        this.rect = new Rectangle(point1.X, point1.Y, width, height);
        this.point = point2;
        this.target = target;
        this.hitShape = HitShape.LineSegment;
        return this;
    }
}