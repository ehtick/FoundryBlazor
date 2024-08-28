using BlazorComponentBus;
using Microsoft.AspNetCore.Components;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using FoundryBlazor.Solutions;

namespace FoundryBlazor.Shared;

public partial class ShapeTreeBase : ComponentBase
{

    [Inject] private ComponentBus? PubSub { get; set; }
    [Inject] private IFoundryService? Service { get; set; }

    public List<ITreeNode> AllNodes = new();

    public IEnumerable<ITreeNode> GetAllNodes()
    {
        if (Service != null)
        {
            AllNodes.Clear();
            AllNodes.Add(Service.Drawing());
            AllNodes.Add(Service.Arena());
        }

        return AllNodes;
    }

    // public async Task<List<UDTO_File>> SaveToStaticFiles()
    // {
    //     RestMentor!.SetServerURL(Navigation?.BaseUri ?? "");

    //     var wrap = await RestMentor!.UploadToStaticFiles(FilesToUpload, "Testing");
    //     return wrap.payload.ToList();
    // }

    // public async Task OnUploadFilesInputChange(InputFileChangeEventArgs e)
    // {
    //     var files = e.GetMultipleFiles();

    //     foreach (var file in files)
    //     {
    //         FilesToUpload.Add(file);
    //     }

    //     var ServerFiles = await SaveToStaticFiles();
    //     var AssetFiles = ServerFiles!.Select(f => new DT_AssetFile { filename = f.filename, url = f.url }).ToList();

        
    //     foreach (var item in AssetFiles)
    //     {
    //         var filename = item.filename ?? "";
    //         var url = item.url ?? "";
    //         $"OnUploadFilesInputChange {filename} {url}".WriteInfo();

    //         var result = await RestMentor!.DownloadAsset(item);
    //         if ( result == null || result.hasError )
    //             continue;
            
    //         var file = result.payload.FirstOrDefault();
    //         if (file == null)
    //             continue;
            
    //         var json = file.source;
    //         var model = CodingExtensions.Hydrate<DrawingPersist>(json, false);
    //         model.RestoreDrawing(MentorManager!, Workspace!.GetDrawing(), PubSub!);
    //     }
    //     //MentorManager?.RestoreModel();

    //     //var json = StorageHelpers.ReadData("Storage", filename);
    //     //var model = CodingExtensions.Hydrate<KnowledgePersist>(json, false);
    //    // model.RestoreAll(MentorManager!,PubSub);
    //     //SetDisabled();

    //     await PubSub!.Publish<RefreshRenderMessage>(FoundryBlazor.Model.RefreshRenderMessage.ClearAllSelected());   
        
    // }

    protected void Run()
    {
        //MentorServices?.EstablishModel("Test");
        Refresh();
    }

    protected void SaveDrawing()
    {
        //var mentor = MentorServices?.MentorModel;
       // mentor?.SaveDrawing<DrawingPersist>();
       // var options = new NavigationOptions { Target = "_blank" };
       //var url = $"{Navigation!.BaseUri}api/MentorStorage/drawing";
        //Navigation!.NavigateTo(url, true);

    }

    protected void SaveModel()
    {
        //var mentor = MentorServices?.MentorModel;
        //mentor?.SaveModel<KnowledgePersist>();
       // var options = new NavigationOptions { Target = "_blank" };
        //var url = $"{Navigation!.BaseUri}api/MentorStorage/model";
        //Navigation!.NavigateTo(url, true);

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


    // private void OnRenderRefresh(RefreshRenderMessage message)
    // {
    //     //no reason to refresh if we are already refreshing
    //     // if (KnBase.RefreshTree)
    //     //     return;

    //    // $"START OnRenderRefresh ".WriteSuccess(4);

    //    // $"MARK AS DIRTY ShapeTreeBase OnRenderRefresh [{message}]".WriteInfo(3);

    //     // var success = message.State switch
    //     // {
    //     //     RefreshState.Refresh => DoRefresh(message),
    //     //     RefreshState.ClearAllSelected => DoRefresh(message),
    //     //     RefreshState.Selected => DoRefresh(message),
    //     //     _ => false
    //     // };
       
    //     // if (success)
    //     //     $"ShapeTreeBase OnRenderRefresh {message.State} Was Applied".WriteSuccess(3);
    //     // else
    //     //     $"ShapeTreeBase OnRenderRefresh {message.State} Failed ".WriteError(3);


    //   //  $"END OnRenderRefresh ".WriteSuccess(4);
    // }


    // private bool DoRefresh(RefreshRenderMessage message)
    // {
    //     // if (KnBase.RefreshTree)
    //     //     return false;

    //     Refresh();
    //     return true;
    // }

    // private bool DoSelected(UIChanged message)
    // {
    //     var guid = message.Selections[0];
    //     $"MentorModelManager DoSelected {guid}".WriteSuccess();
    //     GetAllNodes();
    //     Task.Run(() => InvokeAsync(StateHasChanged));
    //     return true;
    // }
}