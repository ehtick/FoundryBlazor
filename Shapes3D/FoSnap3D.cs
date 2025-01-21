
using BlazorThreeJS.Core;
using BlazorThreeJS.Enums;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Objects;
using BlazorThreeJS.Viewers;
using BlazorThreeJS.Settings;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;


namespace FoundryBlazor.Shape;

public class FoSnap3D : FoGlyph3D, ISnap3D
{


    public FoSnap3D() : base()
    {
    }
    public FoSnap3D(string name) : base(name)
    {
    }

    public FoSnap3D(string name, string color) : base(name, color)
    {
    }





}