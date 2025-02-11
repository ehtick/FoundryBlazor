
using BlazorThreeJS.Core;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;


namespace FoundryBlazor.Shape;

public class FoPipe3D : FoShape3D, IPipe3D
{

    public FoShape3D? FromShape3D { get; set; }
    public FoShape3D? ToShape3D { get; set; }

    private List<Vector3> path3D = new();
    public List<Vector3> Path3D { get { return this.path3D; } set { this.path3D = AssignPath(value, path3D); } }

    private bool closed { get; set; } = false;
    public bool Closed { get { return this.closed; } set { this.closed = AssignBoolean(value, closed); } }

    private double radius { get; set; } = 0.025;
    public double Radius { get { return this.radius; } set { this.radius = AssignDouble(value, radius); } }



    public FoPipe3D(string name) : base(name)
    {
        GeomType = "Pipe";
    }
    public FoPipe3D(string name, string color) : base(name, color)
    {
        GeomType = "Pipe";
    }

     public FoPipe3D CreatePipe(string name, double radius, List<Vector3> path)
    {
        GeomType = "Pipe";
        Radius = radius;
        Key = name;
        Path3D = path;
        return this;
    }

     public FoPipe3D CreateTube(string name, double radius, List<Vector3> path)
    {
        GeomType = "Tube";
        Radius = radius;
        Key = name;
        Path3D = path;
        return this;
    }
     public FoPipe3D CreateLine(string name, List<Vector3> path)
    {
        GeomType = "Line";
        Key = name;
        Path3D = path;
        return this;
    }

    public (bool success, List<Vector3>? path) ComputePath3D()
    {
        var (f1, v1) = FromShape3D?.HitPosition() ?? (false, null!);
        var (f2, v2) = ToShape3D?.HitPosition() ?? (false, null!);

        if (!f1 || !f2) return (false, null);

        var path = new List<Vector3>()
        {
            v1,
            new(v1.X, v1.Y, v2.Z),
            new(v2.X, v1.Y, v2.Z),
            v2
        };
        return (true, path);
    }

    public override Mesh3D AsMesh3D()
    {
       var result = GeomType switch
        {
            "Pipe" => AsPipe(),
            "Tube" => AsTube(),
            "Line" => AsLine(),
            _ => AsBoundary(),
        };
        FinaliseValue3D(result);


        return result;
    }

    public Mesh3D AsPipe()
    {

        var (success, Path3D) = ComputePath3D();

        if (!success) 
            return new Mesh3D();

        var geometry = new TubeGeometry(Radius, Path3D!, 8, 10);
        var mesh = CreateMesh(geometry);
        return mesh;
    }

    public Mesh3D AsTube()
    {
        var list = new List<Vector3>(Path3D);
        if ( Closed )
            list.Add(Path3D[0]);

        var geometry = new TubeGeometry(Radius, list, 8, 10);
        var mesh = CreateMesh(geometry);
        return mesh;
    }
    public Mesh3D AsLine()
    {
        var list = new List<Vector3>(Path3D);
        if ( Closed )
            list.Add(Path3D[0]);

        var geometry = new LineGeometry(list);
        var mesh = CreateMesh(geometry);
        return mesh;
    }
}

