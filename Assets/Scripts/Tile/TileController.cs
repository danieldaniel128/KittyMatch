using Assets.Scripts.Tile;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TileController : MonoBehaviour, IPointerDownHandler,ITile
{
    [SerializeField] RawImage _icon;
    [SerializeField] RectTransform _tileRectTransform;
    protected TileModel _tileModel;
    protected TileView _tileView;
    public UnityEvent<ITile> OnSelectedTile;

    public void Initialize(TileDataSO tileDataSO)
    {
        _tileModel = new TileModel(tileDataSO);
        _tileView = new TileView(_icon, _tileRectTransform,new Vector2(40,40));//size should be from the parent. change later.
        // Initializing the view based on model's data
        _tileView.SetNewTileIcon(_tileModel.TileData.TileIcon);
    }

    public  void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Tile clicked");

        // Request model to process the selection toggle and return the new state
        bool isSelected = _tileModel.ToggleSelection();

        // Update view based on the processed data
        OnSelectedTile?.Invoke(this);
    }

}
public interface ITile
{

}
