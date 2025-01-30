using BlazorThreeJS.Core;

using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;

namespace FoundryBlazor.Shape;

public class FoText3D : FoShape3D, IShape3D
{



    public double fontsize { get; set; } = 0.5;
    public double FontSize 
    { 
        get { return this.fontsize; } 
        set { this.fontsize = AssignDouble(value, fontsize); } 
    }


    private string text = "";
    public string Text
    {
        get { return this.text; }
        set { this.text = CreateDetails(AssignText(value, text)); }
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

    public Text3D AsText3D()
    {
        var label = new Text3D(Text)
        {
            Uuid = GetGlyphId(),
            Name = GetName(),
            Color = Color ?? "Yellow",
            FontSize = FontSize,
            Transform = GetTransform()
        };
        if ( OnAnimationUpdate != null )
            label.SetAnimationUpdate(OnAnimationUpdate);

        label.SetDirty(true);
        return label;
    }

    public override (bool success, Object3D result) GetValue3D()
    {
        if ( IsDirty == false && Value3D != null )
            return (true, Value3D);

        if ( Value3D == null )
        {
            Value3D = AsText3D();
            Value3D.SetDirty(true);
            return (true, Value3D);
        }

        //at this point we have a Value3D but it requires updating
        var label = Value3D as Text3D;
        if (label != null)
        {
            label.Text = Text;
            label.FontSize = FontSize;
            label.Color = Color ?? "Yellow";
            label.Transform = GetTransform();
            return (true, label);
        }
        else
        {
            return (false, Value3D);
        }
    }



    public override bool RefreshToScene(Scene3D scene, bool deep = true)
    {
        var (success, _) = ComputeValue3D(scene);

        if (success)
            $"FoText3D RefreshToScene {Name} {Text} {FontSize}".WriteSuccess();
        else
            $"FoText3D RefreshToScene NO Value3D".WriteError();

        return success;
    }



    public override string GetTreeNodeTitle()
    {
        return $"{base.GetTreeNodeTitle()} {Text}";
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