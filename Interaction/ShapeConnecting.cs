using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;
using FoundryBlazor.Canvas;
using FoundryBlazor.Extensions;
using BlazorComponentBus;

using IoBTMessage.Models;

namespace FoundryBlazor.Shape;


public class ShapeConnecting :  ShapeHovering
{
    private bool isConnecting = false;


    public ShapeConnecting(
            FoDrawing2D draw,
            ComponentBus pub,
            IPanZoomService panzoom,
            ISelectionService select,
            IPageManagement manager,
            IHitTestService hitTest
        ): base(draw,pub,panzoom,select,manager,hitTest)
    {
    }
    public override void Abort()
    {     
        isConnecting = false;
        lastHover?.ForEach(child => child.HoverDraw = null);
    }

    public override bool IsDefaultTool(CanvasMouseArgs args)
    {
        dragArea = panZoomService.HitRectStart(args);
        var findings = ValidDragSource(dragArea);
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
            var rect = panZoomService.TransformRect(dragArea);
            await ctx.StrokeRectAsync(rect.X, rect.Y, rect.Width, rect.Height);
            await ctx.StrokeAsync();
        }
    }
       
    public override bool MouseDown(CanvasMouseArgs args)
    {
        if ( selectedShape != null )
        {
            isConnecting = true;
            selectionService?.ClearAllWhen(true);
            selectionService?.AddItem(selectedShape);
        }

        return true;
    }

    private List<FoGlyph2D> ValidDragSource(Rectangle rect)
    {
        var findings = pageManager?.FindGlyph(rect);
        var heros = findings!.Where(item => item is FoHero2D);
        var targets = heros.Where(item => item.Tag.Matches(nameof(DT_AssetFile)));
        return targets.ToList(); 
    }

    private List<FoGlyph2D> ValidDropTargets(Rectangle rect)
    {
        var findings = pageManager?.FindGlyph(rect);
        var heros = findings!.Where(item => item is FoHero2D);
        var targets = heros.Where(item => !item.Tag.Matches(nameof(DT_AssetFile)));
        return targets.ToList(); 
    }

    public override bool MouseUp(CanvasMouseArgs args)
    {
        if (isConnecting && selectedShape != null)
        {
            isConnecting = false;
            var over = panZoomService.HitRectStart(args);
            var findings = ValidDropTargets(over);
            var found = findings!.Where(item => item != selectedShape).FirstOrDefault();

            if ( found != null)
            {
                //link this in the model and force a new layout
                var msg = new AttachAssetFileEvent()
                {
                    AssetFile = (FoHero2D)selectedShape,
                    Target = (FoHero2D)found
                };
                pubsub?.Publish<AttachAssetFileEvent>(msg);
                return true;
            }
        }
        return false;
    }
    public override bool MouseMove(CanvasMouseArgs args)
    {
        //SendUserMove(args, true);
        if (isConnecting) {
            dragArea = panZoomService.HitRectStart(args);
            var move = panZoomService.Movement();

            drawing.MoveSelectionsBy(move.X, move.Y);
        }

        var over = panZoomService.HitRectStart(args);
        var found = ValidDropTargets(over);

        lastHover?.ForEach(child => child.HoverDraw = null);

        if ( selectedShape != null)
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
        await ctx.StrokeRectAsync(-5, -5, obj.Width+10, obj.Height+10);

        await ctx.RestoreAsync();
    };



}