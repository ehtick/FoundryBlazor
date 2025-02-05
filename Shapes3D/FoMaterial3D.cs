
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

public class FoMaterial3D : FoGlyph3D
{


    public FoMaterial3D() : base()
    {
    }
    public FoMaterial3D(string name) : base(name)
    {
    }

    public FoMaterial3D(string name, string color) : base(name, color)
    {
    }





}