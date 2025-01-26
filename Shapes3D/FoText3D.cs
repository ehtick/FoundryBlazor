using BlazorThreeJS.Core;

using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Models;

namespace FoundryBlazor.Shape;

public class FoText3D : FoShape3D, IShape3D
{

    public double FontSize { get; set; } = 0.5;


    private string _text = "";
    public string Text
    {
        get { return this._text; }
        set { this._text = CreateDetails(AssignText(value, _text)); }
    }

    public List<string>? Details { get; set; }

    public FoText3D() : base()
    {
        GeomType = "Text";
    }
    public FoText3D(string name) : base(name)
    {
        GeomType = "Text";
    }
    public FoText3D(string name, string color) : base(name, color)
    {
        GeomType = "Text";
    }

    protected string CreateDetails(string text)
    {
        Details = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        return text;
    }

    protected string AssignText(string newValue, string oldValue)
    {
        if (newValue != oldValue)
        {
            //ComputeResize = true;
        }

        return newValue;
    }

    public FoText3D CreateTextAt(string text, double x, double y, double z)
    {
        GetTransform().Position = new Vector3(x, y, z);
        Text = text;
        return this;
    }




    public override string GetTreeNodeTitle()
    {
        return $"{base.GetTreeNodeTitle()} {Text}";
    }

    public Text3D AsText3D()
    {
        var label = new Text3D(Text)
        {
            Uuid = GetGlyphId(),
            Name = GetName(),
            Color = Color ?? "Yellow",
            FontSize = FontSize,
            Transform = new Transform3()
            {
                Position = this.GetPosition(),
                Rotation = this.GetRotation(),
                Pivot = this.GetPivot(),
                Scale = this.GetScale(),
            }
        };
        return label;
    }




    // public bool UpdateText(string text)
    // {
    //     Text = text;
    //     //"Update label text".WriteSuccess();
    //     if (Label != null)
    //     {
    //         Label.Text = Text;
    //         return true;
    //     }

    //     return false;
    // }

 

}