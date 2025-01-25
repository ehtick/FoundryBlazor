using BlazorThreeJS.Core;
using BlazorThreeJS.Enums;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Settings;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using System.Text.Json.Serialization;


namespace FoundryBlazor.Shape;

public class FoGeometryComponent3D : FoComponent
{
    public string GlyphId { get; set; } = "";
    protected Object3D Value3D { get; set; }

    public FoGeometryComponent3D(FoGlyph3D owner) : base(owner.GetName())
    {
        GetParent = () => owner;
        Value3D = null!;
    }

    public bool HasValue3D => Value3D != null;
    
    public void SetValue3DDirty(bool value)
    {
        if (Value3D != null)
        {
            Value3D.SetDirty(value);
        }
    }


    public Object3D GetValue3D()
    {
        return Value3D;
    }

    public Object3D DeleteValue3D()
    {
        
        var result = Value3D;
        result.Delete();
        return result;
    }

    public Object3D SetValue3D(Object3D value)
    {
        Value3D = value;
        return Value3D;
    }

    public bool Smash()
    {
        Value3D = null!;
        return Value3D == null;
    }


    public string GetName()
    {
        return Key;
    }


    public string GetGlyphId()
    {
        if (string.IsNullOrEmpty(GlyphId))
            GlyphId = Guid.NewGuid().ToString();

        return GlyphId;
    }

    public bool GlyphIdCompare(string other)
    {
        var id = GetGlyphId();
        var result = id == other;
        // $"GlyphIdCompare {result}  {id} {other}".WriteNote();
        return result;
    }

    public bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    {
        //"Update mesh position".WriteSuccess();
        if (Value3D != null)
        {
            Value3D.Transform.Position.Set(xLoc, yLoc, zLoc);
            Value3D.SetDirty(true);
            return true;
        }
        return false;
    }

    public void RemoveFromScene(Scene3D scene, bool deep = true)
    {
        if (HasValue3D)
        {
            Value3D.ShouldDelete();
            scene.RemoveChild(Value3D);
            Smash();
        }
    }


    private Model3D AsModel3D(FoGlyph3D glyph, Model3DFormats format)
    {
        if ( glyph is not FoModel3D source)
            return null!;

        var model = source.AsModel3D(format);
        return model;
    }

    private Text3D AsText3D(FoGlyph3D glyph)
    {
        if ( glyph is not FoText3D source)
            return null!;

        var model = source.AsText3D();
        return model;
    }
   

   public (FoGeometryComponent3D obj, Object3D value) ComputeValue3D(FoGlyph3D source, Object3D? parent)
    {
        Value3D = source.GeomType switch
        {
            "Collada" => AsModel3D(source,Model3DFormats.Collada),
            "Fbx" => AsModel3D(source, Model3DFormats.Fbx),
            "Obj" => AsModel3D(source, Model3DFormats.Obj),
            "Stl" => AsModel3D(source, Model3DFormats.Stl),
            "Glb" => AsModel3D(source, Model3DFormats.Gltf),

            "Text" => AsText3D(source),

            "Group" => AsGroup(source),
            "Box" => AsBox(source),
            "Boundary" => AsBoundary(source),
            "Circle" => AsCircle(source),
            "Cylinder" => AsCylinder(source),
            "Sphere" => AsSphere(source),
            "Plane" => AsPlane(source),
            "Capsule" => AsCapsule(source),
            "Cone" => AsCone(source),
            "Tube" => AsTube(source),
            "Ring" => AsRing(source),
            "Dodecahedron" => AsDodecahedron(source),
            "Icosahedron" => AsIcosahedron(source),
            "Octahedron" => AsOctahedron(source),
            "Tetrahedron" => AsTetrahedron(source),
            "TorusKnot" => AsTorusKnot(source),
            "Torus" => AsTorus(source),
            _ => Value3D,
        };
        parent?.AddChild(Value3D);
        return (this, Value3D);
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

    public Object3D AsBox(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new BoxGeometry(box.X, box.Y, box.Z));
        return Value3D;
    }

    
    public Object3D AsBoundary(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new BoundaryGeometry(box.X, box.Y, box.Z), source.GetWireframe());
        return Value3D;
    }

    public Object3D AsCylinder(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new CylinderGeometry(box.X / 2, box.X / 2, box.Y));
        return Value3D;
    }



    public Object3D AsSphere(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new SphereGeometry(box.X / 2));
        return Value3D;
    }    


    public Object3D AsTube(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        var geometry = new TubeGeometry(source.Radius, source.Path!, 8, 10);
        Value3D = CreateMesh(source, geometry);
        return Value3D;
    }
    
    public Object3D AsCircle(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new CircleGeometry(box.X / 2));
        return Value3D;
    }
 
 
    public Object3D AsCapsule(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new CapsuleGeometry(box.X / 2, box.Y));
        return Value3D;
    }

    public Object3D AsCone(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new ConeGeometry(box.X / 2, box.Y));
        return Value3D;
    }

    public Object3D AsDodecahedron(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new DodecahedronGeometry(box.X / 2));
        return Value3D;
    }

    public Object3D AsIcosahedron(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new IcosahedronGeometry(box.X / 2));
        return Value3D;
    }

    public Object3D AsOctahedron(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new OctahedronGeometry(box.X / 2));
        return Value3D;
    }

    public Object3D AsTetrahedron(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new TetrahedronGeometry(box.X / 2));
        return Value3D;
    }

    public Object3D AsTorusKnot(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new TorusKnotGeometry(box.X / 2));
        return Value3D;
    }
    public Object3D AsTorus(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new TorusGeometry(box.X / 2));
        return Value3D;
    }

    public Object3D AsPlane(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new PlaneGeometry(box.X, box.Y));
        return Value3D;
    }

    public Object3D AsRing(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;
        var box = source.BoundingBox ?? new Vector3(1, 1, 1);

        Value3D = CreateMesh(source, new RingGeometry(box.X / 2, box.Y / 2));
        return Value3D;
    }
      
    public Object3D AsGroup(FoGlyph3D source)
    {
        if (Value3D != null) return Value3D;

        var box = source.BoundingBox ?? new Vector3(1, 1, 1);
        Value3D = CreateMesh(source, new BoxGeometry(box.X, box.Y, box.Z), source.GetWireframe());
        return Value3D;
    }


}