using System;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Tile
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private Transform _tileIconHolder;
        
        [SerializeField] private IconHandler _currentIcon;
        public IconHandler Icon
        {
            get => _currentIcon;
            private set { _currentIcon = value; }
        }
        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; if (Icon != null) Icon.IsSelected = _isSelected; }
        }
        bool _hasPopped;
        public bool HasPopped
        {
            get { return _hasPopped; }
            set { _hasPopped = value;if(Icon!=null) Icon.IsPopping = _hasPopped; }
        }
        
        //[SerializeField] Vector2 _iconSize;
        public void SetNewTileIcon(Texture2D iconTexture, Color breakingVFXColor)
        {
            if (iconTexture == null)
            {
                Debug.LogWarning("icon texture is null, cannot set tile icon.");
                return;
            }
            Icon.SetIconImage(iconTexture, breakingVFXColor);
            
        }

        ///in case the texture is not in the currect size fitting to parent, make it fit approximetly _iconSize.
        //private void AdjustSize(Texture2D texture)
        //{
        //    float size_factor = 1;
        //    if (_icon.texture.width > _icon.texture.height) //landscape
        //    {
        //        size_factor = _iconSize.x / _icon.texture.width;
        //    }
        //    else //portrait
        //    {
        //        size_factor = _iconSize.y / _icon.texture.height;
        //    }
        //    _tileRectTransform.sizeDelta *= size_factor;
        //}
        public void ConnectIconToParent()
        {
            _currentIcon.transform.SetParent(_tileIconHolder);
        }
        public void ChangeIcon(IconHandler newIcon)
        {
            _currentIcon = newIcon;
        }
        //make an handler that holds every vfx. ask him to show the current vfx of the tile state.
       
        
    }
}
