using BlazorThreeJS.Geometires;
using BlazorThreeJS.Lights;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Viewers;
using FoundryRulesAndUnits;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;

namespace FoundryBlazor.Shape;

public interface IStage
{
    FoStage3D ClearAll();
    V AddShape<V>(V shape) where V : FoGlyph3D;
    T RemoveShape<T>(T value) where T : FoGlyph3D;
}

public class FoStage3D : FoGlyph3D, IStage
{
    public static bool RefreshMenus { get; set; } = true;
    public bool IsActive { get; set; } = false;
    public bool IsDirty { get; set; } = false;  // dirty TRUE means we need to update the scene after render
    public double StageMargin { get; set; } = .50;  //meters
    public double StageWidth { get; set; } = 30.0;  //meters
    public double StageHeight { get; set; } = 30.0;  //meters
    public double StageDepth { get; set; } = 30.0;  //meters


    // private Scene? CurrentScene { get; set; }

    // private Mesh? StageMesh { get; set; }



    protected FoCollection<FoGlyph3D> Pipes3D = new();
    protected FoCollection<FoGlyph3D> Shapes3D = new();

    public FoStage3D() : base()
    {
    }

    public FoStage3D(string name) : base(name)
    {
    }
    public FoStage3D(string name, string color) : base(name, color)
    {
    }


    public FoStage3D(string name, int width, int height, int depth, string color) : base(name, color)
    {
        //ResetLocalPin((obj) => 0, (obj) => 0);
        SetBoundry(width, height, depth);
    }






    public override IEnumerable<ITreeNode> GetTreeChildren()
    {
        var list = new List<ITreeNode>();
        foreach (var item in Pipes3D.Values())
        {
            list.Add(item);
        }
        foreach (var item in Shapes3D.Values())
        {
            list.Add(item);
        }
        return list;
    }
    
    public FoStage3D ClearAll()
    {
       Shapes3D.Clear();
       Pipes3D.Clear();
       return this;
    }

    // public bool EstablishBoundry()
    // {
    //     if (StageMesh != null) 
    //         return false;


    //     StageMesh = new Mesh
    //     {
    //         Geometry = new BoxGeometry(Width, Height, Depth),
    //         Position = new Vector3(0, 0, 0),
    //         Material = new MeshStandardMaterial()
    //         {
    //             Color = "red",
    //             Wireframe = true
    //         }
    //     };

    //     CurrentScene?.Add(StageMesh);


    //     //$"EstablishBoundry {Width} {Height} {Depth}".WriteSuccess();
    //     return true;
    // }

    public T AddShape<T>(T value) where T : FoGlyph3D
    {
        IsDirty = true;
        var collection = DynamicSlot(value.GetType());
        if (string.IsNullOrEmpty(value.Key))
        {
            value.Key = collection.NextItemName();
        }

        collection.AddObject(value.Key, value);

        if (value is IShape3D)
        {
            Shapes3D.Add(value);
            $"IShape3D Added {value.Key}".WriteSuccess();
        }
        else if (value is IPipe3D)
        {
            Pipes3D.Add(value);
            //$"IPipe3D Added {value.Name}".WriteSuccess();
        }



        return value;
    }

    public T RemoveShape<T>(T value) where T : FoGlyph3D
    {
        IsDirty = true;
        var collection = DynamicSlot(value.GetType());
        if (string.IsNullOrEmpty(value.Key))
        {
            value.Key = collection.NextItemName();
        }

        collection.RemoveObject(value.Key);

        if (value is IShape3D)
        {
            Shapes3D.Remove(value);
            $"IShape3D Added {value.Key}".WriteSuccess();
        }
        else if (value is IPipe3D)
        {
            Pipes3D.Remove(value);
            //$"IPipe3D Added {value.Name}".WriteSuccess();
        }

        //do we need to remove mesh from scene

        return value;
    }

    public void PreRender(IArena arena)
    {
        Shapes3D?.ForEach(async shape => await arena.PreRender(shape));
    }
    public async Task RenderDetailed(Scene scene, int tick, double fps)
    {
        IsDirty = false;
        Shapes3D?.ForEach(shape => shape.Render(scene, tick, fps));
        Pipes3D?.ForEach(shape => shape.Render(scene, tick, fps));
        await Task.CompletedTask;
    }



 


}