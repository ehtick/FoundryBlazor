
using BlazorThreeJS.Core;
using BlazorThreeJS.Enums;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Viewers;
using BlazorThreeJS.Settings;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using static System.Formats.Asn1.AsnWriter;
using FoundryRulesAndUnits.Models;


namespace FoundryBlazor.Shape;

public class FoShape3D : FoGlyph3D, IShape3D
{

    public List<FoPanel3D>? TextPanels { get; set; }



    public FoShape3D() : base()
    {
    }
    public FoShape3D(string name) : base(name)
    {
    }
    public FoShape3D(string name, string color) : base(name, color)
    {
    }

    //https://BlazorThreeJS.com/reference/Index.html

    // public override bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    // {
    //     return GeometryParameter3D.UpdateMeshPosition(xLoc, yLoc, zLoc);
    // }

    public override string GetTreeNodeTitle()
    {
        var pos = Transform?.Position ?? new Vector3();
        var HasMesh = Value3D != null ? "Ok" : "No Value3D";
        return $"{GeomType}: {Key} {Color} {GetType().Name} B:{Width:F1} {Height:F1} {Depth:F1} P:{pos.X:F1} {pos.Y:F1} {pos.Z:F1} {HasMesh} => ";
    }

    public override IEnumerable<ITreeNode> GetTreeChildren()
    {
        var list = base.GetTreeChildren().ToList();

        //$"FoShape3D GetTreeChildren has value {GeometryParameter3D.HasValue3D}".WriteInfo();

        //list.Add(GeometryParameter3D);

        // if ( GeometryParameter3D.HasValue3D )
        // {
        //     var value = GeometryParameter3D.GetValue3D();
        //     list.Add(value);
        // }

        foreach (var item in Members<FoShape3D>())
        {
            list.Add(item);
        }

        return list;
    }


    public FoShape3D CreateBox(string name, double width, double height, double depth)
    {
        GeomType = "Box";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    public FoShape3D CreateBoundary(string name, double width, double height, double depth)
    {
        GeomType = "Boundary";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    public FoShape3D CreateGroup(string name, double width, double height, double depth)
    {
        GeomType = "Group";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    public FoShape3D CreateCylinder(string name, double width, double height, double depth)
    {
        GeomType = "Cylinder";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }

    //CreateDodecahedron
    public FoShape3D CreateDodecahedron(string name, double width, double height, double depth)
    {
        GeomType = "Dodecahedron";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    //CreateIcosahedron
    public FoShape3D CreateIcosahedron(string name, double width, double height, double depth)
    {
        GeomType = "Icosahedron";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    //CreateOctahedron
    public FoShape3D CreateOctahedron(string name, double width, double height, double depth)
    {
        GeomType = "Octahedron";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    //CreateTorus
    public FoShape3D CreateTetrahedron(string name, double width, double height, double depth)
    {
        GeomType = "Tetrahedron";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    //CreateTorusKnot
    public FoShape3D CreateTorusKnot(string name, double width, double height, double depth)
    {
        GeomType = "TorusKnot";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    //CreateTorus 
    public FoShape3D CreateTorus(string name, double width, double height, double depth)
    {
        GeomType = "Torus";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }

    public FoShape3D CreateTube(string name, double radius, List<Vector3> path)
    {
        GeomType = "Tube";
        Radius = radius;
        Key = name;
        //Path = path;
        return this;
    }



    public FoShape3D CreateSphere(string name, double width, double height, double depth)
    {
        GeomType = "Sphere";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }

    public FoShape3D CreateCircle(string name, double width, double height, double depth)
    {
        GeomType = "Circle";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    public FoShape3D CreatePlane(string name, double width, double height, double depth)
    {
        GeomType = "Plane";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }

    public FoShape3D CreateRing(string name, double width, double height, double depth)
    {
        GeomType = "Ring";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }

    public FoShape3D CreateCapsule(string name, double width, double height, double depth)
    {
        GeomType = "Capsule";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
    public FoShape3D CreateCone(string name, double width, double height, double depth)
    {
        GeomType = "Cone";
        Key = name;
        Width = width;
        Height = height;
        Depth = depth;
        return this;
    }
  

    public override MeshStandardMaterial GetMaterial()
    {
        var result = GetWireframe();
        result.Wireframe = false;
        return result;
    }


    public void SetAnimationUpdate(Action<Object3D, int, double> update)
    {
        OnAnimationUpdate = update;
        // if ( !GeometryParameter3D.HasValue3D )
        //     GeometryParameter3D.ComputeValue3D(this, null);

        
        // var value = GeometryParameter3D.GetValue3D();
        // if ( value != null)
        //     value.SetAnimationUpdate(update);
        
    }
    public override bool RefreshToScene(Scene3D scene, bool deep = true)
    {
        //var (obj, result) = RenderPrimitives(scene);
        return true;
    }

    // public override (FoGeometryComponent3D, Object3D value) RenderPrimitives(Object3D parent)
    // {
    //     if (!GeometryParameter3D.HasValue3D)
    //         GeometryParameter3D.ComputeValue3D(this,parent);

    //     //$"FoGeometryComponent3D RenderPrimitives".WriteInfo();

    //     var result = GeometryParameter3D.GetValue3D();
    //     if (GeometryParameter3D.HasValue3D)
    //     {
    //         foreach (var item in Members<FoShape3D>())
    //         {
    //             item.RenderPrimitives(result);
    //         }

    //         //if ( OnAnimationUpdate != null)
    //         //    result.SetAnimationUpdate(OnAnimationUpdate);   
    //     }

    //     return (GeometryParameter3D, result);
    // }



    public List<FoPanel3D> EstablishTextPanels(ImportSettings model3D)
    {
        if ( TextPanels != null && TextPanels.Count > 0)
            return TextPanels;

        var root = model3D.Transform.Position.CreatePlus(0, 1, 0);
        // if (Position != null && BoundingBox != null)
        //     root = Position.CreatePlus(0, BoundingBox.Y, 0);

        var leftPos = root.CreatePlus(-3, 1, 0);
        var centerPos = root.CreatePlus(0, 1, 0);  
        var rightPos = root.CreatePlus(3, 1, 0);

        //var lines = Targets?.Where(item => item.address.Length < 20 )
        //            .Select((item) => $"{item.domain}: {item.address}").ToList() ?? new List<string>();

        var center = new FoPanel3D("Threads")
        {
            Width = 2.5,
            Height = 1.5,
            Color = "Gray",
            TextLines = new() { "Thread Links" },
            Transform = new Transform3() { Position = centerPos }
        };

        //if this is Mk48 then add a panel for the process steps
        var left = new FoPanel3D("Process")
        {
            Width = 2.5,
            Height = 1.5,
            Color = "Wisteria",
            TextLines = new() { "Process Steps" },
            Transform = new Transform3() { Position = leftPos }
        };
        //if this is Mk48 then add a panel for the BOM
        var right = new FoPanel3D("BOM")
        {
            Width = 2.5,
            Height = 1.5,
            Color = "Pink",
            TextLines = new() { "BOM Structure" },
            Transform = new Transform3() { Position = rightPos }
        };

        TextPanels = new List<FoPanel3D>() { left, center, right };

        return TextPanels;
    }

    // public bool SetupHitTest(Scene ctx, int tick = 0, double fps = 0, bool deep = true)
    // {
    //     //$"SetupHitTest for {Name}".WriteInfo();
    //     UserHit = (ImportSettings model3D) =>
    //     {
    //         //$"In UserHit".WriteInfo();

    //         var list = EstablishTextPanels(model3D);
    //         foreach (var item in list)
    //         {
    //             item.IsVisible = model3D.IsShow();
    //             item.Render(ctx, tick, fps, deep);  
    //         }

    //     };
    //     return true;
    // }




}