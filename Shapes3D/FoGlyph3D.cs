using BlazorThreeJS.Core;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using System.Text.Json.Serialization;


namespace FoundryBlazor.Shape;

public class FoGlyph3D : FoComponent
{
    public string GlyphId { get; set; } = "";
    public string Color { get; set; } = "Green";


    public string GeomType { get; set; } = "";
    public Transform3? Transform { get; set; }
    public Object3D? Value3D { get; set; }


    public Action<FoGlyph3D, int>? ContextLink;

    protected Action<Object3D, int, double>? OnAnimationUpdate { get; set; } = null;




    public double radius { get; set; } = 0.025;
    public double Radius { get { return this.radius; } set { this.radius = AssignDouble(value, radius); } }

    protected double width = 0;
    public double Width { get { return this.width; } set { this.width = AssignDouble(value, width); } }

    protected double height = 0;
    public double Height { get { return this.height; } set { this.height = AssignDouble(value, height); } }

    protected double depth = 0;
    public double Depth { get { return this.depth; } set { this.depth = AssignDouble(value, depth); } }



    [JsonIgnore]
    public Action<FoGlyph3D>? OnDelete { get; set; }

    public List<TreeNodeAction> DefaultActions = [];
 
    public FoGlyph3D() : base("")
    {
    }
    public FoGlyph3D(string name) : base(name)
    {
    }
    public FoGlyph3D(string name, string color) : this(name)
    {
        Color = color;
    }

    public string GetName()
    {
        return Key;
    }

    public (bool success, Vector3 path) HitPosition()
    {
        // if ( GeometryParameter3D.HasValue3D )
        // {
        //     var value = GeometryParameter3D.GetValue3D();
        //     if ( value == null) return (false, null!);

        //     var boundary = value.HitBoundary;
        //     if ( boundary != null)
        //     {
        //         var pos = boundary.GetPosition();
        //         return (true, pos);
        //     }
        // }
        return (false, null!);
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

   public void AddAction(string name, string color, Action action)
    {
        DefaultActions.AddAction(name, color, action);
    }

    public override IEnumerable<TreeNodeAction> GetTreeNodeActions()
    {
        var result = new List<TreeNodeAction>();
        result.AddRange(DefaultActions);
        
        if ( OnDelete != null )
            result.AddAction("Delete", "btn-danger", () =>
            {
                OnDelete?.Invoke(this);
            });

        return result;
    }



    public virtual void DeleteFromStage(FoStage3D stage)
    {
        $"Deleting {GetTreeNodeTitle()}".WriteWarning();

        $"DeleteFromStage {Key} Object3D".WriteInfo();
 

        stage.RemoveShape<FoGlyph3D>(this);
    }





    protected double AssignDouble(double newValue, double oldValue)
    {
        if (Math.Abs(newValue - oldValue) > 0)
            $"AssignDouble {newValue} {oldValue}  SMASH?".WriteNote();

        return newValue;
    }





    public MeshStandardMaterial GetWireframe()
    {
        var result = new MeshStandardMaterial()
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Color = this.Color,
            Wireframe = true
        };
        return result;
    }
    
    public virtual MeshStandardMaterial GetMaterial()
    {
        var result = new MeshStandardMaterial()
        {
            Color = this.Color,
            //Opacity = this.Opacity,
        };
        return result;
    }

    public virtual bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    {
        return false;
    }



    // public FoGlyph3D MoveTo(double x, double y, double z)
    // {
    //     if ( Transform == null )
    //         GetPosition(x, y, z);
    //     else
    //         Transform.Position.Set(x, y, z);
    //     return this;
    // }
    // public virtual Transform3 GetTransform()
    // {
    //     Transform ??= new Transform3();
    //     return Transform;
    // }

    // public virtual Vector3 GetPosition(double x = 0, double y = 0, double z = 0)
    // {
    //     if ( Transform == null )
    //     {
    //         Transform = new Transform3() { Position = new Vector3(x, y, z) };
    //     }
    //     return Transform.Position;
    // }

    // public virtual Vector3 GetPivot(double x = 0, double y = 0, double z = 0)
    // {
    //     if ( Transform == null )
    //     {
    //         Transform = new Transform3() { Pivot = new Vector3(x, y, z) };
    //     }
    //     return Transform.Pivot;
    // }

    // public virtual Vector3 GetScale(double x = 1, double y = 1, double z = 1)
    // {
    //     if ( Transform == null )
    //     {
    //         Transform = new Transform3() { Scale = new Vector3(x, y, z) };
    //     }
    //     return Transform.Scale;
    // }

    // public virtual Euler GetRotation(double x = 0, double y = 0, double z = 0)
    // {
    //     if ( Transform == null )
    //     {
    //         Transform = new Transform3() { Rotation = new Euler(x, y, z) };
    //     }
    //     return Transform.Rotation;
    // }

    public virtual bool OnModelLoadComplete(Guid PromiseGuid)
    {
        return false;
    }





    public virtual bool RefreshToScene(Scene3D scene, bool deep = true)
    {
        return false;
    }




}