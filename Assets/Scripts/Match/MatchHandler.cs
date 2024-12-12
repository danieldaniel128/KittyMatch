using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MatchHandler : MonoBehaviour
{
    public List<Match> DetectMatches(Dictionary<Vector2Int, TileController> boardTiles,int boardWidth, int boardHeight)
    {

        var matches = new List<Match>();
        foreach (var tile in boardTiles)
        {
            var (h, v) = GetConnections(tile.Key, boardTiles);
            var match = new Match(tile.Value, h, v);
            // Only consider matches with a valid score.
            if (match.Score > -1)
            {
                // Check if the match is unique
                bool isUniqueMatch = !matches.Any(existingMatch =>
                    existingMatch.Tiles.Count(t1 =>
                        match.Tiles.Any(t2 => t1.X == t2.X && t1.Y == t2.Y)
                    ) >= 2
                );

                if (isUniqueMatch)
                    matches.Add(match);
            }
        }

        return matches;
    }

    public static (TileController[], TileController[]) GetConnections(Vector2Int originIndex, Dictionary<Vector2Int, TileController> tiles)
    {
        // Find the origin tile in the list based on its X and Y coordinates.
        var origin = tiles[originIndex];
        if (origin == null || origin.GetModelTileType().Equals("EmptyRendered")) return (new TileController[0], new TileController[0]);

        var horizontalConnections = new List<TileController>();
        var verticalConnections = new List<TileController>();

        TileController other = null;
        // Horizontal connections to the left
        for (var x = originIndex.x - 1; x >= 0; x--)
        {
            if (!tiles.TryGetValue(new Vector2Int(x, originIndex.y), out other) || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
            horizontalConnections.Add(other);
        }

        // Horizontal connections to the right
        for (var x = originIndex.x + 1; x < int.MaxValue; x++) // Remove int.MaxValue limitation if unnecessary
        {
            if (!tiles.TryGetValue(new Vector2Int(x,originIndex.y), out other) || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
                horizontalConnections.Add(other);
        }

        // Vertical connections upwards
        for (var y = originIndex.y - 1; y >= 0; y--)
        {
            if (!tiles.TryGetValue(new Vector2Int(originIndex.x, y),out other) || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
                verticalConnections.Add(other);
        }

        // Vertical connections downwards
        for (var y = originIndex.y + 1; y < int.MaxValue; y++) // Remove int.MaxValue limitation if unnecessary
        {
            if (!tiles.TryGetValue(new Vector2Int(originIndex.x, y), out other) || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
                verticalConnections.Add(other);
        }
        return (horizontalConnections.ToArray(), verticalConnections.ToArray());
    }

}
public class Match
{
    public readonly string TileType;

    public readonly int Score;

    public readonly ITile[] Tiles;

    public Match(ITile origin, ITile[] horizontal, ITile[] vertical)
    {
        TileType = ((TileController)origin).GetModelTileType();

        if (horizontal.Length >= 2 && vertical.Length >= 2)
        {
            Tiles = new ITile[horizontal.Length + vertical.Length + 1];

            Tiles[0] = origin;

            horizontal.CopyTo(Tiles, 1);

            vertical.CopyTo(Tiles, horizontal.Length + 1);
        }
        else if (horizontal.Length >= 2)
        {
            Tiles = new ITile[horizontal.Length + 1];

            Tiles[0] = origin;

            horizontal.CopyTo(Tiles, 1);
        }
        else if (vertical.Length >= 2)
        {
            Tiles = new ITile[vertical.Length + 1];

            Tiles[0] = origin;

            vertical.CopyTo(Tiles, 1);
        }
        else Tiles = null;

        Score = Tiles?.Length ?? -1;
    }
}
