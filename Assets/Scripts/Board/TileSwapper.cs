using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;

public class TileSwapper
{
    private Dictionary<Vector2Int, TileController> _tiles;
    private TileDataSO[] _tileTypes;
    private SpecialTileDataSO[] _specialTileTypes;
    private Transform _overlapParent;

    public TileSwapper(Dictionary<Vector2Int, TileController> tiles, TileDataSO[] tileTypes, SpecialTileDataSO[] specialTypes, Transform overlapParent)
    {
        _tiles = tiles;
        _tileTypes = tileTypes;
        _specialTileTypes = specialTypes;
        _overlapParent = overlapParent;
    }

    public bool TrySwap(Vector2Int a, Vector2Int b)
    {
        if (!_tiles.TryGetValue(a, out var tileA) || !_tiles.TryGetValue(b, out var tileB))
            return false;

        var dataA = tileA.GetModelTileType();
        var dataB = tileB.GetModelTileType();

        tileA.Initialize(_specialTileTypes.Cast<TileDataSO>().Concat(_tileTypes).FirstOrDefault(d => d.TileType == dataB));
        tileB.Initialize(_specialTileTypes.Cast<TileDataSO>().Concat(_tileTypes).FirstOrDefault(d => d.TileType == dataA));

        var matches = new MatchFinder(_tiles).FindMatches();

        tileA.Initialize(_specialTileTypes.Cast<TileDataSO>().Concat(_tileTypes).FirstOrDefault(d => d.TileType == dataA));
        tileB.Initialize(_specialTileTypes.Cast<TileDataSO>().Concat(_tileTypes).FirstOrDefault(d => d.TileType == dataB));

        return matches.Count > 0;
    }

    public async Task PerformSwapAsync(Vector2Int a, Vector2Int b)
    {
        if (!_tiles.TryGetValue(a, out var tileA) || !_tiles.TryGetValue(b, out var tileB))
            return;

        var iconA = tileA.TileView.Icon;
        var iconB = tileB.TileView.Icon;

        tileA.TileView.Icon.transform.SetParent(_overlapParent);
        tileB.TileView.Icon.transform.SetParent(_overlapParent);

        Vector3 posA = tileA.transform.position;
        Vector3 posB = tileB.transform.position;

        Sequence seq = DOTween.Sequence();
        seq.Join(iconA.transform.DOMove(posB, 0.3f));
        seq.Join(iconB.transform.DOMove(posA, 0.3f));
        await seq.Play().AsyncWaitForCompletion();

        tileA.ChangeIcon(iconB);
        tileB.ChangeIcon(iconA);

        var dataA = tileA.GetModelTileType();
        var dataB = tileB.GetModelTileType();

        tileA.Initialize(_specialTileTypes.Cast<TileDataSO>().Concat(_tileTypes).FirstOrDefault(d => d.TileType == dataB));
        tileB.Initialize(_specialTileTypes.Cast<TileDataSO>().Concat(_tileTypes).FirstOrDefault(d => d.TileType == dataA));

        tileA.ConnectIconToParent();
        tileB.ConnectIconToParent();
    }
}