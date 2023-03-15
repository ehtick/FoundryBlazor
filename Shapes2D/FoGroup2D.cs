using Blazor.Extensions.Canvas.Canvas2D;

namespace FoundryBlazor.Shape;

public class FoGroup2D : FoGlyph2D, IShape2D
{

    public FoGroup2D() : base()
    {
        ShapeDraw = DrawBox;
    }

    public FoGroup2D(int width, int height, string color) : base("", width, height, color)
    {
        PinX = PinY = 0;
        ShapeDraw = DrawBox;
    }
    public FoGroup2D(string name, int width, int height, string color) : base(name, width, height, color)
    {
        ShapeDraw = DrawBox;
    }

    public T AddShape<T>(T value) where T : FoGlyph2D
    {
        var dx = -LeftEdge();
        var dy = -TopEdge();
        value.MoveBy(dx, dy);
        Slot<T>().Add(value);   
        return value;
    }


    public List<T>? CaptureSelectedShapes<T>(FoGlyph2D source) where T: FoGlyph2D
    {
        var dx = -LeftEdge();
        var dy = -TopEdge();       

        var members = source.ExtractSelected<T>();
        if ( members != null)
        {
            members.ForEach(shape => shape.MoveBy(dx,dy));
            Slot<T>().AddRange(members);
        }
 
        return members;
    }

    public override async Task<bool> RenderDetailed(Canvas2DContext ctx, int tick, bool deep = true)
    {
         if ( CannotRender() ) return false;

        await ctx.SaveAsync();
        await UpdateContext(ctx, tick);

        PreDraw?.Invoke(ctx, this);
        await Draw(ctx, tick);
        HoverDraw?.Invoke(ctx, this);
        PostDraw?.Invoke(ctx, this);

        if (IsSelected)
        {
            DrawSelected?.Invoke(ctx, this);
            await DrawPin(ctx);
        }

        if ( deep) 
        {
            Members<FoShape1D>().ForEach(async child => await child.RenderDetailed(ctx, tick, deep));    
            Members<FoShape2D>().ForEach(async child => await child.RenderDetailed(ctx, tick, deep));       
            Members<FoImage2D>().ForEach(async child => await child.RenderDetailed(ctx, tick, deep));       
            Members<FoText2D>().ForEach(async child => await child.RenderDetailed(ctx, tick, deep));       
       }
        await ctx.RestoreAsync();
        return true;
    }
}
