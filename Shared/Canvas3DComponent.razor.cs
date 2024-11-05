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

namespace FoundryBlazor.Shared;

public class Canvas3DComponentBase : ComponentBase, IDisposable
{
    public ViewerThreeD? ViewerReference;

    [Inject] private IJSRuntime? JSRuntime { get; set; }
    [Inject] public IWorkspace? Workspace { get; set; }
    [Inject] private ComponentBus? PubSub { get; set; }

    [Parameter] public string CanvasStyle { get; set; } = "width:max-content; border:1px solid black;cursor:default";
    [Parameter] public int CanvasWidth { get; set; } = 2500;
    [Parameter] public int CanvasHeight { get; set; } = 4000;

    [Parameter] public ViewerSettings? Settings3D { get; set; }
    [Parameter] public Scene? Scene3D { get; set; }

    private int tick = 0;



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


    public void Dispose()
    {
        "Canvas3DComponentBase Dispose".WriteInfo();

        PubSub!.UnSubscribeFrom<RefreshUIEvent>(OnRefreshUIEvent);
        GC.SuppressFinalize(this);
    }

    public Scene GetActiveScene() 
    { 
        return ViewerReference!.ActiveScene;
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var scene = GetActiveScene();
            var arena = Workspace?.GetArena();
            arena?.SetScene(scene);

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
            if ( GetActiveScene() != null )
                await GetActiveScene().UpdateScene();
            //$"after ThreeJSView3D.UpdateScene() {e.note}".WriteInfo();
        });
    }


    public FoPage2D GetCurrentPage()
    {
        return Workspace!.CurrentPage();
    }

    public async Task RenderFrameOBSOLITE(double fps)
    {
        if (GetActiveScene() == null) 
            return;

        tick++;

        $"Canvas3D RenderFrame {tick} {fps}".WriteInfo();

        Workspace?.PreRender(tick);

        var arena = Workspace?.GetArena();
        if (arena == null) return;

        var stage = arena.CurrentStage();
        if (stage == null) return;

        // $"RenderFrame {tick} {stage.Name} {stage.IsDirty}".WriteError();

        //if you are already rendering then skip it this cycle
        //if (drawing.SetCurrentlyRendering(true)) return;


        await arena.RenderArena(GetActiveScene(), tick, fps);
        //Workspace?.RenderWatermark(Ctx, tick);


        //drawing.SetCurrentlyRendering(false);

        //Workspace?.PostRender(tick);

        if (stage.IsDirty)
        {
            stage.IsDirty = false;
            await GetActiveScene().UpdateScene();
            //$"RenderFrame stage.IsDirty  so... ThreeJSView3D.UpdateScene()  {tick} {stage.Name}".WriteSuccess();
        }
    }


}
