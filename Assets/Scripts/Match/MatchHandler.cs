using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MatchHandler : MonoBehaviour
{
    public List<Match> DetectMatches(Dictionary<Vector2Int, TileController> boardTiles, int boardWidth, int boardHeight)
    {

        var matches = new List<Match>();
        foreach (var tile in boardTiles)
        {
            var (h, v) = GetConnections(tile.Key, boardTiles);
            var match = new Match(tile.Value, h.Cast<ITile>().ToList(), v.Cast<ITile>().ToList());
            // Only consider matches with a valid score.
            if (match.Score > -1)
            {
                if ((h.Count >= 2 || v.Count >= 2))
                {
                    bool hasAppeared = false;
                    foreach (var addedMatch in matches)
                    {
                        foreach (var addedTile in addedMatch.Tiles)
                        {
                            if (match.Tiles.Count(c => c.TileIndex == addedTile.TileIndex) >= 2)
                                hasAppeared = true;
                        }

                    }
                    if(!hasAppeared)
                        matches.Add(match);
                }
            }
        }
        var LShapedMatches = matches.Where(c => c.HorizontalCount >= 3 && c.VerticalCount >= 3).ToList();
        var everyOtherMatch = matches.Where(c => !(c.HorizontalCount >= 3 && c.VerticalCount >= 3)).ToList();
        foreach (var LShapedMatch in LShapedMatches) 
        {
            bool hasRemovedOverlappedLMatch=false;
            foreach(var otherMatch in everyOtherMatch)
            {
                foreach (var tile in otherMatch.Tiles)
                {
                    if (LShapedMatch.Tiles.Where(c => c.TileIndex == tile.TileIndex).ToList().Count == 1)
                    {
                        matches.Remove(otherMatch);
                        hasRemovedOverlappedLMatch = true;
                        continue;
                    }
                }
                if(hasRemovedOverlappedLMatch)
                    continue;
            }
        }
        return matches;
    }



    public static (List<TileController>, List<TileController>) GetConnections(Vector2Int originIndex, Dictionary<Vector2Int, TileController> tiles)
    {
        // Find the origin tile in the list based on its X and Y coordinates.
        var origin = tiles[originIndex];
        if (origin == null || origin.GetModelTileType().Equals("EmptyRendered")) return (new List<TileController>(), new List<TileController>());

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
        return (horizontalConnections, verticalConnections);
    }

}
public class Match
{
    public readonly string TileType;
    public readonly int Score;

    public readonly List<ITile> Tiles;
    public SpecialMatch MatchType { get; private set; }
    public readonly int HorizontalCount;
    public readonly int VerticalCount;


    public Match(ITile origin, List<ITile> horizontal, List<ITile> vertical)
    {
        TileType = ((TileController)origin).GetModelTileType();

        Tiles = new List<ITile> { origin }; // Initialize the list with the origin tile

        // Add horizontal and vertical tiles if they meet the minimum count
        if (horizontal.Count >= 2)
        {
            Tiles.AddRange(horizontal);
        }

        if (vertical.Count >= 2)
        {
            Tiles.AddRange(vertical);
        }

        // If there are not enough tiles to form a match, set Tiles to null
        if (Tiles.Count <= 1)
        {
            Tiles = null;
        }

        HorizontalCount = horizontal.Count >= 2 ? horizontal.Count + 1 : 0;
        VerticalCount = vertical.Count >= 2 ? vertical.Count + 1 : 0;

        AssignMatchType();
        Score = Tiles?.Count ?? -1;
    }
    public bool IsSpecial => IsLineFiveSpecial() || IsLineSpecial() || IsShapeSpecial();

    private void AssignMatchType()
    {
        if (IsSpecial)
            if (IsShapeSpecial())
            {
                MatchType = SpecialMatch.TShape;
            }
            else if (IsLineFiveSpecial())
                MatchType = SpecialMatch.FiveColumn;
            else
            {
                if (HorizontalCount == 4)
                    MatchType = SpecialMatch.FourRow;
                else
                    MatchType = SpecialMatch.FourColumn;
            }
    }
    private bool IsLineSpecial()
    {
        return HorizontalCount == 4 || VerticalCount == 4; // Line matches with 4 or more tiles
    }
    private bool IsLineFiveSpecial()
    {
        return HorizontalCount >= 5 || VerticalCount >= 5; // Line matches with 4 or more tiles
    }
    private bool IsShapeSpecial()
    {
        // Check if it's an L or T shape (requires connections in both directions)
        return HorizontalCount >= 3 && VerticalCount >= 3;
    }


}
//change enum later to custom shapes. the closest the shape is will return.
public enum SpecialMatch
{
    FourRow,
    FourColumn,
    FiveRow,
    FiveColumn,
    LShape,
    TShape
}
