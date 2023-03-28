using BlazorComponentBus;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Lights;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Scenes;
using BlazorThreeJS.Settings;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Canvas;
using FoundryBlazor.Extensions;
using FoundryBlazor.PubSub;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FoundryBlazor.Shape;

public interface IArena
{
    FoScene3D CurrentScene();

    void RefreshUI();
    void SetViewer(Viewer viewer);
    Task RenderScene(Scene scene, int tick, double fps);

    void SetDoCreate(Action<CanvasMouseArgs> action);

    void RenderWorld(FoWorld3D? world);
    void PostRender(Guid guid);


    FoGroup3D MakeAndRenderTestPlatform();

    List<IFoMenu> CollectMenus(List<IFoMenu> list);
    FoMenu3D EstablishMenu<T>(string name, Dictionary<string, Action> menu, bool clear) where T : FoMenu3D;
    void CreateMenus(IJSRuntime js, NavigationManager nav);
}
public class FoArena3D : FoGlyph3D, IArena
{
    public Viewer? Viewer3D { get; set; }
    private ISceneManagement SceneManager { get; set; }
    private IScaledArena ScaledArena { get; set; }

    public ComponentBus PubSub { get; set; }

    public Action<CanvasMouseArgs>? DoCreate { get; set; }



    public FoArena3D(
        IScaledArena scaled,
        ISceneManagement sceneManagement,
        ComponentBus pubSub)
    {
        SceneManager = sceneManagement;
        PubSub = pubSub;
        ScaledArena = scaled;
    }

    public FoScene3D CurrentScene()
    {
        return SceneManager.CurrentScene();
    }

    public Scene ThreeJSScene()
    {
        return CurrentScene().GetScene();
    }

    public async Task RenderScene(Scene scene, int tick, double fps)
    {
        await Task.CompletedTask;
        //$"Arean Render Scene {tick}".WriteInfo();
    }
    public Scene InitScene()
    {
        var scene = ThreeJSScene();
        if (scene != null)
        {
            scene.Add(new AmbientLight());
            scene.Add(new PointLight()
            {
                Position = new Vector3(1, 3, 0)
            });
            CurrentScene().EstablishBoundry();
            RefreshUI();
            "InitScene".WriteInfo();
        }

        return ThreeJSScene();
    }

    // public async Task ClearViewer3D()
    // {
    //     "ClearViewer3D".WriteInfo();
    //     if (Viewer3D != null)
    //         await Viewer3D.ClearSceneAsync();
    // }

    public void SetViewer(Viewer viewer)
    {
        Viewer3D = viewer;
    }
    public List<IFoMenu> CollectMenus(List<IFoMenu> list)
    {
        return SceneManager.CollectMenus(list);
    }

    public FoMenu3D EstablishMenu<T>(string name, Dictionary<string, Action> menu, bool clear) where T : FoMenu3D
    {
        var result = SceneManager.EstablishMenu3D<T, FoButton3D>(name, menu, clear);
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
        FillScene();
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

        var scene = CurrentScene().GetScene();
        $"scene={scene}".WriteInfo();

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

        var scene = CurrentScene().GetScene();
        $"scene={scene}".WriteInfo();


        platform.Bodies()?.ForEach(body =>
        {
            //$"RenderPlatformToScene Body Name={body.Name}, Type={body.Type}".WriteInfo();
            body.Render(scene, 0, 0);
        });

        platform.Labels()?.ForEach(label =>
        {
            //$"RenderPlatformToScene Label Name={label.Name}, Text={label.Text}".WriteInfo();
            label.Render(scene, 0, 0);
        });

        platform.Datums()?.ForEach(datum =>
        {
            //$"RenderPlatformToScene Datum {datum.Name}".WriteInfo();
            datum.Render(scene, 0, 0);
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

        var scene = CurrentScene().GetScene();
        $"scene={scene}".WriteInfo();


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
    private void FillScene()
    {
        var scene = CurrentScene().GetScene();
        scene.Add(new AmbientLight());
        scene.Add(new PointLight()
        {
            Position = new Vector3(1, 3, 0)
        });
        scene.Add(new Mesh());
        scene.Add(new Mesh
        {
            Geometry = new BoxGeometry(width: 1.2f, height: 0.5f),
            Position = new Vector3(-2, 0, 0),
            Material = new MeshStandardMaterial()
            {
                Color = "magenta"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new CircleGeometry(radius: 0.75f, segments: 12),
            Position = new Vector3(2, 0, 0),
            Scale = new Vector3(1, 0.75f, 1),
            Material = new MeshStandardMaterial()
            {
                Color = "#98AFC7"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new CapsuleGeometry(radius: 0.5f, length: 2),
            Position = new Vector3(-4, 0, 0),
            Material = new MeshStandardMaterial()
            {
                Color = "darkgreen"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new ConeGeometry(radius: 0.5f, height: 2, radialSegments: 16),
            Position = new Vector3(4, 0, 0),
            Material = new MeshStandardMaterial()
            {
                Color = "green",
                FlatShading = true,
                Metalness = 0.5f,
                Roughness = 0.5f
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new CylinderGeometry(radiusTop: 0.5f, height: 1.2f, radialSegments: 16),
            Position = new Vector3(0, 0, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "red",
                Wireframe = true
            }
        });
        scene.Add(new Mesh
        {
            Geometry = new DodecahedronGeometry(radius: 0.8f),
            Position = new Vector3(-2, 0, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "darkviolet",
                Metalness = 0.5f,
                Roughness = 0.5f
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new IcosahedronGeometry(radius: 0.8f),
            Position = new Vector3(-4, 0, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "violet"
            }
        });

        scene.Add(new Mesh
        {

            Geometry = new OctahedronGeometry(radius: 0.75f),
            Position = new Vector3(2, 0, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "aqua"
            }
        });

        scene.Add(new Mesh
        {
            Geometry = new PlaneGeometry(width: 0.5f, height: 2),
            Position = new Vector3(4, 0, -2),
            Material = new MeshStandardMaterial()
            {
                Color = "purple"
            }
        });
        scene.Add(new Mesh
        {
            Geometry = new RingGeometry(innerRadius: 0.6f, outerRadius: 0.7f),
            Position = new Vector3(0, 0, -4),
            Material = new MeshStandardMaterial()
            {
                Color = "DodgerBlue"
            }
        });
        scene.Add(new Mesh
        {
            Geometry = new SphereGeometry(radius: 0.6f),
            Position = new Vector3(-2, 0, -4),
            Material = new MeshStandardMaterial()
            {
                Color = "darkgreen"
            },
        });
        scene.Add(new Mesh
        {
            Geometry = new TetrahedronGeometry(radius: 0.75f),
            Position = new Vector3(2, 0, -4),
            Material = new MeshStandardMaterial()
            {
                Color = "lightblue"
            }
        });
        scene.Add(new Mesh
        {
            Geometry = new TorusGeometry(radius: 0.6f, tube: 0.4f, radialSegments: 12, tubularSegments: 12),
            Position = new Vector3(4, 0, -4),
            Material = new MeshStandardMaterial()
            {
                Color = "lightgreen"
            }
        });
        scene.Add(new Mesh
        {
            Geometry = new TorusKnotGeometry(radius: 0.6f, tube: 0.1f),
            Position = new Vector3(-4, 0, -4),
            Material = new MeshStandardMaterial()
            {
                Color = "RosyBrown"
            }
        });
    }


}