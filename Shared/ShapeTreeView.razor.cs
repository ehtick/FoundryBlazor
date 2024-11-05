using BlazorComponentBus;
using Microsoft.AspNetCore.Components;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using FoundryBlazor.Solutions;
using FoundryBlazor.Shape;
using BlazorThreeJS.Viewers;



namespace FoundryBlazor.Shared;

public partial class ShapeTreeBase : ComponentBase
{

    //[Inject] private ComponentBus? PubSub { get; set; }
    [Inject] private IFoundryService? Service { get; set; }

    public List<ITreeNode> AllNodes = new();

    private bool _addScene = false;
    private bool _addWorld = false;

    public IEnumerable<ITreeNode> GetAllNodes()
    {
        if (Service != null)
        {
            var worlds = Service.WorldManager().AllWorlds();

            AllNodes.Clear();

            if ( _addWorld && worlds.Count > 0)
            {
                var folder = new FoFolder("Worlds");
                AllNodes.Add(folder);
                worlds.ForEach(item => folder.AddChild(item));
            }

            AllNodes.Add(Service.Drawing());
            AllNodes.Add(Service.Arena());
            //var scene = Service.Arena().CurrentScene();

            if ( _addScene )
            {
                var folder = new FoFolder("Scenes");
                AllNodes.Add(folder);
                var list = Scene.GetAllScenes();
                $"{list.Count} Scenes".WriteSuccess();

                foreach (var item in list)
                {
                    folder.AddTreeNode(item);
                }
            }

            
        }

        return AllNodes;
    }

    protected void AddScene()
    {
        _addScene = !_addScene;
        Refresh();
    }

    protected void AddWorld()
    {
        _addWorld = !_addWorld;
        var worlds = Service!.WorldManager().AllWorlds();
        if ( _addWorld && worlds.Count == 0)
        {
            Service.WorldManager().EstablishWorld("World 616");
        }
        Refresh();
    }




//https://learn.microsoft.com/en-us/semantic-kernel/overview/


    protected void Refresh()
    {
        //var nodes = GetAllNodes().Where( n => n != null).ToList();

        GetAllNodes().ForEach(n => n?.SetExpanded(true));
        Task.Run(() => {
            Thread.Sleep(200);
            //KnBase.RefreshTree = true;
            InvokeAsync(StateHasChanged);
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Refresh();
            //PubSub!.SubscribeTo<RefreshRenderMessage>(OnRenderRefresh);

       
        }

        await base.OnAfterRenderAsync(firstRender);
    }



}