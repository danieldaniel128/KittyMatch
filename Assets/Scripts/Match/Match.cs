using System.Collections.Generic;

public class Match
{
    public List<TileController> Tiles { get; private set; }
    public int Score => Tiles.Count;
    public SpecialMatch MatchType { get; private set; }
    public int HorizontalCount { get; private set; }
    public int VerticalCount { get; private set; }

    public bool IsSpecial => MatchType != default;

    public Match(TileController origin, List<TileController> horizontal, List<TileController> vertical)
    {
        Tiles = new List<TileController> { origin };
        if (horizontal.Count >= 2) Tiles.AddRange(horizontal);
        if (vertical.Count >= 2) Tiles.AddRange(vertical);

        HorizontalCount = horizontal.Count + 1;
        VerticalCount = vertical.Count + 1;

        MatchType = DetermineMatchType();
    }

    private SpecialMatch DetermineMatchType()
    {
        if (HorizontalCount >= 5 || VerticalCount >= 5) return SpecialMatch.FiveRow;
        if (HorizontalCount == 4) return SpecialMatch.FourRow;
        if (VerticalCount == 4) return SpecialMatch.FourColumn;
        if (HorizontalCount >= 3 && VerticalCount >= 3) return SpecialMatch.TShape;
        return default;
    }
}
public enum SpecialMatch
{
    None,
    FourRow,
    FourColumn,
    FiveRow,
    FiveColumn,
    TShape,
    LShape
}