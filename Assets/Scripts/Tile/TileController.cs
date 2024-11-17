using Assets.Scripts.Tile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TileController : MonoBehaviour, IPointerDownHandler, ITile
{
    [SerializeField] private PooledObject _pooledObject;
    [SerializeField] private TileDataSO _tileDataSO;
    [SerializeField] protected TileView _tileView;
    [SerializeField] private Vector2Int _tileIndex;//hide it later or making it readonly from inspector
    protected TileModel _tileModel;
    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector2Int TileIndex { get => _tileIndex; private set { _tileIndex = value; X = _tileIndex.x; Y = _tileIndex.y; } }
    public UnityEvent<TileController> OnTrySelectingTile;
    public UnityEvent<bool> OnSelectedTile;
    public UnityEvent<bool> OnDeSelectedTile;
    public bool IsSelected => _tileModel.IsSelected;
    public PooledObject PooledObject => _pooledObject;
    private TilePool _pool;
    private void Start()
    {
        OnSelectedTile.AddListener(ToggleSelection);
        OnDeSelectedTile.AddListener(ToggleSelection);
    }
    private void OnDestroy()
    {
        OnTrySelectingTile.RemoveAllListeners();
        OnSelectedTile.RemoveAllListeners();
        OnDeSelectedTile.RemoveAllListeners();
    }
    private void OnApplicationQuit()
    {
        OnTrySelectingTile.RemoveAllListeners();
        OnSelectedTile.RemoveAllListeners();
        OnDeSelectedTile.RemoveAllListeners();
    }
    public void Initialize(TileDataSO tileDataSO)
    {
        _tileDataSO = tileDataSO;
        _tileModel = new TileModel(tileDataSO);
        // Initializing the view based on model's data
        _tileView.SetNewTileIcon(_tileModel.TileData.TileIcon);
    }
    public void AttachPool(TilePool tilePool)
    {
        _pool = tilePool;
    }
    void ToggleSelection(bool isSelected)
    {
        _tileModel?.ToggleSelection(isSelected);
        _tileView?.UpdateSelectedVFXState(_tileModel.IsSelected);
        Debug.Log(_tileModel.IsSelected);
    }
    public  void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Tile clicked");

        // Request model to process the selection toggle and return the new state

        // Update view based on the processed data
        OnTrySelectingTile?.Invoke(this);
    }
    public string GetModelTileType()
    {
       return _tileModel.GetTileType();
    }
    
    public void SetTileIndex(int x, int y)
    {
        TileIndex = new Vector2Int(x,y);
    }
    public void SetTileIndex(Vector2Int newTileIndex)
    {
        TileIndex = newTileIndex;
    }
    public Transform GetIconTransform()
    {
        return _tileView.Icon;
    }
    public void ConnectIconToParent()
    {
        _tileView.ConnectIconToParent();
    }
    public void ChangeIcon(Transform newIcon)
    {
        _tileView.ChangeIcon(newIcon);
        if(newIcon!=null)
            _pooledObject = newIcon.GetComponent<PooledObject>();
    }
    public void ReleaseToPool()
    {
        _pool.ReturnToPool(_pooledObject);
        _pooledObject = null;
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
