using System.Collections.Generic;
using UnityEngine;

public class MatchFinder
{
    private Dictionary<Vector2Int, TileController> _tiles;//
    public MatchFinder(Dictionary<Vector2Int, TileController> tiles)
    {
        _tiles = tiles;
    }

    public List<Match> FindMatches()
    {
        var matches = new List<Match>();
        foreach (var tile in _tiles)
        {
            var match = MatchAnalyzer.Analyze(tile.Key, _tiles);
            if (match != null && match.Score >= 0)
                matches.Add(match);
        }
        return MatchFilter.RemoveRedundantMatches(matches);
    }
}