using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    public List<Match> DetectMatches(TileController[,] board, int boardHeight)
    {

        var matches = new List<Match>();
        foreach (var tile in board)
        {
            var (h, v) = GetConnections(tile.X, tile.Y, board);
            var match = new Match(tile, h, v);

            if (match.Score > -1) matches.Add(match);
        }

        return matches;
    }

    public static (TileController[], TileController[]) GetConnections(int originX, int originY, TileController[,] tiles)
    {
        var origin = tiles[originX, originY];

        var width = tiles.GetLength(0);
        var height = tiles.GetLength(1);

        var horizontalConnections = new List<TileController>();
        var verticalConnections = new List<TileController>();

        for (var x = originX - 1; x >= 0; x--)
        {
            var other = tiles[x, originY];

            if (!other.GetModelTileType().Equals(origin.GetModelTileType())) break;

            horizontalConnections.Add(other);
        }

        for (var x = originX + 1; x < width; x++)
        {
            var other = tiles[x, originY];

            if (!other.GetModelTileType().Equals(origin.GetModelTileType())) break;

            horizontalConnections.Add(other);
        }

        for (var y = originY - 1; y >= 0; y--)
        {
            var other = tiles[originX, y];

            if (!other.GetModelTileType().Equals(origin.GetModelTileType())) break;

            verticalConnections.Add(other);
        }

        for (var y = originY + 1; y < height; y++)
        {
            var other = tiles[originX, y];

            if (!other.GetModelTileType().Equals(origin.GetModelTileType())) break;

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
