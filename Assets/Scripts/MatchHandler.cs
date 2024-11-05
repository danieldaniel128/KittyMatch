using System.Collections.Generic;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    public List<List<Tile>> DetectMatches(GridManager gridManager)
    {
        List<List<Tile>> matches = new List<List<Tile>>(); // List to store all detected matches

        // Horizontal Match Detection
        for (int y = 0; y < gridManager.Height; y++)
        {
            for (int x = 0; x < gridManager.Width; x++)
            {

            }
        }


        return matches; // Return all detected matches
    }

}
