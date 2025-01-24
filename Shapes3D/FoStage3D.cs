using BlazorThreeJS.Viewers;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;

namespace FoundryBlazor.Shape;

public interface IStage
{
    FoStage3D ClearStage();
    FoStage3D UpdateStage();
    V AddShape<V>(V shape) where V : FoGlyph3D;
    T RemoveShape<T>(T value) where T : FoGlyph3D;
}

public class FoStage3D : FoGlyph3D, IStage
{
    public static bool RefreshMenus { get; set; } = true;
    public bool IsActive { get; set; } = false;
    public double StageMargin { get; set; } = .50;  //meters
    public double StageWidth { get; set; } = 30.0;  //meters
    public double StageHeight { get; set; } = 30.0;  //meters
    public double StageDepth { get; set; } = 30.0;  //meters



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
        SetBoundary(width, height, depth);
    }

    public (string, FoShape3D?) FindMemberByPath(List<FoShape3D> members, string path)
    {
        var parts = path.Split('.', 2); // Split into first part and the rest
        var firstPart = parts[0];
        var rest = parts.Length > 1 ? parts[1] : null;

        var current = members.FirstOrDefault(x => x.GetName().Matches(firstPart));
        if (current == null || rest == null) return (path, current!);
        return (path, FindMemberByPath(current, rest));
    }

    public FoShape3D? FindMemberByPath(FoShape3D parent, string path)
    {
        if ( path == null || parent == null) return parent;

        var parts = path.Split('.');
        var current = parent;
        foreach (var part in parts)
        {
            current = current.GetMembers<FoShape3D>().FirstOrDefault(x => x.GetName().Matches(part));
            if (current == null) return null;
        }
        return current;
    }


    public override IEnumerable<TreeNodeAction> GetTreeNodeActions()
    {
        var result = base.GetTreeNodeActions().ToList();
        result.AddAction("Clear", "btn-danger", () =>
        {
            ClearStage();
         });

        result.AddAction("Render", "btn-success", () =>
        {
            UpdateStage();
        });
        return result;
    }

    public List<FoShape3D> GetShapes3D()
    {
        return Shapes3D.Values().Cast<FoShape3D>().ToList();
    }

    public List<FoPipe3D> GetPipes3D()
    {
        return Pipes3D.Values().Cast<FoPipe3D>().ToList();
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
    
    public FoStage3D ClearStage()
    {

       Shapes3D.ForEach(shape => shape.DeleteFromStage(this));
       Pipes3D.ForEach(pipe => pipe.DeleteFromStage(this));
       return this;
    }

     public FoStage3D UpdateStage()
    {

       Shapes3D.ForEach(shape => shape.SetValue3DDirty(true));
       Pipes3D.ForEach(pipe => pipe.SetValue3DDirty(true));
       return this;
    }

    public T AddShape<T>(T value) where T : FoGlyph3D
    {

        var collection = DynamicSlot(value.GetType());
        if (string.IsNullOrEmpty(value.Key))
            value.Key = collection.NextItemName();
        

        value.IsDirty = true;
        collection.AddObject(value.Key, value);

        if (value is IPipe3D)
        {
            Pipes3D.Add(value);
            //$"IPipe3D Added {value.Name}".WriteSuccess();
        }
        else if (value is IShape3D)
        {
            Shapes3D.Add(value);
            //$"IShape3D Added {value.Key}".WriteSuccess();
        }

        return value;
    }

    public T RemoveShape<T>(T value) where T : FoGlyph3D
    {

        var collection = DynamicSlot(value.GetType());
        if (string.IsNullOrEmpty(value.Key))
        {
            value.Key = collection.NextItemName();
        }

        value.IsDirty = true;
        collection.RemoveObject(value.Key);

        if (value is IShape3D)
        {
            Shapes3D.Remove(value);
            //$"IShape3D Added {value.Key}".WriteSuccess();
        }
        else if (value is IPipe3D)
        {
            Pipes3D.Remove(value);
            //$"IPipe3D Added {value.Name}".WriteSuccess();
        }

        return value;
    }


    public override bool RefreshScene(Scene3D scene, bool deep = true)
    {

        //$"Stage RefreshScene".WriteNote();
        Shapes3D?.ForEach(shape => {
            shape.SetValue3DDirty(true);
            shape.RefreshScene(scene, deep);
        });
        //Pipes3D?.ForEach(shape => shape.Render(scene, tick, fps));
        //scene.ForceSceneRefresh();
        return true;
    }



 


}