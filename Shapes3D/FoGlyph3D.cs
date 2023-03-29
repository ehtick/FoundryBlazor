using BlazorThreeJS.Materials;
using BlazorThreeJS.Scenes;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;

namespace FoundryBlazor.Shape;

public class FoGlyph3D : FoComponent
{
    public string GlyphId { get; set; } = "";
    public string PlatformName { get; set; } = "";
    public float Opacity { get; set; } = 1.0F;
    public string Color { get; set; } = "Green";

    protected double width = 0;
    public double Width { get { return this.width; } set { this.width = AssignDouble(value, width); } }
    protected double height = 0;
    public double Height { get { return this.height; } set { this.height = AssignDouble(value, height); } }
    protected double depth = 0;
    public double Depth { get { return this.depth; } set { this.depth = AssignDouble(value, depth); } }

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
        $"Smashing {Name} {GetType().Name}".WriteInfo(2);

        // ResetHitTesting = true;
        // this._matrix = null;
        // this._invMatrix = null;

        return true;
    }

    public Action<FoGlyph3D, int>? ContextLink;

    public bool IsSamePlatform(FoGlyph3D obj)
    {
        return PlatformName.Matches(obj.PlatformName);
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

    public FoGlyph3D MoveTo(int x, int y, int z) 
    {
        var pos = GetPosition();
        pos.Loc(x, y, z);
        return this; 
    }
    public virtual FoVector3D GetPosition()
    {
        var result = new FoVector3D(0, 0, 0);
        return result;
    }


    public virtual bool PreRender(FoArena3D arena, Viewer viewer, bool deep = true)
    {
        return false;
    }

    public virtual bool Render(Scene ctx, int tick, double fps, bool deep = true)
    {
        return false;
    }
    public virtual bool PostRender(Scene ctx, Guid guid)
    {
        return false;
    }

    public virtual List<T>? ExtractWhere<T>(Func<T, bool> whereClause) where T : FoBase
    {
        var target = GetSlot<T>();
        return target?.ExtractWhere(whereClause);
    }

    public virtual List<T>? FindWhere<T>(Func<T, bool> whereClause) where T : FoBase
    {
        var target = GetSlot<T>();
        return target?.FindWhere(whereClause);
    }

}