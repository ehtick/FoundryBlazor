using BlazorComponentBus;
using BlazorThreeJS.Enums;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.PubSub;
using FoundryBlazor.Shared;
using FoundryBlazor.Solutions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using FoundryRulesAndUnits.Units;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;

namespace FoundryBlazor.Shape;

public interface IArena: ITreeNode
{
    void SetScene(Scene scene);
    //Task RenderArena(Scene scene, int tick, double fps);
    Task ClearArena();
    Task UpdateArena();
    void SetDoCreate(Action<CanvasMouseArgs> action);

    Task<bool> PreRender(FoGlyph3D glyph);

    FoStage3D SetCurrentStage(FoStage3D stage);
    void AddAction(string name, string color, Action action);

    V AddShape<V>(V shape) where V : FoGlyph3D;
    V RemoveShape<V>(V shape) where V : FoGlyph3D;

T   EstablishStage<T>(string name) where T : FoStage3D;
    IStageManagement Stages();
    List<FoStage3D> GetAllStages();
    FoStage3D CurrentStage();
    Scene CurrentScene();

    //FoWorld3D StressTest3DModelFromFile(string folder, string filename, string baseURL, int count);
    //FoWorld3D Load3DModelFromFile(UDTO_Body spec, string folder, string filename, string baseURL);
    //void SetCanvasSizeInPixels(int width, int height);
    void CreateMenus(IWorkspace space, IJSRuntime js, NavigationManager nav);

}
public class FoArena3D : FoGlyph3D, IArena
{

    public Scene? Scene { get; set; }
    private IStageManagement StageManager { get; set; }
    public ComponentBus PubSub { get; set; }

    public Action<CanvasMouseArgs>? DoCreate { get; set; }

    public FoArena3D(
        IStageManagement manager,
        ComponentBus pubSub)
    {
        StageManager = manager;

        PubSub = pubSub;
    }
    public FoStage3D SetCurrentStage(FoStage3D stage)
    {
        StageManager.SetCurrentStage(stage);
        //stage.InitScene(CurrentScene());
        //PanZoomService.ReadFromPage(page);
        return stage;
    }
    public T EstablishStage<T>(string name) where T : FoStage3D
    {
        var stage = StageManager.EstablishStage<T>(name, this) as T;
        return (T)stage;
    }

    public FoStage3D CurrentStage()
    {
        var stage = StageManager.GetCurrentStage();
        if ( stage == null)
            StageManager.EstablishStage<FoStage3D>("Stage-1", this);

        stage = StageManager.GetCurrentStage()!;
        return stage;
    }
    public IStageManagement Stages()
    {
        return StageManager;
    }
    public List<FoStage3D> GetAllStages()
    {
        return StageManager.GetAllStages();
    }

    public override IEnumerable<ITreeNode> GetTreeChildren()
    {
        var list = new List<ITreeNode>();
        foreach (var item in GetAllStages())
        {
            list.Add(item);
        }

        return list;
    }

    // public void SetCanvasSizeInPixels(int width, int height)
    // {
    //     TrueCanvasWidth = width;
    //     TrueCanvasHeight = height;
    // }

    // public async Task RenderArena(Scene scene, int tick, double fps)
    // {
    //     await StageManager.RenderDetailed(scene, tick, fps);

    //     //if the stage is dirty call to update
    //     //$"Arean Render Scene {tick}".WriteInfo();
    // }

    public V AddShape<V>(V shape) where V : FoGlyph3D
    {
        var stage = CurrentStage();
        var scene = CurrentScene();

        shape.OnDelete = (FoGlyph3D item) =>
        {
            var mesh = item.Value3D();
            if ( mesh != null)
                scene.RemoveChild(mesh);

            stage.RemoveShape<FoGlyph3D>(item);
            PubSub!.Publish<RefreshUIEvent>(new RefreshUIEvent("FoArena3D:RemoveShape"));

        };
        return StageManager.AddShape<V>(shape);
    }

    public V RemoveShape<V>(V shape) where V : FoGlyph3D
    {    
        var scene = CurrentScene();
        var mesh = shape.Value3D();
        if ( mesh != null)
            scene.RemoveChild(mesh);


        var result = StageManager.RemoveShape<V>(shape);
        PubSub!.Publish<RefreshUIEvent>(new RefreshUIEvent("FoArena3D:RemoveShape"));
        return result;
    }

    public async Task ClearArena()
    {

        "ClearArena".WriteInfo();
        var stage = CurrentStage();
        stage.ClearStage();

        var scene = CurrentScene();
        await scene.ClearScene();
    }

    public async Task UpdateArena()
    {
        "UpdateArena".WriteInfo();
        var stage = CurrentStage();
        var scene = CurrentScene();
        await stage.RenderToScene(scene);
    }

    public void SetScene(Scene scene)
    {
        if ( Scene == scene || scene == null)
            return;

        var lastName = Scene?.Name ?? "None";
        Scene = scene;
        $"SetSceneAndViewer {Name} was {lastName} is now: {scene.Title}".WriteSuccess();
    }


    public Scene CurrentScene()
    {
        return Scene!;
    }
    public async Task<bool> PreRender(FoGlyph3D glyph)
    {
        return await glyph.PreRender(this);
    }

    public virtual void CreateMenus(IWorkspace space, IJSRuntime js, NavigationManager nav)
    {
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
            $" DoCreate {action.Method.Name} {ex.Message}".WriteNote();
        }
    }

    public override IEnumerable<TreeNodeAction> GetTreeNodeActions()
    {
        var result = base.GetTreeNodeActions().ToList();
        result.AddAction("Clear", "btn-danger", () =>
        {
            Task.Run(async () => await ClearArena());
        });

        result.AddAction("Update", "btn-success", () =>
        {
           Task.Run(async () => await UpdateArena());
        });
        
        return result;
    }

    //public FoWorld3D StressTest3DModelFromFile(string folder, string filename, string baseURL, int count)
    //{
    //    var name = Path.GetFileNameWithoutExtension(filename);


    //    var world3D = new UDTO_World();
    //    var data = new MockDataMaker();
    //    var url = Path.Join(baseURL, folder, filename);

    //    var root = new DT_Hero();

    //    for (int i = 0; i < count; i++)
    //    {
    //        root.name = $"{name}-{i}";
    //        var shape = world3D.CreateGlb(root, url, 1, 2, 3);
    //        shape.EstablishLoc(data.GenerateDouble(-5, 5), data.GenerateDouble(-5, 5), data.GenerateDouble(-5, 5), "m");
    //        shape.EstablishAng(data.GenerateDouble(0, 360), data.GenerateDouble(0, 360), data.GenerateDouble(0, 360), "r");
    //    };


    //    var world = new FoWorld3D(world3D);
    //    RenderWorld3D(world);

    //    //PostRenderplatform

    //    return world;
    //}



    //public FoWorld3D Load3DModelFromFile(UDTO_Body spec, string folder, string filename, string baseURL)
    //{
    //    var name = Path.GetFileNameWithoutExtension(filename);

    //    var url = Path.Join(baseURL, folder, filename);

    //    var root = new DT_Hero
    //    {
    //        name = name,
    //        guid = spec.uniqueGuid,
    //    };


    //    var world3D = new UDTO_World();
    //    var body = world3D.CreateGlb(root, url);
    //    body.boundingBox = spec.boundingBox;
    //    body.position = spec.position;

    //    var world = new FoWorld3D(world3D);
    //    RenderWorld3D(world);

    //    return world;
    //}



    public bool RenderDrawingToScene(IDrawing drawing)
    {
        if (Scene == null)
            return false;

        var north = new FoPanel3D("-Z Wall")
        {
            Position = new Vector3(0, 7, -14.8),
            Rotation = new Euler(0, Math.PI * 0 / 180, 0),
        };
        var south = new FoPanel3D("+Z Wall")
        {
            Position = new Vector3(0, 7, 14.8),
            Rotation = new Euler(0, Math.PI * 180 / 180, 0),
        };
        var east = new FoPanel3D("-X Wall")
        {
            Position = new Vector3(-14.8, 7, 0),
            Rotation = new Euler(0, Math.PI * 90 / 180, 0),
        };
        var west = new FoPanel3D("+X Wall")
        {
            Position = new Vector3(14.8, 7, 0),
            Rotation = new Euler(0, Math.PI * 270 / 180, 0),
        };
        //need to convert pixels to meters
        //Conversion(5000, "px", 1, "m");

        var queue = new Queue<FoPanel3D>();
        queue.Enqueue(north);
        queue.Enqueue(east);
        queue.Enqueue(west);
        queue.Enqueue(south);


        var pixels = 100;
        var z = 0.1;
        foreach (var page in drawing.GetAllPages())
        {

            var shapes = page.AllShapes2D();
            if (shapes.Count == 0)
                continue;

            var wall = queue.Dequeue();
            wall.Width = 16;
            wall.Height = 16;
            wall.Color = page.Color;
            var halfW = wall.Width / 2;
            var halfH = wall.Height / 2;

            shapes?.ForEach(shape =>
            {
                var w = (double)shape.Width / pixels;
                var h = (double)shape.Height / pixels;
                var x = (double)shape.PinX / pixels;
                var y = (double)shape.PinY / pixels;
                var panel = new FoPanel3D(shape.Key)
                {
                    Width = w,
                    Height = h,
                    Color = shape.Color,
                    // Position = Placement(page.Name, wall.Position!, x, y),
                    Position = new Vector3(x - halfW, halfH - y, z),
                };
                var textLines = shape.GetText().Split('_').ToList();
                panel.TextLines.AddRange(textLines);

                wall.Add<FoPanel3D>(panel);
            });

            var lineShapes = page.AllShapes1D();
            var pathZ = z - 0.1;

            foreach (var lineShape in lineShapes)
            {
                var X1 = lineShape.StartX / pixels;
                var Y1 = lineShape.StartY / pixels;
                var X2 = lineShape.FinishX / pixels;
                var Y2 = lineShape.FinishY / pixels;
                var path = new List<Vector3>() {
                    new Vector3(X1 - halfW, halfH - Y1, pathZ),
                    new Vector3(X2 - halfW, halfH - Y2, pathZ)
                };
                var pathway = new FoPathway3D(lineShape.GetName())
                {
                    Path = path,
                    Color = "black"
                };

                wall.Add(pathway);
            }

            wall.Render(Scene, 0, 0);
        }
        return true;
    }

    // public void RenderWorld3D(IWorld3D world)
    // {
    //     if (world == null) return;

    //     $"RenderWorld {world.GetTreeNodeTitle()}".WriteNote();

    //     Task.Run(async () =>
    //     {
    //         await PreRenderWorld3D(world);
    //         RenderWorld3DToScene(world);
    //         await UpdateArena();
    //     });
    // }


    // public async Task PreRenderWorld3D(IWorld3D world)
    // {
    //     //$"PreRenderWorld world={world}".WriteInfo();
    //     if (world == null)
    //     {
    //         $"world is empty or viewer is not preent".WriteError();
    //         return;
    //     }

    //     var bodies = world.ShapeBodies();
    //     if (bodies != null)
    //         await PreRenderGLBClones(bodies);
    // }


   



    public async Task PreRenderGLBClones(List<FoShape3D> shapes)
    {
        var glbBodies = shapes.Where((body) => body.Type.Matches("Glb")).ToList();
        var otherBodies = shapes.Where((body) => !body.Type.Matches("Glb")).ToList();

        var bodyDict = glbBodies
            .GroupBy(item => item.Url)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var keyValuePair in bodyDict)
        {
            await FoShape3D.PreRenderClones(keyValuePair.Value, this,  Import3DFormats.Gltf);
        }

        foreach (var body in otherBodies)
        {
            //$"PreRenderPlatform Body {body.Name}".WriteInfo();
            await body.PreRender(this);
        }

    }




}
