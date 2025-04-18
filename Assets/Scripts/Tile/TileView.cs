using System;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Tile
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private Transform _iconHolder;
        [SerializeField] private IconHandler _icon;

        public IconHandler Icon => _icon;

        public bool IsSelected
        {
            set { if (_icon != null) _icon.IsSelected = value; }
        }

        public bool HasPopped
        {
            set { if (_icon != null) _icon.IsPopping = value; }
        }

        public void SetNewTileIcon(Texture2D iconTexture, Color breakingColor)
        {
            if (iconTexture == null) return;
            _icon.SetIconImage(iconTexture, breakingColor);
        }

        public void ChangeIcon(IconHandler newIcon)
        {
            _icon = newIcon;
        }

        public void ConnectIconToParent()
        {
            if (_icon != null)
                _icon.transform.SetParent(_iconHolder);
        }
    }
}
