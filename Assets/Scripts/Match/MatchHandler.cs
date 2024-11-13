using System.Collections.Generic;
using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    public List<Match> DetectMatches(ITile[,] board)
    {
       return new List<Match>();
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
