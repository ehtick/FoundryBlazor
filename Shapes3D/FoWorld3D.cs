// this is a tool to load/unload knowledge modules that define projects





using FoundryBlazor.Extensions;
using FoundryRulesAndUnits.Models;


namespace FoundryBlazor.Shape;

// a World is just of bag o things to draw,  then need to be handed to a arena to do the rendering
// on to a stage or scene

public interface IWorld3D: ITreeNode
{
    public List<FoGroup3D>? ShapeGroups();
    public List<FoDatum3D>? Datums();
    public List<FoShape3D>? ShapeBodies();
    public List<FoMenu3D>? Menus();
    public List<FoPanel3D>? Panels();
    public List<FoText3D>? Labels();
    public List<FoRelationship3D>? Relationships();
}

public class FoWorld3D : FoGlyph3D, IWorld3D
{


    public FoWorld3D() : base()
    {
        GetSlot<FoGroup3D>();
        GetSlot<FoShape3D>();
        GetSlot<FoText3D>();
        GetSlot<FoDatum3D>();
        GetSlot<FoMenu3D>();
        GetSlot<FoPanel3D>();
        GetSlot<FoPathway3D>();
    }

    public override IEnumerable<ITreeNode> GetChildren()
    {
        var list = new List<ITreeNode>();
        AddFolderIfNotEmpty<FoGroup3D>(list);
        AddFolderIfNotEmpty<FoShape3D>(list);
        AddFolderIfNotEmpty<FoText3D>(list);
        AddFolderIfNotEmpty<FoDatum3D>(list);

        AddFolderIfNotEmpty<FoMenu3D>(list);

        AddFolderIfNotEmpty<FoPanel3D>(list);
        AddFolderIfNotEmpty<FoPathway3D>(list);
        return list;
    }

 
    public List<FoGroup3D>? ShapeGroups()
    {
        return GetMembers<FoGroup3D>();
    }
    public List<FoDatum3D>? Datums()
    {
        return GetMembers<FoDatum3D>();
    }

    public List<FoShape3D>? ShapeBodies()
    {
        return GetMembers<FoShape3D>();
    }

    public List<FoMenu3D>? Menus()
    {
        return GetMembers<FoMenu3D>();
    }
    public List<FoPanel3D>? Panels()
    {
        return GetMembers<FoPanel3D>();
    }

    public List<FoText3D>? Labels()
    {
        return GetMembers<FoText3D>();
    }
    public List<FoRelationship3D>? Relationships()
    {
        return GetMembers<FoRelationship3D>();
    }




    public FoWorld3D RemoveDuplicates()
    {

        var platforms = ShapeGroups()?.GroupBy(i => i.GlyphId).Select(g => g.First()).ToList();
        if (platforms != null)
            GetSlot<FoGroup3D>()?.Flush().AddRange(platforms);

        var bodies = ShapeBodies()?.GroupBy(i => i.GlyphId).Select(g => g.First()).ToList();
        if (bodies != null)
            GetSlot<FoShape3D>()?.Flush().AddRange(bodies);

        var labels = Labels()?.GroupBy(i => i.GlyphId).Select(g => g.First()).ToList();
        if (labels != null)
            GetSlot<FoText3D>()?.Flush().AddRange(labels);

        var datums = Datums()?.GroupBy(i => i.GlyphId).Select(g => g.First()).ToList();
        if (datums != null)
            GetSlot<FoDatum3D>()?.Flush().AddRange(datums);

        var relationships = Relationships()?.GroupBy(i => i.GlyphId).Select(g => g.First()).ToList();
        if (relationships != null)
            GetSlot<FoRelationship3D>()?.Flush().AddRange(relationships);

        return this;
    }
}
