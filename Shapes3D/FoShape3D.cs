
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

public class FoShape3D : FoGlyph3D, IShape3D
{

    public string Url { get; set; } = "";
    public string GeomType { get; set; } = "";

    public double Radius { get; set; } = 0.025;
    public Vector3? Position { get; set; }
    public List<Vector3>? Path { get; set; }
    public Vector3? Pivot { get; set; }
    public Euler? Rotation { get; set; } // replace with Quaternion
    public Vector3? BoundingBox { get; set; }
    public Vector3? Scale { get; set; }
    public string? LoadingURL { get; set; }

    public List<FoPanel3D>? TextPanels { get; set; }
    public Action<ImportSettings> UserHit { get; set; } = (ImportSettings model3D) => { };

    //private Object3D? ShapeMesh { get; set; }
    //private Object3D? ShapeObject3D { get; set; }

    public FoShape3D() : base()
    {
    }
    public FoShape3D(string name) : base(name)
    {
    }
    public FoShape3D(string name, string color) : base(name, color)
    {
    }

    //https://BlazorThreeJS.com/reference/Index.html

    public override bool UpdateMeshPosition(double xLoc, double yLoc, double zLoc)
    {
        //"Update mesh position".WriteSuccess();
        if (_value3D != null)
        {
            _value3D.Position.Set(xLoc, yLoc, zLoc);
            return true;
        }

        return false;
    }

    public override string GetTreeNodeTitle()
    {

        var HasMesh = _value3D != null ? "Ok" : "No Value3D";
        return $"{GeomType}: {Key} {GetType().Name} {HasMesh} => ";
    }


    public FoShape3D CreateBox(string name, double width, double height, double depth)
    {
        GeomType = "Box";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    public FoShape3D CreateBoundry(string name, double width, double height, double depth)
    {
        GeomType = "Boundry";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    public FoShape3D CreateCylinder(string name, double width, double height, double depth)
    {
        GeomType = "Cylinder";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }

    //CreateDodecahedron
    public FoShape3D CreateDodecahedron(string name, double width, double height, double depth)
    {
        GeomType = "Dodecahedron";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    //CreateIcosahedron
    public FoShape3D CreateIcosahedron(string name, double width, double height, double depth)
    {
        GeomType = "Icosahedron";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    //CreateOctahedron
    public FoShape3D CreateOctahedron(string name, double width, double height, double depth)
    {
        GeomType = "Octahedron";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    //CreateTorus
    public FoShape3D CreateTetrahedron(string name, double width, double height, double depth)
    {
        GeomType = "Tetrahedron";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    //CreateTorusKnot
    public FoShape3D CreateTorusKnot(string name, double width, double height, double depth)
    {
        GeomType = "TorusKnot";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    //CreateTorus 
    public FoShape3D CreateTorus(string name, double width, double height, double depth)
    {
        GeomType = "Torus";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }

    public FoShape3D CreateTube(string name, double radius, List<Vector3> path)
    {
        GeomType = "Tube";
        Radius = radius;
        BoundingBox = new Vector3(radius, 0, 0);
        Key = name;
        Path = path;
        return this;
    }

    public FoShape3D CreateGlb(string url, double width, double height, double depth)
    {
        CreateGlb(url);
        BoundingBox = new Vector3(width, height, depth);
        return this;
    }
    public FoShape3D CreateGlb(string url)
    {
        GeomType = "Glb";
        Url = url;
        $"CreateGlb url [{Url}] ".WriteSuccess();
        return this;
    }

    public FoShape3D CreateSphere(string name, double width, double height, double depth)
    {
        GeomType = "Sphere";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }

    public FoShape3D CreateCircle(string name, double width, double height, double depth)
    {
        GeomType = "Circle";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    public FoShape3D CreatePlane(string name, double width, double height, double depth)
    {
        GeomType = "Plane";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }

    public FoShape3D CreateRing(string name, double width, double height, double depth)
    {
        GeomType = "Ring";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }

    public FoShape3D CreateCapsule(string name, double width, double height, double depth)
    {
        GeomType = "Capsule";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }
    public FoShape3D CreateCone(string name, double width, double height, double depth)
    {
        GeomType = "Cone";
        BoundingBox = new Vector3(width, height, depth);
        Key = name;
        return this;
    }

       

    public Object3D Box()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);
        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new BoxGeometry(box.X, box.Y, box.Z),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial(),

        };

        return _value3D;
    }

    public Object3D Boundary()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);
        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new BoxGeometry(box.X, box.Y, box.Z),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetWireframe()
        };
        return _value3D;
    }

    private Object3D Cylinder()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);
        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new CylinderGeometry(radiusTop: box.X / 2, radiusBottom: box.X / 2, height: box.Y),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Sphere()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new SphereGeometry(radius: box.X / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Tube()
    {
        if (_value3D != null) return _value3D;


        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new TubeGeometry(radius: Radius, path: Path!, 8, 10),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }
    private Object3D Circle()
    {
        if (_value3D != null) return _value3D;
        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new CircleGeometry(radius: box.X / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Capsule()
    {
        if (_value3D != null) return _value3D;
        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new CapsuleGeometry(radius: box.X / 2, box.Y),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Cone()
    {
        if (_value3D != null) return _value3D;
        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new ConeGeometry(radius: box.X / 2, height: box.Y),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Dodecahedron()
    {
        if (_value3D != null) return _value3D;
        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new DodecahedronGeometry(radius: box.X / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Icosahedron()
    {
        if (_value3D != null) return _value3D;
        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new IcosahedronGeometry(radius: box.X / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Octahedron()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new OctahedronGeometry(radius: box.X / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }
    private Object3D Tetrahedron()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new TetrahedronGeometry(radius: box.X / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }
    private Object3D Plane()
    {
        if (_value3D != null) return _value3D;

        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new PlaneGeometry(width: box.X, height: box.Y),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }

    private Object3D Ring()
    {
        if (_value3D != null) return _value3D;
        var box = BoundingBox ?? new Vector3(1, 1, 1);

        _value3D = new Mesh
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Geometry = new RingGeometry(innerRadius: box.X / 2, outerRadius: box.Y / 2),
            Position = GetPosition(),
            Pivot = GetPivot(),
            Scale = GetScale(),
            Rotation = GetRotation(),
            Material = GetMaterial()
        };
        return _value3D;
    }



    private async Task<bool> PreRenderImport(FoArena3D arena, Import3DFormats format)
    {
        var settings = AsImportSettings(arena, format);

        if (string.IsNullOrEmpty(LoadingURL)) return false;
        $"PreRenderImport url [{LoadingURL}] ".WriteInfo(1);

        var scene = arena.CurrentScene();
        var uuid = await scene.Request3DModel(settings);
        arena.Add<FoShape3D>(uuid, this);
        return true;
    }

    public ImportSettings AsImportSettings(FoArena3D arena, Import3DFormats format)
    {
        LoadingURL = Url;
        var scene = arena.CurrentScene();

        var setting = new ImportSettings
        {
            Uuid = GetGlyphId(),

            Format = format,
            FileURL = LoadingURL,
            Position = GetPosition(),
            Rotation = GetRotation(),
            Pivot = GetPivot(),
            Scale = GetScale(),

            OnClick = async (ImportSettings self) =>
            {
                self.Increment();
                //$"FoundryBlazor OnClick handler for self.Uuid={self.Uuid}, self.IsShow={self.IsShow()}".WriteInfo();
                UserHit?.Invoke(self);
                await arena.UpdateArena();
                //$"FoundryBlazor OnClick handler UpdateArena called".WriteInfo();
            },

            OnComplete = () =>
            {
                _value3D = new Group3D()
                {
                    Name = GetName(),
                    Uuid = GetGlyphId(),
                };
                scene.AddChild(_value3D);
                $"OnComplete for object3D.Uuid={_value3D.Uuid}, body.LoadingURL={LoadingURL}, position.x={Position?.X}".WriteInfo();
            }
        };

        return setting;
    }

    public static async Task<bool> PreRenderClones(List<FoShape3D> bodies, FoArena3D arena, Import3DFormats format)
    {
        var settings = new List<ImportSettings>();

        foreach (var body in bodies)
        {
            var setting = body.AsImportSettings(arena, format);
            arena.Add<FoShape3D>(body.GetGlyphId(), body);
            settings.Add(setting);

            $"AsImportSettings body.Symbol {body.Url} X = {setting.FileURL}".WriteSuccess();
        }

        var source = settings.ElementAt(0);
        settings.RemoveAt(0);

        var sourceBody = bodies.ElementAt(0);
        bodies.RemoveAt(0);

        // source.OnComplete = async () =>
        // {
        //     if (object3D != null)
        //     {
        //         sourceBody.ShapeObject3D = object3D;
        //         if (settings.Count > 0)
        //             await scene.Clone3DModel(object3D.Uuid!, settings);
        //     }
        //     else
        //         "Unexpected empty object3D".WriteError(1);
        // };

        var scene = arena.Scene!;
        await scene.Request3DModel(source);
        return true;
    }





    public MeshStandardMaterial GetWireframe()
    {
        var result = new MeshStandardMaterial()
        {
            Name = Key,
            Uuid = GetGlyphId(),
            Color = this.Color,
            Wireframe = true
        };
        return result;
    }

    public override MeshStandardMaterial GetMaterial()
    {
        if (!string.IsNullOrEmpty(Url))
            return base.GetMaterial();

        var result = GetWireframe();
        result.Wireframe = false;
        return result;
    }


    public override Vector3 GetPosition(int x = 0, int y = 0, int z = 0)
    {
        if (Position == null)
            return base.GetPosition(x, y, z);
        return Position;
    }

    public override Vector3 GetPivot(int x = 0, int y = 0, int z = 0)
    {
        if (Pivot == null)
            return base.GetPivot(x, y, z);
        return Pivot;
    }

    public override Vector3 GetScale(double x = 1, double y = 1, double z = 1)
    {
        if (Scale == null)
            return base.GetScale(x, y, z);
        return Scale;
    }

    public override Euler GetRotation(int x = 0, int y = 0, int z = 0)
    {
        if (Rotation == null)
            return base.GetRotation(x, y, z);
        return Rotation;
    }

    public override async Task<bool> PreRender(FoArena3D arena, bool deep = true)
    {
        if ( _value3D != null)
            return true;
            
        //is symbol ends with ....
        //LoadingURL = Symbol.Replace("http:", "https:");
        //await Task.CompletedTask;

        LoadingURL = Url;
        $"Shape PRERENDER {Name} => {GetTreeNodeTitle()} {LoadingURL}".WriteWarning();

        var result = GeomType switch
        {
            "Collada" => await PreRenderImport(arena, Import3DFormats.Collada),
            "Fbx" => await PreRenderImport(arena, Import3DFormats.Fbx),
            "Obj" => await PreRenderImport(arena, Import3DFormats.Obj),
            "Stl" => await PreRenderImport(arena, Import3DFormats.Stl),
            "Glb" => await PreRenderImport(arena, Import3DFormats.Gltf),
            _ => false
        };

        if (arena.Scene != null)
            SetupHitTest(arena.Scene);

        return result;
    }
    public void RenderPrimitives(Scene scene)
    {
        if (_value3D == null && IsVisible)
        {
            _value3D = GeomType switch
            {
                "Box" => Box(),
                "Boundary" => Boundary(),
                "Circle" => Circle(),
                "Cylinder" => Cylinder(),
                "Sphere" => Sphere(),
                "Plane" => Plane(),
                "Capsule" => Capsule(),
                "Cone" => Cone(),
                "Tube" => Tube(),
                "Ring" => Ring(),
                "Dodecahedron" => Dodecahedron(),
                "Icosahedron" => Icosahedron(),
                "Octahedron" => Octahedron(),
                "Tetrahedron" => Tetrahedron(),

                _ => null
            };
        };

        if (_value3D != null)
        {
            scene.AddChild(_value3D);

        }

        //delete mesh if you are invisible
        if (_value3D != null && !IsVisible)
        {
            scene.RemoveChild(_value3D);
            _value3D = null!;
        }
    }
    public override async Task<bool> RemoveFromRender(Scene scene, bool deep = true)
    {
        if (_value3D != null)
        {
            scene.RemoveChild(_value3D);
        }
        _value3D = null!;
        await Task.CompletedTask;
        return true;
    }


            // "PIN" => "Pink",
            // "PROC" => "Wisteria",
            // "DOC" => "Gray",
            // "ASST" => "Aqua",
            // "CAD" => "Orange",
            // "WRLD" => "Green",

    public List<FoPanel3D> EstablishTextPanels(ImportSettings model3D)
    {
        if ( TextPanels != null && TextPanels.Count > 0)
            return TextPanels;

        var root = model3D.Position.CreatePlus(0, 1, 0);
        if (Position != null && BoundingBox != null)
            root = Position.CreatePlus(0, BoundingBox.Y, 0);

        var leftPos = root.CreatePlus(-3, 1, 0);
        var centerPos = root.CreatePlus(0, 1, 0);  
        var rightPos = root.CreatePlus(3, 1, 0);

        //var lines = Targets?.Where(item => item.address.Length < 20 )
        //            .Select((item) => $"{item.domain}: {item.address}").ToList() ?? new List<string>();

        var center = new FoPanel3D("Threads")
        {
            Width = 2.5,
            Height = 1.5,
            Color = "Gray",
            TextLines = new() { "Thread Links" },
            Position = centerPos
        };

        //if this is Mk48 then add a panel for the process steps
        var left = new FoPanel3D("Process")
        {
            Width = 2.5,
            Height = 1.5,
            Color = "Wisteria",
            TextLines = new() { "Process Steps" },
            Position = leftPos
        };
        //if this is Mk48 then add a panel for the BOM
        var right = new FoPanel3D("BOM")
        {
            Width = 2.5,
            Height = 1.5,
            Color = "Pink",
            TextLines = new() { "BOM Structure" },
            Position = rightPos
        };

        TextPanels = new List<FoPanel3D>() { left, center, right };

        return TextPanels;
    }

    public bool SetupHitTest(Scene ctx, int tick = 0, double fps = 0, bool deep = true)
    {
        //$"SetupHitTest for {Name}".WriteInfo();
        UserHit = (ImportSettings model3D) =>
        {
            //$"In UserHit".WriteInfo();

            var list = EstablishTextPanels(model3D);
            foreach (var item in list)
            {
                item.IsVisible = model3D.IsShow();
                item.Render(ctx, tick, fps, deep);  
            }

        };
        return true;
    }

    public override bool Render(Scene scene, int tick, double fps, bool deep = true)
    {
        RenderPrimitives(scene);

        SetupHitTest(scene, tick, fps, deep);
        return true;
    }


}