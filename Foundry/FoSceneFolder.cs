using BlazorThreeJS.Scenes;
using FoundryRulesAndUnits.Models;

namespace FoundryBlazor;

public class FoSceneFolder : FoFolder
{
    private Scene Scene;
    public FoSceneFolder(Scene scene): base("Scene")
    {
        Scene = scene;
    }
    public override IEnumerable<ITreeNode> GetTreeChildren()
    {
        var result = new List<ITreeNode>();
        result.Add(Scene);
        return result;
    }
}


