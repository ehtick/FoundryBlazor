

using System.Drawing;

namespace FoundryBlazor.Shape;
public enum HitShape
{
    None,
    Rectangle,
    Circle,
    LineSegment,
}
public interface ICanHitTarget
{
    Rectangle HitTestRect();
    Point[] HitTestSegment();
    bool IsSmashed();
}

public class QuadHitTarget 
{
    public Rectangle rect;
    public Point point;
    public ICanHitTarget target;
    public HitShape hitShape = HitShape.None;

    public QuadHitTarget Purge()
    {
        rect = Rectangle.Empty;
        point = Point.Empty;
        target = null!;
        hitShape = HitShape.None;
        return this;
    }


    public QuadHitTarget(Rectangle rect, ICanHitTarget target)
    {
        Update(rect, target);
        this.target = target;
    }

    public QuadHitTarget Update(Rectangle rect, ICanHitTarget target)
    {
        this.rect = rect;
        this.point = new Point(rect.X + rect.Width, rect.Y + rect.Height);
        this.target = target;
        this.hitShape = HitShape.Rectangle;
        return this;
    }

    public QuadHitTarget(Point point1, Point point2, ICanHitTarget target)
    {
        Update(point1, point2, target);
        this.target = target;
    }

    public QuadHitTarget Update(Point point1, Point point2, ICanHitTarget target)
    {
        var width = point2.X - point1.X;
        var height = point2.Y - point1.Y;
        this.rect = new Rectangle(point1.X, point1.Y, width, height);
        this.point = point2;
        this.target = target;
        this.hitShape = HitShape.LineSegment;
        return this;
    }

    public bool IsOpen()
    {
        return hitShape == HitShape.None;
    }
    public bool IsLineSegment()
    {
        return hitShape == HitShape.LineSegment;
    }
    public bool IsRectangle()
    {
        return hitShape == HitShape.Rectangle;
    }

    public bool IsContainedBy(Rectangle rect)
    {
        if (IsLineSegment())
        {
            return rect.Contains(this.rect) && rect.Contains(point);
        }
        else if (IsRectangle())
        {
            return rect.Contains(this.rect);
        }
        return false;
    }

   public bool SegmentIntersects(Rectangle rect)
    {
        // Cohen–Sutherland algorithm for line clipping
        int OutCode(Point p)
        {
            int code = 0;
            if (p.X < rect.X) code |= 1;
            else if (p.X > rect.X + rect.Width) code |= 2;
            if (p.Y < rect.Y) code |= 4;
            else if (p.Y > rect.Y + rect.Height) code |= 8;
            return code;
        }

        var Start = rect.Location;
        var End = point;
        int outcode0 = OutCode(Start);
        int outcode1 = OutCode(End);

        while (true)
        {
            if ((outcode0 | outcode1) == 0) return true; // Both points inside
            if ((outcode0 & outcode1) != 0) return false; // Line is outside

            int outcodeOut = (outcode0 != 0) ? outcode0 : outcode1;
            Point p = new Point(0, 0);

            if ((outcodeOut & 8) != 0) // Above
            {
                p.X = Start.X + (End.X - Start.X) * (rect.Y + rect.Height - Start.Y) / (End.Y - Start.Y);
                p.Y = rect.Y + rect.Height;
            }
            else if ((outcodeOut & 4) != 0) // Below
            {
                p.X = Start.X + (End.X - Start.X) * (rect.Y - Start.Y) / (End.Y - Start.Y);
                p.Y = rect.Y;
            }
            else if ((outcodeOut & 2) != 0) // Right
            {
                p.Y = Start.Y + (End.Y - Start.Y) * (rect.X + rect.Width - Start.X) / (End.X - Start.X);
                p.X = rect.X + rect.Width;
            }
            else if ((outcodeOut & 1) != 0) // Left
            {
                p.Y = Start.Y + (End.Y - Start.Y) * (rect.X - Start.X) / (End.X - Start.X);
                p.X = rect.X;
            }

            if (outcodeOut == outcode0)
            {
                Start = p;
                outcode0 = OutCode(Start);
            }
            else
            {
                End = p;
                outcode1 = OutCode(End);
            }
        }
    }

    public bool SegmentContainsPoint(Point point, double tolerance = 0.1)
    {
        var Start = rect.Location;
        var End = point;

        double crossProduct = (point.Y - Start.Y) * (End.X - Start.X) - (point.X - Start.X) * (End.Y - Start.Y);
        if (Math.Abs(crossProduct) > tolerance) return false;

        double dotProduct = (point.X - Start.X) * (End.X - Start.X) + (point.Y - Start.Y) * (End.Y - Start.Y);
        if (dotProduct < 0) return false;

        double squaredLength = (End.X - Start.X) * (End.X - Start.X) + (End.Y - Start.Y) * (End.Y - Start.Y);
        if (dotProduct > squaredLength) return false;

        return true;
    }

}