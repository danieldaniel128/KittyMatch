using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    public List<Match> DetectMatches(List<ITile> board, int boardHeight)
    {
        List<Match> allMatches = new List<Match>();

        for (int y = 0; y < boardHeight; y++)
        {
            // Filter tiles that are in the current row
            List<ITile> tilesToCheck = board.Where(tile => tile.Y == y).ToList();

            // Find horizontal matches in the current row
            var horizontalMatches = tilesToCheck
                .GroupBy(tile => tile.TileIndex.y) // Group tiles by row (Y position)
                .SelectMany(rowGroup =>
                    rowGroup.GroupBy(tile => ((TileController)tile).GetModelTileType())) // Group by TileType
                .ToList();

            List<Match> rowMatches = new List<Match>(); // Holds all matches for this row

            foreach (var tileGroup in horizontalMatches)
            {
                Match tilesSequence = new Match(new List<ITile>());

                for (int i = 0; i < tileGroup.Count(); i++)
                {
                    ITile tile = tileGroup.ElementAt(i);
                    // Skip groups with only one tile as they can't form matches
                    if (tileGroup.Count() == 1)
                    {
                        //Debug.Log("type: " + tileGroup.Key + " tileIndex: " + tile.TileIndex);
                        continue;
                    }


                    // Start a new sequence with the first tile
                    if (tilesSequence.MatchTiles.Count == 0)
                    {
                        tilesSequence.MatchTiles.Add(tile);
                        //Debug.Log("type: " + tileGroup.Key + " tileIndex: " + tile.TileIndex);
                        continue;
                    }

                    // Check if the current tile is consecutive with the last tile in the sequence
                    if (tile.X - 1 == tilesSequence.MatchTiles[tilesSequence.MatchTiles.Count-1].X)
                    {
                        tilesSequence.MatchTiles.Add(tile); // Add to the sequence
                        //Debug.Log("type: " + tileGroup.Key + " tileIndex: " + tile.TileIndex);
                    }
                    else
                    {
                        // If sequence breaks and is a valid match, add to row matches
                        //if (tilesSequence.MatchTiles.Count >= 3)
                        //{
                            rowMatches.Add(tilesSequence);
                        //}
                        // Clear sequence to start new potential match
                        tilesSequence.MatchTiles.Clear();
                        tilesSequence.MatchTiles.Add(tile);
                    }
                }

                // After the loop, check if the last sequence in the row is a match
                if (tilesSequence.MatchTiles.Count >= 3)
                {
                    rowMatches.Add(tilesSequence);
                    string typeTile= (tilesSequence.MatchTiles.FirstOrDefault() as TileController).GetModelTileType();
                    Debug.Log("type: " + typeTile);
                    foreach (var tile in tilesSequence.MatchTiles)
                        Debug.Log("tiletileIndex: " + tile.TileIndex);
                }
            }

            // Add all row matches to the overall matches list
            allMatches.AddRange(rowMatches);
        }
        foreach (Match match in allMatches) 
        {
            foreach (var item in match.MatchTiles)
            {
                ((TileController)item).GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        return allMatches; // Returns all matches found across all rows
    }


}
public class Match//
{
    public List<ITile> MatchTiles { get; private set; }
    public Match(List<ITile> matchedTiles)
    {
        MatchTiles = matchedTiles;
    }
}
