namespace FoundryBlazor.Shape;


public enum SelectionState
{
    Cleared,
    Changed,
    Dropped,
}

public class SelectionChanged
{
    public SelectionState State { get; set; }
    public required List<FoGlyph2D> Selections { get; set; }
    public static SelectionChanged Changed(List<FoGlyph2D> selections)
    {
        var list = new List<FoGlyph2D>();
        list.AddRange(selections);
        var result = new SelectionChanged()
        {
            State = SelectionState.Changed,
            Selections = list
        };
        return result;
    }
    public static SelectionChanged Cleared(List<FoGlyph2D> selections)
    {
        var list = new List<FoGlyph2D>();
        list.AddRange(selections);
        var result = new SelectionChanged()
        {
            State = SelectionState.Cleared,
            Selections = list
        };
        return result;
    }
    public static SelectionChanged Dropped(List<FoGlyph2D> selections)
    {
        var list = new List<FoGlyph2D>();
        list.AddRange(selections);
        var result = new SelectionChanged()
        {
            State = SelectionState.Dropped,
            Selections = list
        };
        return result;
    }
}