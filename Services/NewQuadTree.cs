using System.Drawing;

public class NewQuadTree
{
    private const int MAX_CAPACITY = 4;
    private const int MAX_DEPTH = 10;

    private Rectangle boundary;
    private List<LineSegment> segments;
    private NewQuadTree[] children;
    private int depth;

    public NewQuadTree(Rectangle boundary, int depth = 0)
    {
        this.boundary = boundary;
        this.segments = new List<LineSegment>();
        this.children = null!;
        this.depth = depth;
    }

    public void Insert(LineSegment segment)
    {
        if (!segment.Intersects(boundary))
            return;

        if (segments.Count < MAX_CAPACITY || depth == MAX_DEPTH)
        {
            segments.Add(segment);
            return;
        }

        if (children == null)
            Subdivide();

        if ( children != null)
            foreach (var child in children)
            {
                child.Insert(segment);
            }
        

    }

    private void Subdivide()
    {
        var x = boundary.X;
        var y = boundary.Y;
        var w = boundary.Width / 2;
        var h = boundary.Height / 2;

        children = new NewQuadTree[4];
        children[0] = new NewQuadTree(new Rectangle(x, y, w, h), depth + 1);
        children[1] = new NewQuadTree(new Rectangle(x + w, y, w, h), depth + 1);
        children[2] = new NewQuadTree(new Rectangle(x, y + h, w, h), depth + 1);
        children[3] = new NewQuadTree(new Rectangle(x + w, y + h, w, h), depth + 1);

        foreach (var segment in segments)
        {
            foreach (var child in children)
            {
                child.Insert(segment);
            }
        }

        segments.Clear();
    }

    public List<LineSegment> QueryRange(Rectangle range)
    {
        var segmentsInRange = new List<LineSegment>();

        if (!range.IntersectsWith(boundary))
            return segmentsInRange;

        foreach (var segment in segments)
        {
            if (segment.Intersects(range))
                segmentsInRange.Add(segment);
        }

        if (children != null)
        {
            foreach (var child in children)
            {
                segmentsInRange.AddRange(child.QueryRange(range));
            }
        }

        return segmentsInRange;
    }

    public List<LineSegment> QueryPoint(Point point)
    {
        var segmentsContainingPoint = new List<LineSegment>();

        if (!boundary.Contains(point))
            return segmentsContainingPoint;

        foreach (var segment in segments)
        {
            if (segment.ContainsPoint(point))
                segmentsContainingPoint.Add(segment);
        }

        if (children != null)
        {
            foreach (var child in children)
            {
                segmentsContainingPoint.AddRange(child.QueryPoint(point));
            }
        }

        return segmentsContainingPoint;
    }
}


public class QuadTreeExample
{
    public static void Main()
    {
        // Create a QuadTree with a 100x100 boundary
        var quadTree = new NewQuadTree(new Rectangle(0, 0, 100, 100));

        // Insert some line segments
        var segments = new List<LineSegment>
        {
            new LineSegment(new Point(10, 10), new Point(20, 20)),
            new LineSegment(new Point(30, 30), new Point(40, 40)),
            new LineSegment(new Point(50, 50), new Point(60, 60)),
            new LineSegment(new Point(70, 70), new Point(80, 80)),
            new LineSegment(new Point(90, 0), new Point(0, 90))
        };

        foreach (var segment in segments)
        {
            quadTree.Insert(segment);
        }

        // Query segments in a specific range
        var queryRange = new Rectangle(25, 25, 30, 30);
        var segmentsInRange = quadTree.QueryRange(queryRange);

        Console.WriteLine($"Segments intersecting range ({queryRange.X}, {queryRange.Y}, {queryRange.Width}, {queryRange.Height}):");
        foreach (var segment in segmentsInRange)
        {
            Console.WriteLine($"({segment.Start.X}, {segment.Start.Y}) - ({segment.End.X}, {segment.End.Y})");
        }

        // Query segments containing a specific point
        var queryPoint = new Point(35, 35);
        var segmentsContainingPoint = quadTree.QueryPoint(queryPoint);

        Console.WriteLine($"\nSegments containing point ({queryPoint.X}, {queryPoint.Y}):");
        foreach (var segment in segmentsContainingPoint)
        {
            Console.WriteLine($"({segment.Start.X}, {segment.Start.Y}) - ({segment.End.X}, {segment.End.Y})");
        }

        // Insert more random segments to test subdivision
        // Random rand = new Random();
        // for (int i = 0; i < 100; i++)
        // {
        //     var start = new Point(rand.NextDouble() * 100, rand.NextDouble() * 100);
        //     var end = new Point(rand.NextDouble() * 100, rand.NextDouble() * 100);
        //     quadTree.Insert(new LineSegment(start, end));
        // }

        // Query again to verify subdivision
        var largeQueryRange = new Rectangle(0, 0, 100, 100);
        var allSegments = quadTree.QueryRange(largeQueryRange);
        Console.WriteLine($"\nTotal number of segments in the QuadTree: {allSegments.Count}");
    }
}