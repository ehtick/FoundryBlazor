using BlazorComponentBus;
using BlazorThreeJS.Scenes;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Canvas;
using FoundryBlazor.Extensions;
using FoundryBlazor.PubSub;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FoundryBlazor.Shape;

public interface IArena
{

    void RefreshUI();
    void SetViewer(Viewer viewer, Scene scene);
    Task RenderArena(Scene scene, int tick, double fps);

    void SetDoCreate(Action<CanvasMouseArgs> action);

    void RenderWorld(FoWorld3D? world);
    void PostRender(Guid guid);

    FoStage3D CurrentStage();
    FoGroup3D MakeAndRenderTestPlatform();

    List<IFoMenu> CollectMenus(List<IFoMenu> list);
    FoMenu3D EstablishMenu<T>(string name, Dictionary<string, Action> menu, bool clear) where T : FoMenu3D;
    void CreateMenus(IJSRuntime js, NavigationManager nav);
}
public class FoArena3D : FoGlyph3D, IArena
{
    public Viewer? Viewer3D { get; set; }
    private IStageManagement StageManager { get; set; }
    private IScaledArena ScaledArena { get; set; }

    public ComponentBus PubSub { get; set; }

    public Action<CanvasMouseArgs>? DoCreate { get; set; }



    public FoArena3D(
        IScaledArena scaled,
        IStageManagement manager,
        ComponentBus pubSub)
    {
        StageManager = manager;
        PubSub = pubSub;
        ScaledArena = scaled;
    }

    public FoStage3D CurrentStage()
    {
        var stage = StageManager.CurrentStage();
        return stage;
    }

    public async Task RenderArena(Scene scene, int tick, double fps)
    {
        var stage = StageManager.CurrentStage();
        stage.IsDirty = true;
        await StageManager.RenderDetailed(scene, tick, fps);

        //if the stage is dirty call to update
        //$"Arean Render Scene {tick}".WriteInfo();
    }

    // public async Task ClearViewer3D()
    // {
    //     "ClearViewer3D".WriteInfo();
    //     if (Viewer3D != null)
    //         await Viewer3D.ClearSceneAsync();
    // }

    public void SetViewer(Viewer viewer, Scene scene)
    {
        Viewer3D = viewer;
        CurrentStage().InitScene(scene);
    }
    public List<IFoMenu> CollectMenus(List<IFoMenu> list)
    {
        return StageManager.CollectMenus(list);
    }

    public FoMenu3D EstablishMenu<T>(string name, Dictionary<string, Action> menu, bool clear) where T : FoMenu3D
    {
        var result = StageManager.EstablishMenu3D<T, FoButton3D>(name, menu, clear);
        return result;
    }

    public virtual void CreateMenus(IJSRuntime js, NavigationManager nav)
    {
        EstablishMenu<FoMenu3D>("Main", new Dictionary<string, Action>()
        {
            //{ "Clear", () => PageManager?.ClearAll()},
            //{ "Group", () => PageManager?.GroupSelected<FoGroup2D>()},
            //{ "Ungroup", () => PageManager.UngroupSelected<FoGroup2D>()},
            //{ "Save", () => Command?.Save()},
            //{ "Restore", () => Command?.Restore()},
            //{ "Pan Zoom", () => TogglePanZoomWindow()},
        }, true);

    }

    public void SetDoCreate(Action<CanvasMouseArgs> action)
    {

        try
        {
            DoCreate = action;

            DoCreate?.Invoke(new CanvasMouseArgs()
            {
                OffsetX = 0,
                OffsetY = 0
            });

            //var region = _helper.UserWindow();
            //page.ComputeShouldRender(region);
        }
        catch (System.Exception ex)
        {
            $" DoCreate {action.Method.Name} {ex.Message}".WriteLine();
        }
    }

    public void RefreshUI()
    {
        PubSub!.Publish<RefreshUIEvent>(new RefreshUIEvent());
    }

    public FoGroup3D MakeAndRenderTestPlatform()
    {

        var platform = new FoGroup3D()
        {
            GlyphId = Guid.NewGuid().ToString(),
            PlatformName = "RonTest",
            Name = "RonTest"
        };
        platform.EstablishBox("Platform", 1, 1, 1);

        var largeBlock = platform.CreateUsing<FoShape3D>("LargeBlock").CreateBox("Large", 3, 1, 2);
        largeBlock.Position = new FoVector3D();

        var smallBlock = platform.CreateUsing<FoShape3D>("SmallBlock").CreateBox("SmallBlock", 1.5, .5, 1);
        smallBlock.Position = new FoVector3D()
        {
            X = -2.25,  //might need to changes sign
            Y = 0.75,
            Z = 1.5,
        };

        platform.CreateUsing<FoText3D>("Label-1").CreateTextAt("Hello", -1.0, 2.0, 1.0)
            .Position = new FoVector3D();


        RenderPlatformToScene(platform);

        return platform;
    }

    public void RenderWorld(FoWorld3D? world)
    {
        if (world == null) return;
        world.FillPlatforms();

        $"RenderWorld {world.Name}".WriteLine(ConsoleColor.Blue);


        PreRenderWorld(world);
        RenderWorldToScene(world);
        RefreshUI();
    }



    public void RenderWorldToScene(FoWorld3D? world)
    {
        $"world={world}".WriteInfo();
        if (world == null)
        {
            $"world is empty or viewer is not present".WriteError();
            return;
        }


        //await ClearViewer3D();
        //$"cleared scene".WriteInfo();

        $"Platforms Count={world.Platforms()?.Count}".WriteInfo();
        var platforms = world.Platforms();
        if (platforms != null)
        {
            foreach (var platform in platforms)
            {
                RenderPlatformToScene(platform);
            }
        }
    }

    public void RenderPlatformToScene(FoGroup3D? platform)
    {
        //await ClearViewer3D();

        $"RenderPlatformToScene PlatformName={platform?.PlatformName}".WriteInfo();

        $"platform={platform}".WriteInfo();
        if (platform == null)
        {
            $"platform is empty or viewer is not present".WriteError();
            return;
        }




        platform.Bodies()?.ForEach(body =>
        {
            //$"RenderPlatformToScene Body Name={body.Name}, Type={body.Type}".WriteInfo();
            //body.Render(scene, 0, 0);
        });

        platform.Labels()?.ForEach(label =>
        {
            //$"RenderPlatformToScene Label Name={label.Name}, Text={label.Text}".WriteInfo();
            //label.Render(scene, 0, 0);
        });

        platform.Datums()?.ForEach(datum =>
        {
            //$"RenderPlatformToScene Datum {datum.Name}".WriteInfo();
            //datum.Render(scene, 0, 0);
        });

        RefreshUI();
    }

    public void PreRenderWorld(FoWorld3D? world)
    {
        $"PreRenderWorld world={world}".WriteInfo();
        if (world == null )
        {
            $"world is empty or viewer is not preent".WriteError();
            return;
        }

        world.Platforms()?.ForEach(PreRenderPlatform);
    }

    public void PreRenderPlatform(FoGroup3D? platform)
    {
        $"PreRenderPlatform platform={platform}".WriteInfo();
        if (platform == null)
        {
            $"platform is empty or viewer is not present".WriteError();
            return;
        }

        //var scene = CurrentStage().GetScene();
        //$"scene={scene}".WriteInfo();


        platform.Bodies()?.ForEach(body =>
        {
            $"PreRenderPlatform Body {body.Name}".WriteInfo();
            body.PreRender(this, Viewer3D!);
        });
    }

    public void PostRender(Guid guid)
    {
        var shape = Find<FoShape3D>(guid.ToString());
        if (shape != null)
        {

            //var removeGuid = shape.LoadingGUID ?? Guid.NewGuid();
            //await Viewer3D!.RemoveByUuidAsync(removeGuid);
            shape.PromiseGUID = null;
            shape.LoadingGUID = null;

        }
        else
        {
            $"Did not find Shape guid={guid}".WriteError();
        }
    }

}