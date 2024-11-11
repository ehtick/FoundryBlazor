using BlazorThreeJS.Core;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using System.Text.Json.Serialization;


namespace FoundryBlazor.Shape;

public class FoGeometryParameter2D : FoComponent
{
    public string GlyphId { get; set; } = "";
    protected Object3D? _value3D;

    public Object3D? Value3D()
    {
        return _value3D;
    }

    public Object3D SetValue3D(Object3D value)
    {
        _value3D = value;
        return _value3D;
    }

 

    public FoGeometryParameter2D() : base("")
    {
    }
    public FoGeometryParameter2D(string name) : base(name)
    {
    }


    public string GetName()
    {
        return Key;
    }


    public string GetGlyphId()
    {
        if (string.IsNullOrEmpty(GlyphId))
            GlyphId = Guid.NewGuid().ToString();

        return GlyphId;
    }

    public bool GlyphIdCompare(string other)
    {
        var id = GetGlyphId();
        var result = id == other;
        // $"GlyphIdCompare {result}  {id} {other}".WriteNote();
        return result;
    }




}