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


    public Object3D? Value3D { get; set; }

    protected string geomType = "";
    public string GeomType
    {
        get { return this.geomType; }
        set { this.geomType = AssignText(value, geomType); }
    }
    protected Transform3? transform = null;
  
    public Transform3 Transform
    {
        get { return this.transform != null ? this.transform! : AssignTransform(new Transform3(), transform); }
        set { this.transform = AssignTransform(value, transform); }
    }


    private Action<Object3D, int, double>? OnAnimationUpdate { get; set; } = null;


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

    public void SetValue3D(Object3D obj)
    {
        Value3D = obj;
    }
    
    public virtual (bool success, Object3D result) GetValue3D()
    {
        return (Value3D != null, Value3D!);
    }

    public Object3D FinaliseValue3D(Object3D obj)
    {
        obj.SetDirty(true);
        var self = this;
        //allows for the object to be updated
        var update = (Object3D obj, int index, double value) =>
        {
            OnAnimationUpdate?.Invoke(obj, index, value);
            self.GetValue3D();
        };
        obj.SetAnimationUpdate(update);
        return obj;
    }

    public bool HasChanged()
    {
        return IsDirty || Transform.IsDirty;
    }


    public void SetAnimationUpdate(Action<Object3D, int, double> update)
    {
        OnAnimationUpdate = update; 
    }

    public override void SetDirty(bool value)
    {
        base.SetDirty(value);
            
        if (Value3D != null)
            Value3D.SetDirty(value);
    }


    public (bool success, Vector3 path) HitPosition()
    {
        if ( Value3D != null)
        {
            var boundary = Value3D.HitBoundary;
            if ( boundary != null)
            {
                var pos = boundary.GetPosition();
                return (true, pos);
            }
        }
        return (false, null!);
    }

    public string GetGlyphId()
    {
        if (string.IsNullOrEmpty(GlyphId))
            GlyphId = Guid.NewGuid().ToString();

        return GlyphId;
    }



   public void AddAction(string name, string color, Action action)
    {
        DefaultActions.AddAction(name, color, action);
    }

    public override string GetTreeNodeTitle()
    {
        var pos = Transform?.Position ?? new Vector3();
        var HasMesh = Value3D != null ? "Ok" : "No Value3D";
        return $"{GeomType}: {Key} {Color} {GetType().Name} B:{Width:F1} {Height:F1} {Depth:F1} P:{pos.X:F1} {pos.Y:F1} {pos.Z:F1} {HasMesh} => ";
    }


    public override IEnumerable<ITreeNode> GetTreeChildren()
    {
        var result = new List<ITreeNode>();

        if (Value3D != null)
            result.Add(Value3D);

        result.AddRange(AllSubGlyph3Ds());
        return result;
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



    protected Transform3 AssignTransform(Transform3 newValue, Transform3? oldValue)
    {
        SetDirty(true);  //this is good because it will also mark Valus3D as dirty (which triggers the update)
        if (oldValue != null)
            oldValue.OnChange = null!;

       
        newValue.OnChange = () => SetDirty(true);
        return newValue;
    }

    protected double AssignDouble(double newValue, double oldValue)
    {
        if (Math.Abs(newValue - oldValue) > 0)
        {
            SetDirty(true);
        }

        return newValue;
    }

    protected string AssignText(string newValue, string oldValue)
    {
        if (newValue != oldValue)
        {
            SetDirty(true);
        }

        return newValue;
    }

    protected List<Vector3> AssignPath(List<Vector3> newValue, List<Vector3> oldValue)
    {
        SetDirty(true);
        return newValue;
    }

    protected bool AssignBoolean(bool newValue, bool oldValue)
    {
        if (newValue != oldValue)
        {
            SetDirty(true);
        }

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

    public virtual T? FindSubGlyph3D<T>(string key) where T : FoGlyph3D
    {
        var result = AllSubGlyph3Ds().FirstOrDefault(item => item.Key.Matches(key));

        return result as T;
    }

    public virtual T AddSubGlyph3D<T>(T glyph) where T : FoGlyph3D
    {
        glyph.GetParent = () => this;
        if ( !Members<FoGlyph3D>().Contains(glyph))
            Add<FoGlyph3D>(glyph);

        return glyph;
    }

    public virtual List<FoGlyph3D> AllSubGlyph3Ds()
    {
        var result = Members<FoGlyph3D>().ToList();
        return result;
    }


    public virtual (bool success, Object3D result) ComputeValue3D(Object3D parent)
    {

        var (success, result) = GetValue3D();
        if (!success)
            return (false, null!);    

        parent.AddChild(result);

        foreach (var item in AllSubGlyph3Ds())
        {
            item.ComputeValue3D(result!);
        }

        return (true, result);
    }





    public virtual bool RefreshToScene(Scene3D scene, bool deep = true)
    {
        return false;
    }




}