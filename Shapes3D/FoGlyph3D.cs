using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;


namespace FoundryBlazor.Shape;

public class FoGlyph3D : FoComponent
{
    public string GlyphId { get; set; } = "";
    public float Opacity { get; set; } = 1.0F;
    public string Color { get; set; } = "Green";
    public string Address { get; set; } = "";


    protected double width = 0;
    public double Width { get { return this.width; } set { this.width = AssignDouble(value, width); } }
    protected double height = 0;
    public double Height { get { return this.height; } set { this.height = AssignDouble(value, height); } }
    protected double depth = 0;
    public double Depth { get { return this.depth; } set { this.depth = AssignDouble(value, depth); } }
    public bool IsVisible
    {
        get { return this.StatusBits.IsVisible; }
        set { this.StatusBits.IsVisible = value; }
    }


    public Action<Scene, FoGlyph3D>? ShapeDraw;
    public List<TreeNodeAction> DefaultActions = [];
    public void AddAction(string name, string color, Action action)
    {
        DefaultActions.AddAction(name, color, action);
    }

    public override IEnumerable<TreeNodeAction> GetTreeNodeActions()
    {
        var result = new List<TreeNodeAction>();
        result.AddRange(DefaultActions);
        return result;
    }
    
    public FoGlyph3D() : base("")
    {
    }
    public FoGlyph3D(string name) : base(name)
    {
    }
    public FoGlyph3D(string name, string color) : base(name)
    {
        Color = color;
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

    public FoGlyph3D SetBoundry(int width, int height, int depth)
    {
        (Width, Height, Depth) = (width, height, depth);
        return this;
    }

    protected double AssignDouble(double newValue, double oldValue)
    {
        if (Math.Abs(newValue - oldValue) > 0)
            Smash(true);

        return newValue;
    }



    public virtual bool Smash(bool force)
    {
        //if ( _matrix == null && !force) return false;
        //$"Smashing {Name} {GetType().Name}".WriteInfo(2);

        // ResetHitTesting = true;
        // this._matrix = null;
        // this._invMatrix = null;

        return true;
    }

    public Action<FoGlyph3D, int>? ContextLink;

    public string GetName()
    {
        return Key;
    }

    public virtual MeshStandardMaterial GetMaterial()
    {
        var result = new MeshStandardMaterial()
        {
            Color = this.Color
        };
        return result;
    }

    public virtual bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    {
        return false;
    }

    public virtual async Task Draw(Scene ctx, int tick)
    {
        //await ctx.SaveAsync();
        ShapeDraw?.Invoke(ctx, this);
        //await ctx.RestoreAsync();
        //await DrawPin(ctx);
        await Task.CompletedTask;
    }

    public FoGlyph3D MoveTo(int x, int y, int z)
    {
        var pos = GetPosition(x, y, z);
        return this;
    }
    public virtual Vector3 GetPosition(int x = 0, int y = 0, int z = 0)
    {
        var result = new Vector3(x, y, z);
        return result;
    }

    public virtual Vector3 GetPivot(int x = 0, int y = 0, int z = 0)
    {
        var result = new Vector3(x, y, z);
        return result;
    }

    public virtual Vector3 GetScale(double x = 1, double y = 1, double z = 1)
    {
        var result = new Vector3(x, y, z);
        return result;
    }

    public virtual Euler GetRotation(int x = 0, int y = 0, int z = 0)
    {
        var result = new Euler(x, y, z);
        return result;
    }

    public virtual async Task<bool> PreRender(FoArena3D arena, bool deep = true)
    {
        await Task.CompletedTask;
        $"NO Prerender {Name}".WriteWarning();
        return false;
    }



    public virtual bool Render(Scene scene, int tick, double fps, bool deep = true)
    {
        scene.ForceSceneRefresh();
        return false;
    }
    public virtual async Task<bool> RemoveFromRender(Scene scene, bool deep = true)
    {
        await scene.ClearScene();
        return false;
    }
    public virtual bool OnModelLoadComplete(Guid PromiseGuid)
    {
        return false;
    }



}