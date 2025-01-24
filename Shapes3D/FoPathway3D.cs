
using BlazorThreeJS.Core;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;

namespace FoundryBlazor.Shape;


public class FoPathway3D : FoShape3D, IPipe3D
{


    public FoPathway3D(string name) : base(name, "Grey")
    {

    }

    public Object3D EstablishPathway3D()
    {
        if (GeometryParameter3D.HasValue3D) 
            return GeometryParameter3D.GetValue3D();

        GeometryParameter3D.AsTube(this);
        return GeometryParameter3D.GetValue3D();
    }

    // public override bool RefreshScene(Scene3D scene, bool deep = true)
    // {
    //     $"RenderPathway {Key}".WriteNote();
    //     scene.AddChild(EstablishPathway3D());
    //     return true;
    // }

}