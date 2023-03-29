using BlazorThreeJS.Scenes;
using FoundryBlazor.Extensions;

namespace FoundryBlazor.Shape;

public interface IStageManagement
{

    FoStage3D CurrentStage();
    FoStage3D SetCurrentStage(FoStage3D page);
    FoStage3D AddStage(FoStage3D page);
    List<IFoMenu> CollectMenus(List<IFoMenu> list);

    void ClearAll();
    int StageCount();

    Task RenderDetailed(Scene scene, int tick, double fps);

    //T Add<T>(T value) where T : FoGlyph3D;
    //T Duplicate<T>(T value) where T : FoGlyph3D;
    //U MorphTo<T, U>(T value) where T : FoGlyph3D where U : FoGlyph3D;
    //T? GroupSelected<T>() where T : FoGlyph3D;
    U EstablishMenu3D<U, T>(string name, Dictionary<string, Action> actions, bool clear) where T : FoButton3D where U : FoMenu3D;
}


public class StageManagementService : FoComponent, IStageManagement
{

    private bool RenderHitTestTree = false;
    private FoStage3D ActiveStage { get; set; }
    //private readonly FoCollection<FoStage3D> Stages = new();
    private readonly IHitTestService _hitTestService;
    private readonly ISelectionService _selectService;
    private readonly IScaledArena _ScaledArena;

    public StageManagementService
    (
        IHitTestService hit,
        IScaledArena scaled,
        ISelectionService sel)
    {
        _hitTestService = hit;
        _selectService = sel;
        _ScaledArena = scaled;

        ActiveStage = CurrentStage();
    }



    public int StageCount()
    {
        return Members<FoStage3D>().Count;
    }

    public async Task RenderDetailed(Scene scene, int tick, double fps)
    {
        await CurrentStage().RenderDetailed(scene, tick, fps);
    }



    public void ClearAll()
    {
        FoGlyph2D.ResetHitTesting = true;
       // CurrentStage().ClearAll();
    }

    public bool ToggleHitTestRender()
    {
        FoGlyph2D.ResetHitTesting = true;
        RenderHitTestTree = !RenderHitTestTree;
        return RenderHitTestTree;
    }


    public List<IFoMenu> CollectMenus(List<IFoMenu> list)
    {
        var stage = CurrentStage();
        var items = stage.GetMembers<FoMenu3D>();
        if ( items != null)
            list.AddRange(items);
        return list;
    }   



     public T AddShape<T>(T value) where T : FoGlyph3D
    {
        var found = CurrentStage().AddShape(value);
        //if ( found != null)
        //    _hitTestService.Insert(value);

        return found!;

    }

    public FoStage3D CurrentStage()
    {
        if (ActiveStage == null)
        {
            var found = Members<FoStage3D>().Where(page => page.IsActive).FirstOrDefault();
            if (found == null)
            {
                found = new FoStage3D("Stage-1",10,10,10,"Red");
                found.SetScaledArena(_ScaledArena);
                AddStage(found);
            }
            ActiveStage = found;
            ActiveStage.IsActive = true;
        }

        return ActiveStage;
    }
    public FoStage3D SetCurrentStage(FoStage3D page)
    {
        ActiveStage = page;
        Members<FoStage3D>().ForEach(item => item.IsActive = false);
        ActiveStage.IsActive = true;
        return ActiveStage!;
    }

    public FoStage3D AddStage(FoStage3D scene)
    {
        var found = Members<FoStage3D>().Where(item => item == scene).FirstOrDefault();
        if (found == null)
            Add(scene);
        return scene;
    }

 

    public U MorphTo<T, U>(T value) where T : FoGlyph3D where U : FoGlyph3D
    {
        var body = StorageHelpers.Dehydrate<T>(value, false);
        var shape = StorageHelpers.Hydrate<U>(body, false);

        shape!.Name = "";
        //shape!.GlyphId = "";

        return Add<U>(shape);
    }



    public void ClearMenu3D<U>(string name) where U : FoMenu3D
    {
        var stage = CurrentStage();
        var menu = stage.Find<U>(name);
        menu?.Clear();
    }

    public U EstablishMenu3D<U, T>(string name, Dictionary<string, Action> actions, bool clear) where T : FoButton3D where U : FoMenu3D
    {
        var stage = CurrentStage();
        var menu = stage.EstablishMenu3D<U>(name, clear);

        foreach (KeyValuePair<string, Action> item in actions)
        {
            if (Activator.CreateInstance(typeof(T), item.Key, item.Value) is T shape)
                menu.Add<T>(shape);
        }

        //menu.LayoutHorizontal();

        return menu;
    }








}