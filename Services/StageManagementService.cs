using BlazorThreeJS.Scenes;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;

namespace FoundryBlazor.Shape;

public interface IStageManagement
{

    FoStage3D EstablishStage(Scene scene, Viewer viewer, string name="Stage-1");
    FoStage3D SetCurrentStage(FoStage3D page);
    FoStage3D AddStage(FoStage3D page);

    V AddShape<V>(V shape) where V : FoGlyph3D;
    V RemoveShape<V>(V shape) where V : FoGlyph3D;
    void ClearAll();
    int StageCount();
    List<FoStage3D> GetAllStages();

    Task RenderDetailed(Scene scene, int tick, double fps);

    //T Add<T>(T value) where T : FoGlyph3D;
    //T Duplicate<T>(T value) where T : FoGlyph3D;
    //U MorphTo<T, U>(T value) where T : FoGlyph3D where U : FoGlyph3D;
    //T? GroupSelected<T>() where T : FoGlyph3D;
 }


public class StageManagementService : FoComponent, IStageManagement
{

    private bool RenderHitTestTree = false;
    private readonly IHitTestService _hitTestService;
    private readonly ISelectionService _selectService;

    private FoStage3D? _stage;
    public FoStage3D ActiveStage
    {
        get
        {
            if (_stage?.IsActive != true)
                $"Get Active Stage {_stage?.Key} is broken".WriteInfo();

            return _stage!;
        }
        set
        {
            GetAllStages().ForEach(stage => stage.IsActive = false);
            _stage = value;
            _stage.IsActive = true;
        }
    }

    public StageManagementService
    (
        IHitTestService hit,
        ISelectionService sel)
    {
        _hitTestService = hit;
        _selectService = sel;

    }


    public List<FoStage3D> GetAllStages()
    {
        return Members<FoStage3D>();
    }
    public int StageCount()
    {
        return Members<FoStage3D>().Count;
    }

    public async Task RenderDetailed(Scene scene, int tick, double fps)
    {
        await ActiveStage.RenderDetailed(scene, tick, fps);
    }



    public void ClearAll()
    {
        FoGlyph2D.ResetHitTesting(true,"ClearAll");
        ActiveStage.ClearAll();
    }

    public bool ToggleHitTestRender()
    {
        RenderHitTestTree = !RenderHitTestTree;
        return RenderHitTestTree;
    }




     public T AddShape<T>(T value) where T : FoGlyph3D
    {
        var found = ActiveStage.AddShape(value);
        return found!;
    }

    public T RemoveShape<T>(T value) where T : FoGlyph3D
    {
        var found = ActiveStage.RemoveShape(value);
        //if ( found != null)
        //    _hitTestService.Insert(value);

        return found!;
    }

    public FoStage3D EstablishStage(Scene scene, Viewer viewer, string name="Stage-1")
    {
        if (ActiveStage == null)
        {
            var found = Members<FoStage3D>().Where(stage => stage.IsActive).FirstOrDefault();
            if (found == null)
            {
                found = new FoStage3D(name,10,10,10,"Red");
                found.InitScene(scene,viewer);
                AddStage(found);
            }
            return SetCurrentStage(found);
        }

        return ActiveStage;
    }

    public FoStage3D SetCurrentStage(FoStage3D stage)
    {
        if (_stage == stage && _stage.IsActive)
            return _stage;

        ActiveStage = stage;

        //force refresh of hit testing
        FoGlyph2D.ResetHitTesting(true);
        return ActiveStage;
    }

    public FoStage3D AddStage(FoStage3D stage)
    {
        var found = Members<FoStage3D>().Where(item => item == stage).FirstOrDefault();
        if (found == null)
        {
            Slot<FoStage3D>().Add(stage);
        }
        return stage;
    }

     public FoStage3D RemoveStage(FoStage3D stage)
    {
        var found = Members<FoStage3D>().Where(item => item == stage).FirstOrDefault();
        if (found != null)
        {
            Slot<FoStage3D>().Remove(found);
            if (found == _stage)
            {
                found = Members<FoStage3D>().FirstOrDefault();
                SetCurrentStage(found!);
            }
        }
        return stage;
    }

    public FoStage3D? FindStage(string name)
    {
        var found = Members<FoStage3D>().Where(item => item.Key.Matches(name)).FirstOrDefault();
        return found;
    }

    public T Duplicate<T>(T value) where T : FoGlyph3D
    {
        var body = CodingExtensions.Dehydrate<T>(value, false);
        var shape = CodingExtensions.Hydrate<T>(body, false);

        shape.Key = "";
        shape.GlyphId = "";

        //SRS write a method to duplicate actions
        shape.ShapeDraw = value.ShapeDraw;
        shape.OpenCreater = value.OpenCreater;
        shape.OpenEditor = value.OpenEditor;
        shape.OpenViewer = value.OpenViewer;

        AddShape<T>(shape);
        return shape;
    }

    public U MorphTo<T, U>(T value) where T : FoGlyph3D where U : FoGlyph3D
    {
        var body = CodingExtensions.Dehydrate<T>(value, false);
        var shape = CodingExtensions.Hydrate<U>(body, false);

        shape!.Key = "";
        //shape!.GlyphId = "";

        return Add<U>(shape);
    }



    public void ClearMenu3D<U>(string name) where U : FoMenu3D
    {

        var menu = ActiveStage.Find<U>(name);
        menu?.Clear();
    }

    public virtual async Task Draw(Scene ctx, int tick)
    {
        await ActiveStage.Draw(ctx, tick);
    }
}