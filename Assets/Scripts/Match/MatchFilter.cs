using System.Collections.Generic;
using System.Linq;

public static class MatchFilter
{
    public static List<Match> RemoveRedundantMatches(List<Match> matches)
    {
        var filtered = new List<Match>();
        foreach (var match in matches)
        {
            if (!filtered.Any(m => m.Tiles.Any(t => match.Tiles.Contains(t))))
            {
                filtered.Add(match);
            }
        }
        return filtered;
    }
}