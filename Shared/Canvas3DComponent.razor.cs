using BlazorComponentBus;
using BlazorThreeJS.Events;
using BlazorThreeJS.Scenes;
using BlazorThreeJS.Settings;
using BlazorThreeJS.Viewers;

using FoundryBlazor.Canvas;
using FoundryBlazor.Extensions;
using FoundryBlazor.PubSub;
using FoundryBlazor.Solutions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FoundryBlazor.Shared;

public class Canvas3DComponentBase : ComponentBase, IDisposable
{
    
    public Viewer ThreeJSView3D = null!;
    private ViewerSettings? Settings { get; set; }
    private Scene? ActiveScene { get; set; }


    [Inject] public IWorkspace? Workspace { get; set; }
    [Inject] private ComponentBus? PubSub { get; set; }
    [Inject] protected IJSRuntime? JsRuntime { get; set; }

    [Parameter] public string StyleCanvas { get; set; } = "position: absolute; top: 80px; left: 0px; z-index: 10";
    [Parameter] public int CanvasWidth { get; set; } = 2500;
    [Parameter] public int CanvasHeight { get; set; } = 4000;
    private int tick = 0;

    public AnimationHelper? AnimationHelperReference;


    public ViewerSettings GetSettings()
    {
        return Settings!;
    }

    public Scene GetActiveScene()
    {
        return ActiveScene!;
    }

    public void Dispose()
    {
        ActiveScene = null;
        //Dispose(true);

        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SupressFinalize to
        // take this object off the finalization queue 
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        ActiveScene = new Scene();
        Settings = new()
        {
            CanSelect = true,// default is false
            SelectedColor = "black",
            WebGLRendererSettings = new WebGLRendererSettings
            {
                Antialias = false // if you need poor quality for some reasons
            }
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            await AnimationHelperReference!.Initialize();
            PubSub!.SubscribeTo<RefreshUIEvent>(OnRefreshUIEvent);
            ThreeJSView3D.ObjectLoaded += OnObjectLoaded;

            var arena = Workspace?.GetArena();
            arena?.SetViewer(ThreeJSView3D,ActiveScene!);
            //await ThreeJSView3D.UpdateScene();

           // $"OnAfterRenderAsync Viewer={View3D1}".WriteInfo();
        }
        await base.OnAfterRenderAsync(firstRender);
    }


    private void OnRefreshUIEvent(RefreshUIEvent e)
    {
        InvokeAsync(StateHasChanged);
        $"ThreeJSView3D.UpdateScene()".WriteInfo();
        Task.Run(async () => await ThreeJSView3D.UpdateScene());
    }
    public async Task OnObjectLoaded(Object3DArgs e)
    {
        var arena = Workspace?.GetArena();
        arena?.PostRender(e.UUID);
        //await Task.CompletedTask;
        await ThreeJSView3D.UpdateScene();
    }


    public async Task RenderFrame(double fps)
    {
        if (ActiveScene == null) return;
        tick++;

        //Workspace?.PreRender(tick);

        var arena = Workspace?.GetArena();
        if (arena == null) return;

        var stage = arena.CurrentStage(); 
        if (stage == null || !stage.IsDirty ) return;

        $"RenderFrame {tick} {stage.Name} {stage.IsDirty}".WriteError();

        //if you are already rendering then skip it this cycle
        //if (drawing.SetCurrentlyRendering(true)) return;


        await arena.RenderArena(ActiveScene, tick, fps);
        //Workspace?.RenderWatermark(Ctx, tick);


        //drawing.SetCurrentlyRendering(false);

        //Workspace?.PostRender(tick);

        await ThreeJSView3D.UpdateScene();
    }



}