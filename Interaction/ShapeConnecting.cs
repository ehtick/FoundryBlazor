using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;
using BlazorComponentBus;
 
using FoundryBlazor.Extensions;
using FoundryBlazor.Shared;
using FoundryRulesAndUnits.Extensions;

namespace FoundryBlazor.Shape;


public class ShapeConnecting : ShapeHovering
{
    private bool isConnecting = false;

    public Type SourceType { get; set; } = typeof(FoGlyph2D);
    public Type TargetType { get; set; } = typeof(FoGlyph2D);

    public ShapeConnecting(
            InteractionStyle style,
            int priority,
            string cursor,
            FoDrawing2D draw,
            ComponentBus pubsub,
            IPanZoomService panzoom,
            ISelectionService select,
            IPageManagement manager,
            IHitTestService hitTest
        ) : base(style, priority, cursor, draw, pubsub, panzoom, select, manager, hitTest)
    {
    }
    public override void Abort()
    {
        isConnecting = false;
        lastHover?.ForEach(child => child.HoverDraw = null);
    }

    public override bool IsDefaultTool(CanvasMouseArgs args)
    {
        DragArea = panZoomService.HitRectStart(args);
        var findings = ValidDragSource(DragArea);
        selectedShape = findings.LastOrDefault(); // get one on top

        if (findings?.Count == 1 && selectedShape != null)
        {
            $"selectedShape {selectedShape.GetType().Name}".WriteLine();
            return true;
        }
        return false;
    }

    public override async Task RenderDrawing(Canvas2DContext ctx, int tick)
    {
        if (isConnecting)
        {
            await ctx.BeginPathAsync();
            await ctx.SetLineDashAsync(new float[] { 50, 10 });
            await ctx.SetLineWidthAsync(1);
            await ctx.SetStrokeStyleAsync("Yellow");
            var rect = panZoomService.TransformRect(DragArea);
            await ctx.StrokeRectAsync(rect.X, rect.Y, rect.Width, rect.Height);
            await ctx.StrokeAsync();
        }
    }

    public override bool MouseDown(CanvasMouseArgs args)
    {
        if (selectedShape != null)
        {
            isConnecting = true;
            selectionService?.ClearAllWhen(true);
            selectionService?.AddItem(selectedShape);
        }

        return true;
    }

    private List<FoGlyph2D> ValidDragSource(Rectangle rect)
    {
        var findings = hitTestService?.FindGlyph(rect);
        var heros = findings!.Where(item => item.GetType() == SourceType);
        return heros.ToList();
    }

    private List<FoGlyph2D> ValidDropTarget(Rectangle rect)
    {
        var findings = hitTestService?.FindGlyph(rect);
        var targets = findings!.Where(item => item.GetType() == TargetType);
        //var targets = heros.Where(item => !item.Tag.Matches(TargetType.Name));
        return targets.ToList();
    }

    public override bool MouseUp(CanvasMouseArgs args)
    {
        if (isConnecting && selectedShape != null)
        {
            isConnecting = false;
            var over = panZoomService.HitRectStart(args);
            var findings = ValidDropTarget(over);
            var found = findings!.Where(item => item != selectedShape).FirstOrDefault();

            if (found != null)
            {
                //link this in the model and force a new layout
                var msg = new AttachAssetFileEvent()
                {
                    AssetGuid = selectedShape.GetGlyphId(),
                    TargetGuid = found.GetGlyphId(),
                    AssetShape = selectedShape,
                    TargetShape = found
                };
                pubsub?.Publish<AttachAssetFileEvent>(msg);
                return true;
            }
        }
        drawing.SetInteraction(InteractionStyle.ShapeHovering);
        return false;
    }
    public override bool MouseMove(CanvasMouseArgs args)
    {
        //SendUserMove(args, true);
        if (isConnecting)
        {
            DragArea = panZoomService.HitRectStart(args);
            var move = panZoomService.MouseDeltaMovement();

            drawing.MoveSelectionsBy(move.X, move.Y);
        }

        var over = panZoomService.HitRectStart(args);
        var found = ValidDropTarget(over);

        lastHover?.ForEach(child => child.HoverDraw = null);

        if (selectedShape != null)
        {
            lastHover = found;
            lastHover.ForEach(child => child.HoverDraw = OnHoverTarget);
        }

        return true;
    }


    public Action<Canvas2DContext, FoGlyph2D>? OnHoverTarget { get; set; } = async (ctx, obj) =>
    {
        await ctx.SaveAsync();

        await ctx.SetLineDashAsync(new float[] { 10, 10 });
        await ctx.SetLineWidthAsync(10);
        await ctx.SetStrokeStyleAsync("White");
        await ctx.StrokeRectAsync(-5, -5, obj.Width + 10, obj.Height + 10);

        await ctx.RestoreAsync();
    };



}