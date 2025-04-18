using Assets.Scripts.Tile;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TileController : MonoBehaviour, ITile, IPointerDownHandler
{
    [SerializeField] private TileView _tileView;
    [SerializeField] private TileDataSO _tileDataSO;
    [SerializeField] private Vector2Int _tileIndex;

    private TileModel _tileModel;
    private TilePool _pool;
    private PooledObject _pooledObject;

    public UnityEvent<TileController> OnTrySelectingTile;
    public UnityEvent<bool> OnSelectedTile;
    public UnityEvent<bool> OnDeSelectedTile;

    public Vector2Int TileIndex => _tileIndex;
    public int X => _tileIndex.x;
    public int Y => _tileIndex.y;
    public bool IsSelected => _tileModel.IsSelected;
    public TileView TileView => _tileView;

    private void Start()
    {
        OnSelectedTile.AddListener(ToggleSelection);
        OnDeSelectedTile.AddListener(ToggleSelection);
    }

    private void OnDestroy()
    {
        OnSelectedTile.RemoveAllListeners();
        OnDeSelectedTile.RemoveAllListeners();
        OnTrySelectingTile.RemoveAllListeners();
    }

    public void Initialize(TileDataSO dataSO)
    {
        _tileDataSO = dataSO;
        _tileModel = new TileModel(_tileDataSO);
        _tileView.SetNewTileIcon(_tileDataSO.TileIcon, _tileDataSO.Color);
    }

    public void SetTileIndex(int x, int y)
    {
        _tileIndex = new Vector2Int(x, y);
    }

    public void SetTileIndex(Vector2Int index)
    {
        _tileIndex = index;
    }

    public string GetModelTileType() => _tileModel.GetTileType();

    public async Task AwaitPopIcon()
    {
        if (_tileView.Icon != null)
        {
            _tileView.HasPopped = true;
            await _tileView.Icon.AwaitPop();
        }
    }

    public void AttachPool(TilePool pool)
    {
        _pool = pool;
    }

    public void ChangeIcon(IconHandler icon)
    {
        _tileView.ChangeIcon(icon);
        _pooledObject = icon?.GetComponent<PooledObject>();
    }

    public void ConnectIconToParent() => _tileView.ConnectIconToParent();

    public void AssignSpecialIcon()
    {
        _tileView.Icon.IsSpecial = true;
        if (_tileDataSO is SpecialTileDataSO special)
        {
            if (special.SpecialMatchType == SpecialMatch.FourRow)
                _tileView.Icon.RotateSpecialToRow();
            else if (special.SpecialMatchType == SpecialMatch.FourColumn)
                _tileView.Icon.RotateSpecialToColumn();
        }
    }

    public void UnAssignSpecialIcon() => _tileView.Icon.IsSpecial = false;

    public void ReleaseToPool()
    {
        _pool?.ReturnToPool(_pooledObject);
        _pooledObject = null;
    }

    void ToggleSelection(bool selected)
    {
        _tileModel.ToggleSelection(selected);
        _tileView.IsSelected = selected;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTrySelectingTile?.Invoke(this);
        Debug.Log("selected tile");//
    }
}
public interface ITile
{
    int X { get; }
    int Y { get; }
    Vector2Int TileIndex { get; }
    public void SetTileIndex(int x,int y);
    public void SetTileIndex(Vector2Int newTileIndex);

}
