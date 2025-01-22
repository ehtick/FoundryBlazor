
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


namespace FoundryBlazor.Shape;

public class FoModel3D : FoShape3D
{

    public string Url { get; set; } = "";


    public FoModel3D() : base()
    {
    }
    public FoModel3D(string name) : base(name)
    {
    }
    public FoModel3D(string name, string color) : base(name, color)
    {
    }

    public Model3D AsModel3D(Model3DFormats format)
    {
        var model = new Model3D()
        {
            Url = Url,
            Format = format,
            Name = this.GetName(),
            Uuid = this.GetGlyphId(),
            Transform = new Transform3()
            {
                Position = this.GetPosition(),
                Rotation = this.GetRotation(),
                Pivot = this.GetPivot(),
                Scale = this.GetScale(),
            }
        };
        return model;
    }

    //https://BlazorThreeJS.com/reference/Index.html

    public override bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    {
        return GeometryParameter3D.UpdateMeshPosition(xLoc, yLoc, zLoc);
    }

    public override string GetTreeNodeTitle()
    {

        var HasMesh = GeometryParameter3D.HasValue3D ? "Ok" : "No Value3D";
        return $"{GeomType}: {Key} {GetType().Name} {HasMesh} => ";
    }


    public FoModel3D CreateGlb(string url, double width, double height, double depth)
    {
        CreateGlb(url);
        BoundingBox = new Vector3(width, height, depth);
        return this;
    }

    public FoModel3D CreateGlb(string url)
    {
        GeomType = "Glb";
        Url = url;
        $"CreateGlb url [{Url}] ".WriteSuccess();
        return this;
    }



    // public override async Task<bool> PreRender(FoArena3D arena, bool deep = true)
    // {
    //     if (GeometryParameter3D.HasValue3D)
    //         return true;
            
    //     //is symbol ends with ....
    //     //LoadingURL = Symbol.Replace("http:", "https:");
    //     //await Task.CompletedTask;

    //     //LoadingURL = Url;
    //     $"Shape PRERENDER {Name} => {GetTreeNodeTitle()} {Url}".WriteWarning();

    //     var result = await GeometryParameter3D.PreRender(this, arena, deep);
    //     //if (arena.Scene != null)
    //     //    SetupHitTest(arena.Scene);

    //     return result;
    // }




    public override bool RefreshScene(Scene3D scene, bool deep = true)
    {
        RenderPrimitives(scene);

        // SetupHitTest(scene, tick, fps, deep);
        return true;
    }

    public override FoGeometryComponent3D RenderPrimitives(Scene3D scene)
    {
        if (!GeometryParameter3D.HasValue3D)
            GeometryParameter3D.ComputeValue(this);

        //$"FoGeometryComponent3D RenderPrimitives".WriteInfo();

        if (GeometryParameter3D.HasValue3D)
        {
            var geom = GeometryParameter3D.GetValue3D();
            if ( geom.ShouldDelete() )
            {
                scene.RemoveChild(geom);
                GeometryParameter3D.Smash();
            }
            else
            {
                geom.IsDirty();
                scene.AddChild(geom);
            }
        }

        return GeometryParameter3D;
    }

    // public override bool RemoveFromRender(Scene3D scene, bool deep = true)
    // {
    //     GeometryParameter3D.RemoveFromScene(scene);
    //     return true;
    // }




}