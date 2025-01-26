
using BlazorThreeJS.Core;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;


namespace FoundryBlazor.Shape;

public class FoPipe3D : FoShape3D, IPipe3D
{
    public List<Vector3>? Path3D { get; set; }
    public FoShape3D? FromShape3D { get; set; }
    public FoShape3D? ToShape3D { get; set; }
    //public double Radius { get; set; } = 1.0;


    public FoPipe3D(string name) : base(name)
    {
        GeomType = "Pipe";
    }
    public FoPipe3D(string name, string color) : base(name, color)
    {
        GeomType = "Pipe";
    }

    public override string GetTreeNodeTitle()
    {
        var box = BoundingBox ?? new Vector3(0, 0, 0);
        var pos = GetPosition();
        var HasMesh = GeometryParameter3D.HasValue3D ? "Ok" : "No Value3D";
        return $"{GeomType}: [{Key}] {Color} {GetType().Name} {HasMesh} => ";
    }

    public (bool success, List<Vector3>? path) ComputePath3D()
    {
        var p1 = FromShape3D?.GeometryParameter3D.GetValue3D();
        var p2 = ToShape3D?.GeometryParameter3D.GetValue3D();

        if ( p1 == null || p2 == null) 
            return (false, null);

        var h1 = p1.HitBoundary;
        var h2 = p2.HitBoundary;

        if ( h1 == null || h2 == null) 
            return (false, null);

        var v1 = h1.GetPosition();
        var v2 = h2.GetPosition();

        var path = new List<Vector3>()
        {
            v1,
            new(v1.X, v1.Y, v2.Z),
            new(v2.X, v1.Y, v2.Z),
            v2
        };
        return (true, path);
    }

    public Mesh3D AsPipe3D()
    {

        var (success, Path3D) = ComputePath3D();

        if (!success) 
            return new Mesh3D();

        var geometry = new TubeGeometry(Radius, Path3D!, 8, 10);
        var mesh = CreateMesh(this, geometry);
        return mesh;
    }

 

    public Mesh3D CreateMesh(FoGlyph3D source, BufferGeometry geometry, Material material = null!)
    {
        return new Mesh3D
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = geometry,
            Transform = new Transform3()
            {
                Position = source.GetPosition(),
                Pivot = source.GetPivot(),
                Scale = source.GetScale(),
                Rotation = source.GetRotation(),
            },
            Material = material != null ? material : source.GetMaterial()
        };
    }
}

