
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
using static System.Formats.Asn1.AsnWriter;


namespace FoundryBlazor.Shape;

public class FoModel3D : FoShape3D
{

    public string Url { get; set; } = "";


    public FoModel3D() : base()
    {
    }
    public FoModel3D(string name) : base(name)
    {
    }
    public FoModel3D(string name, string color) : base(name, color)
    {
    }

    public Model3D AsModel3D(Model3DFormats format)
    {
        var model = new Model3D()
        {
            Url = Url,
            Format = format,
            Name = this.GetName(),
            Uuid = this.GetGlyphId(),
            Transform = new Transform3()
            {
                Position = this.GetPosition(),
                Rotation = this.GetRotation(),
                Pivot = this.GetPivot(),
                Scale = this.GetScale(),
            }
        };
        return model;
    }




    public FoModel3D CreateGlb(string url, double width, double height, double depth)
    {
        CreateGlb(url);
        BoundingBox = new Vector3(width, height, depth);
        return this;
    }

    public FoModel3D CreateGlb(string url)
    {
        GeomType = "Glb";
        Url = string.Intern(url);
        $"CreateGlb url [{Url}] ".WriteSuccess();
        return this;
    }




}