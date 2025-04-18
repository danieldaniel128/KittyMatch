using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class MatchAnalyzer
{
    public static Match Analyze(Vector2Int originIndex, Dictionary<Vector2Int, TileController> tiles)
    {
        if (!tiles.TryGetValue(originIndex, out var originTile)) return null;
        if (originTile.GetModelTileType() == "EmptyRendered") return null;

        var horizontal = CollectInDirection(originIndex, Vector2Int.left, tiles, originTile);
        horizontal.AddRange(CollectInDirection(originIndex, Vector2Int.right, tiles, originTile));

        var vertical = CollectInDirection(originIndex, Vector2Int.down, tiles, originTile);
        vertical.AddRange(CollectInDirection(originIndex, Vector2Int.up, tiles, originTile));

        return new Match(originTile, horizontal, vertical);
    }

    private static List<TileController> CollectInDirection(Vector2Int origin, Vector2Int dir, Dictionary<Vector2Int, TileController> tiles, TileController matchAgainst)
    {
        var matches = new List<TileController>();
        for (Vector2Int pos = origin + dir; tiles.ContainsKey(pos); pos += dir)
        {
            var tile = tiles[pos];
            if (tile.GetModelTileType() == matchAgainst.GetModelTileType())
                matches.Add(tile);
            else
                break;
        }
        return matches;
    }
}