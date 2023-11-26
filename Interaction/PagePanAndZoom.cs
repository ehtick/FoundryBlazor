 
using BlazorComponentBus;
using FoundryBlazor.PubSub;
using FoundryBlazor.Shared;
using FoundryRulesAndUnits.Extensions;


namespace FoundryBlazor.Shape;


public class PagePanAndZoom : BaseInteraction
{
    private bool isDraggingPage = false;

    public PagePanAndZoom(
            int priority,
            string cursor,
            IDrawing draw,
            ComponentBus pubsub,
            IPanZoomService panzoom,
            ISelectionService select,
            IPageManagement manager,
            IHitTestService hitTest
        ) : base(priority, cursor, draw, pubsub, panzoom, select, manager, hitTest)
    {
         Style = InteractionStyle<PagePanAndZoom>();
    }

    public override void Abort()
    {
        isDraggingPage = false;
    }

    public override bool IsDefaultTool(CanvasMouseArgs args)
    {
        return args.CtrlKey && args.ShiftKey;
    }

    public override bool MouseDown(CanvasMouseArgs args)
    {
        isDraggingPage = true;
        return true;
    }
    public override bool MouseUp(CanvasMouseArgs args)
    {
        isDraggingPage = false;
        InteractionManager?.SetInteraction<ShapeHovering>();
        return true;
    }
    public override bool MouseMove(CanvasMouseArgs args)
    {
        if (isDraggingPage)
        {
            drawing.MovePanBy(args.MovementX, args.MovementY);
            pubsub!.Publish<RefreshUIEvent>(new RefreshUIEvent("PanZoom"));
        }

        return true;
    }



}