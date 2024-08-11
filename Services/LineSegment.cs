using System.Drawing;

public class LineSegment
{
    public Point Start { get; set; }
    public Point End { get; set; }

    public LineSegment(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public bool Intersects(Rectangle rect)
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

    public bool ContainsPoint(Point point, double tolerance = 0.1)
    {
        double crossProduct = (point.Y - Start.Y) * (End.X - Start.X) - (point.X - Start.X) * (End.Y - Start.Y);
        if (Math.Abs(crossProduct) > tolerance) return false;

        double dotProduct = (point.X - Start.X) * (End.X - Start.X) + (point.Y - Start.Y) * (End.Y - Start.Y);
        if (dotProduct < 0) return false;

        double squaredLength = (End.X - Start.X) * (End.X - Start.X) + (End.Y - Start.Y) * (End.Y - Start.Y);
        if (dotProduct > squaredLength) return false;

        return true;
    }
}