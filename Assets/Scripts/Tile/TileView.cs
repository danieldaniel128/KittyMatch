using System;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Tile
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] Transform _tileIconHolder;
        [SerializeField] private RawImage _regularIconImage;
        [SerializeField] private RawImage _selectedIconImage;
        [SerializeField] private Material _selectedMaterial;
        [SerializeField] private Material _selectedMaterialInstance;
        private RawImage _currentIconImage;
        public Transform Icon
        {
            get
            {
                if (_currentIconImage != null)
                {
                    _selectedMaterialInstance.SetTexture("_Texture2D", _currentIconImage.texture); // Update the material
                    return _currentIconImage.gameObject.transform.parent; // Return the GameObject
                }
                return null; // Handle the case where _currentIconImage is null
            }
        }

        private bool _isSelected;
        //[SerializeField] Vector2 _iconSize;
        public void SetNewTileIcon(Texture2D iconTexture)
        {
            if (iconTexture == null)
            {
                Debug.LogWarning("icon texture is null, cannot set tile icon.");
                return;
            }
            _selectedMaterialInstance = new Material(_selectedMaterial);
            // Set the texture of the RawImage to the sprite sheet texture
            _selectedIconImage.material = _selectedMaterialInstance;
            _regularIconImage.texture = iconTexture;
            _regularIconImage.SetNativeSize();
            _selectedIconImage.texture = iconTexture;
            _selectedIconImage.SetNativeSize();
            _currentIconImage = _regularIconImage;
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
            Icon.SetParent(_tileIconHolder);
        }
        public void ChangeIcon(Transform antoherIcon)
        {
            _regularIconImage = antoherIcon.GetChild(0).GetComponent<RawImage>();
            _selectedIconImage = antoherIcon.GetChild(1).GetComponent<RawImage>();
            _currentIconImage = _regularIconImage;
        }
        //make an handler that holds every vfx. ask him to show the current vfx of the tile state.
        public void UpdateSelectedVFXState(bool isSelected)
        {
            _regularIconImage.gameObject.SetActive(!isSelected);
            _selectedIconImage.gameObject.SetActive(isSelected);
            _currentIconImage = isSelected ? _selectedIconImage : _regularIconImage;
        }
        
    }
}
