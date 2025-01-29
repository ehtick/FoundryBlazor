
using System.Linq;
using BlazorThreeJS.Geometires;
using BlazorThreeJS.Materials;
using BlazorThreeJS.Maths;
using BlazorThreeJS.Viewers;
using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Extensions;
using FoundryRulesAndUnits.Models;
using BlazorThreeJS.Core;

namespace FoundryBlazor.Shape;

public class FoMenu3D : FoPanel3D, IFoMenu
{


    public List<IFoButton> Buttons()
    {
        return GetMembers<FoButton3D>()?.Select(item => item as IFoButton).ToList() ?? new List<IFoButton>();
    }

    public FoMenu3D(string name) : base(name)
    {
        //ResetLocalPin((obj) => 0, (obj) => 0);
    }



    public override FoMenu3D Clear()
    {
        GetMembers<FoButton3D>()?.Clear();
        return this;
    }


    public FoMenu3D LayoutHorizontal(int width = 95, int height = 40)
    {
        return this;
    }

    public FoMenu3D LayoutVertical(int width = 95, int height = 40)
    {
        return this;
    }

    public bool DrawMenu3D(Scene3D scene)
    {
        var buttons = Buttons().Select((item) =>
        {
            var text = item.DisplayText();
            var button = new Button3D(text, text)
            {
                OnClick = (Button3D btn) => Console.WriteLine("Clicked Button1")
            };
            return button;
        }).ToList();

        Width = 1;
        Height = buttons.Count * 0.22;
        var menu = new PanelMenu3D()
        {
            Buttons = buttons,
            Height = Height,
            Width = Width,
            //Transform = GetTransform()
        };
        scene.AddChild(menu);
        return true;
    }

    public override bool RefreshToScene(Scene3D ctx, bool deep = true)
    {
        var result = DrawMenu3D(ctx);
        return result;
    }

}
