using BlazorThreeJS.Core;

using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Models;

namespace FoundryBlazor.Shape;

public class FoText3D : FoGlyph3D, IShape3D
{

    private Text3D? Label { get; set; }

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
    }
    public FoText3D(string name) : base(name)
    {
    }
    public FoText3D(string name, string color) : base(name, color)
    {
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


    public override Vector3 GetPosition(double x = 0, double y = 0, double z = 0)
    {
        if (Transform == null)
            return base.GetPosition(x, y, z);

        var result = Transform.Position;
        return result;
    }

    public override string GetTreeNodeTitle()
    {
        return $"{base.GetTreeNodeTitle()} {Text}";
    }

    public override bool RefreshScene(Scene3D scene, bool deep = true)
    {
        var text = Text ?? "Text3D";
        Label = new Text3D(text)
        {
            Uuid = GetGlyphId(),
            Name = GetName(),
            Color = Color ?? "Yellow",
            Transform = GetTransform(),
            FontSize = FontSize,
        };
        scene.AddChild(Label);

        return true;
    }

    public bool UpdateText(string text)
    {
        Text = text;
        //"Update label text".WriteSuccess();
        if (Label != null)
        {
            Label.Text = Text;
            return true;
        }

        return false;
    }

    public override bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    {
        //"Update label position".WriteSuccess();
        if (Label != null)
        {
            Label.Transform.Position.Set(xLoc, yLoc, zLoc);
            return true;
        }

        return false;
    }

}