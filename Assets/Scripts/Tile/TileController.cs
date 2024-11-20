using Assets.Scripts.Tile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TileController : MonoBehaviour, IPointerDownHandler, ITile
{
    [SerializeField] RawImage _icon;
    [SerializeField] RectTransform _tileRectTransform;
    [SerializeField] Transform _tileHolder;
    [SerializeField] TileDataSO _tileDataSO;
    protected TileModel _tileModel;
    protected TileView _tileView;
    public UnityEvent<TileController> OnSelectedTile;
    public int X { get; private set; }
    public int Y { get; private set; }
    [SerializeField] private Vector2Int _tileIndex;
    public Vector2Int TileIndex { get => _tileIndex; private set { _tileIndex = value; X = _tileIndex.x; Y = _tileIndex.y; } }

    public void Initialize(TileDataSO tileDataSO)
    {
        _icon.transform.SetParent(_tileHolder);
        _tileDataSO = tileDataSO;
        _tileModel = new TileModel(tileDataSO);
        //make static method of creating instance that does the set icon too.
        _tileView = new TileView(_icon, _icon.GetComponent<RectTransform>(), new Vector2(175,175));//size should be from the parent. change later.
        // Initializing the view based on model's data
        _tileView.SetNewTileIcon(_tileModel.TileData.TileIcon);
    }

    public  void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Tile clicked");

        // Request model to process the selection toggle and return the new state
        bool isSelected = _tileModel.ToggleSelection();

        // Update view based on the processed data
        OnSelectedTile?.Invoke(this);
    }
    public string GetModelTileType()
    {
       return _tileModel.GetTileType();
    }
    public Transform GetTileIconTransform()
    {
        return _tileRectTransform;
    }

    public void ChangeIcon(RawImage rawImage)
    {
        _icon = rawImage;
    }
    public RawImage GetIcon()
    {
        return _icon;
    }
    public void SetTileIndex(int x, int y)
    {
        TileIndex = new Vector2Int(x,y);
    }
    public void SetTileIndex(Vector2Int newTileIndex)
    {
        TileIndex = newTileIndex;
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
