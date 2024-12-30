using BlazorComponentBus;
using BlazorThreeJS.Events;
using BlazorThreeJS.Viewers;
using BlazorThreeJS.Settings;

using FoundryBlazor.PubSub;
using FoundryBlazor.Shape;
using FoundryBlazor.Solutions;
using FoundryRulesAndUnits.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using System.Reflection.Metadata.Ecma335;

namespace FoundryBlazor.Shared;

public class Canvas3DComponentBase : ComponentBase, IDisposable
{

    [Inject] public IWorkspace? Workspace { get; set; }
    [Inject] private ComponentBus? PubSub { get; set; }

    [Parameter] public string CanvasStyle { get; set; } = "width:max-content; border:1px solid black;cursor:default";
    [Parameter] public int CanvasWidth { get; set; } = 2500;
    [Parameter] public int CanvasHeight { get; set; } = 4000;

    [Parameter] public ViewerSettings? Settings3D { get; set; }
    [Parameter] public Scene3D? Scene3D { get; set; }
    [Parameter,EditorRequired] public string? SceneName { get; set; }


    protected ViewerThreeD? Viewer3DReference;


    public string GetCanvasStyle()
    {
        var style = new StringBuilder(CanvasStyle)
            .Append("; ")
            .Append("width:")
            .Append(CanvasWidth)
            .Append("px; ")
            .Append("height:")
            .Append(CanvasHeight)
            .Append("px; ")
            .ToString();
        return style;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            "Canvas3DComponentBase DisposeAsync".WriteInfo();
            //await DoStop();
            //await _jsRuntime!.InvokeVoidAsync("AppBrowser.Finalize");
            await ValueTask.CompletedTask;
        }
        catch (Exception ex)
        {
            $"Canvas3DComponentBase DisposeAsync Exception {ex.Message}".WriteError();
        }
    }

    public void Dispose()
    {
        "Canvas3DComponentBase Dispose".WriteInfo();
        PubSub!.UnSubscribeFrom<RefreshUIEvent>(OnRefreshUIEvent);
        GC.SuppressFinalize(this);
    }

    public (bool, Scene3D) GetActiveScene() 
    {
        if (Viewer3DReference == null) 
            return (false, null!);

        var scene = Viewer3DReference.ActiveScene;
        var found = scene != null;
        return (found, scene!);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var (found, scene) = GetActiveScene();
            if (found)
                Workspace?.GetArena()?.SetScene(scene);

            scene?.SetAfterUpdateAction((s,j)=>
            {
                PubSub!.Publish<RefreshUIEvent>(new RefreshUIEvent("Canvas3DComponentBase"));
            });


            PubSub!.SubscribeTo<RefreshUIEvent>(OnRefreshUIEvent);

        }
        await base.OnAfterRenderAsync(firstRender);
    }


    private void OnRefreshUIEvent(RefreshUIEvent e)
    {
        //InvokeAsync(StateHasChanged);
        //$"Canvas3DComponentBase OnRefreshUIEvent StateHasChanged {e.note}".WriteInfo();

        Task.Run(async () =>
        {
            var arena = Workspace?.GetArena();
            if ( arena != null )
                await arena.UpdateArena();
            //$"after ThreeJSView3D.UpdateScene() {e.note}".WriteInfo();
        });
    }




    // public async Task RenderFrameOBSOLITE(double fps)
    // {
    //     if (GetActiveScene() == null) 
    //         return;

    //     tick++;

    //     $"Canvas3D RenderFrame {tick} {fps}".WriteInfo();

    //     Workspace?.PreRender(tick);

    //     var arena = Workspace?.GetArena();
    //     if (arena == null) return;

    //     var stage = arena.CurrentStage();
    //     if (stage == null) return;

    //     // $"RenderFrame {tick} {stage.Name} {stage.IsDirty}".WriteError();

    //     //if you are already rendering then skip it this cycle
    //     //if (drawing.SetCurrentlyRendering(true)) return;


    //     await arena.RenderArena(GetActiveScene(), tick, fps);
    //     //Workspace?.RenderWatermark(Ctx, tick);


    //     //drawing.SetCurrentlyRendering(false);

    //     //Workspace?.PostRender(tick);

    //     if (stage.IsDirty)
    //     {
    //         stage.IsDirty = false;
    //         await GetActiveScene().UpdateScene();
    //         //$"RenderFrame stage.IsDirty  so... ThreeJSView3D.UpdateScene()  {tick} {stage.Name}".WriteSuccess();
    //     }
    // }


}
