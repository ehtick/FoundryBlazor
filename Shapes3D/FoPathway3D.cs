
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


}