using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;
using FoundryBlazor.Extensions;

namespace FoundryBlazor.Shape;

public interface IGlueOwner
{
    void AddGlue(FoGlue2D glue);
    void RemoveGlue(FoGlue2D glue);

    bool Smash(bool force);
    //void RecomputeGlue();
    //public bool ComputeFinishFor(FoGlyph2D? target);
    //public bool ComputeStartFor(FoGlyph2D? target);
}

public class FoShape1D : FoGlyph2D, IGlueOwner, IShape1D
{
    private static int gluecount = 0;
    protected int x1 = 0;
    public int StartX { get { return this.x1; } set { this.x1 = AssignInt(value, x1); } }
    protected int y1 = 0;
    public int StartY { get { return this.y1; } set { this.y1 = AssignInt(value, y1); } }
    protected int x2 = 0;
    public int FinishX { get { return this.x2; } set { this.x2 = AssignInt(value, x2); } }
    protected int y2 = 0;
    public int FinishY { get { return this.y2; } set { this.y2 = AssignInt(value, y2); } }

    private double rotation = 0;
    public float AntiRotation { get { return (float)(-1.0 * this.rotation * Matrix2D.DEG_TO_RAD); } }

    protected Point? startPT;
    protected Point? finishPT;
    public Point Start()
    {
        if (startPT == null)
        {
            var matrix = GetInvMatrix();
            if (matrix == null)
            {
                "Point Start() IMPOSSABLE".WriteError();
                return new Point();
            }

            startPT = matrix.TransformPoint(StartX, StartY);
        }
        return (Point)startPT;
    }
    public Point Finish()
    {
        if (finishPT == null)
        {
            var matrix = GetInvMatrix();
            if (matrix == null)
            {
                "Point Finish() IMPOSSABLE".WriteError();
                return new Point();
            }
            finishPT = matrix.TransformPoint(FinishX, FinishY);
        }
        return (Point)finishPT;
    }

    public FoShape1D() : base()
    {
        ShapeDraw = DrawRect;
        this.height = 10;
    }

    public FoShape1D(int x1, int y1, int x2, int y2, int height, string color) : base("", color)
    {
        ShapeDraw = DrawRect;

        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.height = height;
    }


    public FoShape1D(FoGlyph2D? start, FoGlyph2D? finish, int height, string color) : base("", color)
    {
        ShapeDraw = DrawRect;
        this.x1 = start?.PinX ?? 0;
        this.y1 = start?.PinY ?? 0;
        this.x2 = finish?.PinX ?? 0;
        this.y2 = finish?.PinY ?? 0;
        this.height = height;

        if (start != null)
            GlueStartTo(start);

        if (finish != null)
            GlueFinishTo(finish);
    }

    public override Rectangle Rect()
    {
        var d = Height / 2;
        var sz = new Size(Height, Height);
        var loc = PinLocation();
        var matrix = GetMatrix();
        var pt = matrix?.TransformPoint(loc.X - d, loc.Y - d) ?? new Point(loc.X, loc.Y);
        var result = new Rectangle(pt, sz);
        return result;
    }



    public override bool Smash(bool force)
    {
        if (!base.Smash(force)) return false;
        if (startPT != null)
        {
            startPT = null;
            //$"Smashing startPT  {Name} {GetType().Name}".WriteInfo(3);
        }
        if (finishPT != null)
        {
            finishPT = null;
            //$"Smashing finishPT  {Name} {GetType().Name}".WriteInfo(3);
        }

        return true;
    }




    public override List<FoHandle2D> GetHandles()
    {
        if (!this.HasSlot<FoHandle2D>())
        {
            Add<FoHandle2D>(new FoHandle2D("Begin", LeftX(), CenterY(), "Green"));
            Add<FoHandle2D>(new FoHandle2D("Finish", RightX(), CenterY(), "Green"));
            Add<FoHandle2D>(new FoHandle2D("Top", CenterX(), TopY(), "Green"));
            Add<FoHandle2D>(new FoHandle2D("Bottom", CenterX(), BottomY(), "Green"));
        }
        var result = this.Members<FoHandle2D>();
        return result;
    }

    public override Matrix2D GetMatrix()
    {
        if (_matrix == null)
        {
            RecomputeGlue();
            var dx = (double)(x2 - x1);
            var dy = (double)(y2 - y1);
            x = (x2 + x1) / 2;  //compute PinX in center
            y = (y2 + y1) / 2; //compute PinY in center
            width = (int)Math.Sqrt(dx * dx + dy * dy); //compute the length
            rotation = (Math.Atan2(dy, dx) * Matrix2D.RAD_TO_DEG) + RotationZ(this);

            //$"Shape1D GetMatrix {PinX} {PinY} {angle}".WriteError();
            _matrix = new Matrix2D();
            if (_matrix != null)
            {
                _matrix.AppendTransform(this.PinX, this.PinY, 1.0, 1.0, rotation, 0.0, 0.0, LocPinX(this), LocPinY(this));
            }
            else
                "GetMatrix Shape1D here is IMPOSSABLE".WriteError();

            //FoGlyph2D.ResetHitTesting = true;
            //$"GetMatrix  {Name}".WriteLine(ConsoleColor.DarkBlue);
        }
        return _matrix!;
    }



    public void RecomputeGlue()
    {
        GetMembers<FoGlue2D>()?.ForEach(glue =>
        {
            var (source, target) = glue;
            if (source == this && target != null)
            {
                var found = glue.Name[..3] switch
                {
                    "STA" => ComputeStartFor(target),
                    "FIN" => ComputeFinishFor(target),
                    _ => false
                };
            }
        });
    }


    public async Task DrawStart(Canvas2DContext ctx, string color)
    {
        var start = Start();


        await ctx.SaveAsync();
        await ctx.BeginPathAsync();

        await ctx.SetFillStyleAsync(color);
        await ctx.ArcAsync(start.X, start.Y, 26.0, 0.0, 2 * Math.PI);
        await ctx.FillAsync();

        await ctx.SetLineWidthAsync(1);
        await ctx.SetStrokeStyleAsync("#003300");
        await ctx.StrokeAsync();

        await ctx.SetTextAlignAsync(TextAlign.Center);
        await ctx.SetTextBaselineAsync(TextBaseline.Middle);
        var FontSpec = $"normal bold 28px sans-serif";
        await ctx.SetFontAsync(FontSpec);
        await ctx.SetFillStyleAsync("Black");

        await ctx.RotateAsync(AntiRotation);
        await ctx.FillTextAsync("Start", start.X, start.Y);

        await ctx.RestoreAsync();
    }

    public async Task DrawFinish(Canvas2DContext ctx, string color)
    {

        var finish = Finish();

        await ctx.SaveAsync();
        await ctx.BeginPathAsync();

        await ctx.SetFillStyleAsync(color);
        await ctx.ArcAsync(finish.X, finish.Y, 26.0, 0.0, 2 * Math.PI);
        await ctx.FillAsync();

        await ctx.SetLineWidthAsync(1);
        await ctx.SetStrokeStyleAsync("#003300");
        await ctx.StrokeAsync();

        await ctx.SetTextAlignAsync(TextAlign.Center);
        await ctx.SetTextBaselineAsync(TextBaseline.Middle);
        await ctx.SetFillStyleAsync("Black");
        var FontSpec = $"normal bold 28px sans-serif";
        await ctx.SetFontAsync(FontSpec);
        await ctx.FillTextAsync("Finish", finish.X, finish.Y);

        await ctx.RestoreAsync();
    }


    public FoGlue2D? GlueStartTo(FoGlyph2D? target)
    {
        if (target == null) return null;

        var glue = new FoGlue2D($"START_{target.Name}_{gluecount++}");
        glue.GlueTo(this, target);
        Smash(false);
        return glue;
    }

    public FoGlue2D? GlueFinishTo(FoGlyph2D? target)
    {
        if (target == null) return null;

        var glue = new FoGlue2D($"FINISH_{target.Name}_{gluecount++}");
        glue.GlueTo(this, target);
        Smash(false);
        return glue;
    }

    public bool ComputeStartFor(FoGlyph2D? target)
    {
        if (target == null) return false;
        var pt = target.AttachTo();
        StartX = pt.X;
        StartY = pt.Y;

        IsSelected = false;
        return true;

        //  $"{Name} ComputeStartFor {target.Name}: {StartX}  {StartY}:  pin {PinX} {PinY}".WriteInfo();
    }



    public bool ComputeFinishFor(FoGlyph2D? target)
    {
        if (target == null) return false;
        var pt = target.AttachTo();
        FinishX = pt.X;
        FinishY = pt.Y;

        IsSelected = false;
        return true;

        // $"{Name} ComputeFinishFor {target.Name}: {FinishX}  {FinishY}:   pin {PinX} {PinY}".WriteInfo();
    }



    public virtual async Task<bool> DrawStraight(Canvas2DContext ctx, string color, int tick)
    {

        var start = Start();
        var finish = Finish();

        await ctx.BeginPathAsync();
        await ctx.MoveToAsync(start.X, start.Y);
        await ctx.LineToAsync(finish.X, finish.Y);
        await ctx.ClosePathAsync();

        await ctx.SetStrokeStyleAsync(color ?? Color);
        await ctx.SetLineWidthAsync(Thickness);
        await ctx.StrokeAsync();
        return true;
    }


    public override async Task<bool> RenderDetailed(Canvas2DContext ctx, int tick, bool deep = true)
    {
        if (CannotRender()) return false;

        await ctx.SaveAsync();
        await UpdateContext(ctx, tick);

        PreDraw?.Invoke(ctx, this);
        await Draw(ctx, tick);

        if (!IsSelected)
            HoverDraw?.Invoke(ctx, this);

        await DrawTag(ctx);

        PostDraw?.Invoke(ctx, this);

        if (IsSelected)
            await DrawWhenSelected(ctx, tick, deep);

        if (deep)
        {
            GetMembers<FoShape1D>()?.ForEach(async child => await child.RenderDetailed(ctx, tick, deep));
            GetMembers<FoShape2D>()?.ForEach(async child => await child.RenderDetailed(ctx, tick, deep));
        }

        if (GetMembers<FoGlue2D>()?.Count > 0)
            await DrawTriangle(ctx, "Black");

        await DrawStraight(ctx, "Red", tick);
        await DrawStart(ctx, "Blue");
        await DrawFinish(ctx, "Cyan");

        await ctx.RestoreAsync();
        return true;
    }
}
