using Blazor.Extensions.Canvas.Canvas2D;


namespace FoundryBlazor.Shape;

public class FoText2D : FoGlyph2D, IShape2D
{
    private string text = "";
    public string Text { get { return this.text; } set { this.text = CreateDetails(AssignText(value, text)); } }

    private string fontsize = "30";
    public string FontSize { get { return this.fontsize; } set { this.fontsize = AssignText(value, fontsize); } }

    private string font = "Segoe UI";
    public string Font { get { return this.font; } set { this.font = AssignText(value, font); } }

    public string TextColor { get; set; } = "White";
    public bool IsEditing { get; set; } = false;
    public bool ComputeResize { get; set; } = false;

    public int Margin { get; set; } = 2;
    public List<string>? Details { get; set; }

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

    public string FontSizeAndName()
    {
        //styles -   normal | italic | oblique | inherit
        //weights - normal | bold | bolder | lighter | 100 | 200 | 300 | 400 | 500 | 600 | 700 | 800 | 900 | inherit | auto
        // face - serif, sans-serif, cursive, fantasy, and monospace.

        //[font style] [font weight] [font size] [font face]

        //"normal bold 80px sans-serif"
        //"italic bold 24px serif"
        //"normal lighter 50px cursive"
        return $"{FontSize}px {Font}";
    }

    public override async Task Draw(Canvas2DContext ctx, int tick)
    {
        //$"FoText2D Draw {Text} {Width} {Height}   ".WriteLine(ConsoleColor.DarkMagenta);   

        await ctx.FillRectAsync(0, 0, Width, Height);

        await ctx.SetTextAlignAsync(TextAlign.Left);
        await ctx.SetTextBaselineAsync(TextBaseline.Top);

        await ctx.SetFillStyleAsync(TextColor);
        await ctx.FillTextAsync(Text, LeftX() + 1, TopY() + 1);

    }

    // https://jenkov.com/tutorials/html5-canvas/text.html

    public async Task ComputeSize(Canvas2DContext ctx, string Fragment)
    {
        if (string.IsNullOrEmpty(Fragment))
        {
            ComputeResize = false;
            return;
        }

        var result = await ComputeMeasuredText(ctx, Fragment, FontSize, Font);
        Height = result.Height + Margin;
        Width = result.Width + Margin;
        ComputeResize = result.Failure;
    }

    public override async Task<bool> RenderDetailed(Canvas2DContext ctx, int tick, bool deep = true)
    {
        if (!IsVisible) return false;

        await ctx.SaveAsync();
        await ctx.SetFontAsync(FontSizeAndName());

        if (IsEditing || ComputeResize)
            await ComputeSize(ctx, Text);

        await UpdateContext(ctx, tick);

        PreDraw?.Invoke(ctx, this);
        await Draw(ctx, tick);
        HoverDraw?.Invoke(ctx, this);
        PostDraw?.Invoke(ctx, this);

        if (IsSelected)
        {
            DrawSelected?.Invoke(ctx, this);
            //GetHandles()?.ForEach(async child => await child.Render(ctx, tick, deep));
            //await DrawPin(ctx);
        }


        await ctx.RestoreAsync();
        return true;
    }



    public FoText2D() : base()
    {
    }
    public FoText2D(string name) : base(name, "Orange")
    {
        Text = name;
    }

    public FoText2D(int width, int height, string color) : base("", width, height, color)
    {
        Text = "Hello Everyone";
    }





}
