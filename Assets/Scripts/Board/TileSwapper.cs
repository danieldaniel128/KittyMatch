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

        await AnimateSwapAsync(iconA.transform, iconB.transform, tileA.transform.position, tileB.transform.position);

        ApplySwap(tileA, tileB, iconA, iconB);

        var matches = new MatchFinder(_tiles).FindMatches();
        if (matches.Count == 0)
        {
            await AnimateSwapAsync(iconA.transform, iconB.transform, tileB.transform.position, tileA.transform.position);
            UndoSwap(tileA, tileB, iconA, iconB);
        }
    }
    private async Task AnimateSwapAsync(Transform iconA, Transform iconB, Vector3 toA, Vector3 toB)
    {
        iconA.SetParent(_overlapParent);
        iconB.SetParent(_overlapParent);

        Sequence seq = DOTween.Sequence();
        seq.Join(iconA.DOMove(toB, 0.3f));
        seq.Join(iconB.DOMove(toA, 0.3f));
        await seq.Play().AsyncWaitForCompletion();
    }
    private void ApplySwap(TileController tileA, TileController tileB, IconHandler iconA, IconHandler iconB)
    {
        tileA.ChangeIcon(iconB);
        tileB.ChangeIcon(iconA);

        var dataA = tileA.GetModelTileType();
        var dataB = tileB.GetModelTileType();

        tileA.Initialize(GetTileData(dataB));
        tileB.Initialize(GetTileData(dataA));

        tileA.ConnectIconToParent();
        tileB.ConnectIconToParent();
    }
    private void UndoSwap(TileController tileA, TileController tileB, IconHandler iconA, IconHandler iconB)
    {
        tileA.ChangeIcon(iconA);
        tileB.ChangeIcon(iconB);

        var dataA = tileA.GetModelTileType();
        var dataB = tileB.GetModelTileType();

        tileA.Initialize(GetTileData(dataA));
        tileB.Initialize(GetTileData(dataB));

        tileA.ConnectIconToParent();
        tileB.ConnectIconToParent();
    }
    private TileDataSO GetTileData(string type)
    {
        return _specialTileTypes.Cast<TileDataSO>()
            .Concat(_tileTypes)
            .FirstOrDefault(d => d.TileType == type);
    }

}