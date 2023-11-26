
 
using BlazorComponentBus;
using FoundryBlazor.PubSub;
using FoundryBlazor.Shared;


namespace FoundryBlazor.Shape;


public class ShapeHovering : BaseInteraction
{
    public ShapeHovering(
            int priority,
            string cursor,
            IDrawing draw,
            ComponentBus pubsub,
            ToolManagement tools
        ) : base(priority, cursor, draw, pubsub, tools)
    {
        Style = ToolManagement.InteractionStyle<ShapeHovering>();
    }

    public override bool MouseMove(CanvasMouseArgs args)
    {
        var list = new List<ShapeHoverUIEvent>();

        lastHover?.ForEach(child =>
        {
            child.HoverDraw = null;
            list.Add(new ShapeHoverUIEvent(child));
        });
        lastHover?.ForEach(child => child.LocalMouseHover(args, null));



        var loc = GetPanZoomService().HitRectStart(args);
        lastHover = GetHitTestService().FindGlyph(loc);

        lastHover.ForEach(child =>
        {
            child.HoverDraw = OnHover;
            list.Add(new ShapeHoverUIEvent(child));
        });
        lastHover.ForEach(child => child.LocalMouseHover(args, OnSubHover));


        //list.ForEach(item => pubsub.Publish(item));    

        return true;
    }

}