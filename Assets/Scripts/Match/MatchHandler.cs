using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    public List<Match> DetectMatches(List<TileController> boardTiles, int boardHeight)
    {

        var matches = new List<Match>();
        foreach (var tile in boardTiles)
        {
            var (h, v) = GetConnections(tile.X, tile.Y, boardTiles);
            var match = new Match(tile, h, v);
            if(matches != null)
            {
                if (match.Score > -1)
                {
                    if(matches.Count ==0)
                            matches.Add(match);
                        else if (matches.Count > 0)
                        {
                            if (!matches.Any(existingMatch => existingMatch.Tiles.
                            Count(//count tiles with the same index
                                t1 => 
                                match.Tiles.Any(t2 => t1.X == t2.X && t1.Y == t2.Y)//check if there are tiles with the same index
                            ) >= 2)
                                )
                                matches.Add(match);
                        } 
                } 

            }
        }

        return matches;
    }

    public static (TileController[], TileController[]) GetConnections(int originX, int originY, List<TileController> tiles)
    {
        // Find the origin tile in the list based on its X and Y coordinates.
        var origin = tiles.FirstOrDefault(tile => tile.X == originX && tile.Y == originY);
        if (origin == null || origin.GetModelTileType().Equals("EmptyRendered")) return (new TileController[0], new TileController[0]);

        var horizontalConnections = new List<TileController>();
        var verticalConnections = new List<TileController>();

        // Horizontal connections to the left
        for (var x = originX - 1; x >= 0; x--)
        {
            var other = tiles.FirstOrDefault(tile => tile.X == x && tile.Y == originY);
            if (other == null || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
            horizontalConnections.Add(other);
        }

        // Horizontal connections to the right
        for (var x = originX + 1; x < int.MaxValue; x++) // Remove int.MaxValue limitation if unnecessary
        {
            var other = tiles.FirstOrDefault(tile => tile.X == x && tile.Y == originY);
            if (other == null || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
            horizontalConnections.Add(other);
        }

        // Vertical connections upwards
        for (var y = originY - 1; y >= 0; y--)
        {
            var other = tiles.FirstOrDefault(tile => tile.X == originX && tile.Y == y);
            if (other == null || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
            verticalConnections.Add(other);
        }

        // Vertical connections downwards
        for (var y = originY + 1; y < int.MaxValue; y++) // Remove int.MaxValue limitation if unnecessary
        {
            var other = tiles.FirstOrDefault(tile => tile.X == originX && tile.Y == y);
            if (other == null || !other.GetModelTileType().Equals(origin.GetModelTileType())) break;
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
