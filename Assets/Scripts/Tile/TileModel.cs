using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tile
{
    public class TileModel
    {
        public TileDataSO TileData { get; private set; }
        public bool IsSelected { get; private set; }

        public TileModel(TileDataSO tileDataSO)
        {
            TileData = tileDataSO;
            IsSelected = false;
        }

        public void ToggleSelection(bool selected)
        {
            IsSelected = selected;
        }

        public void Reset()
        {
            IsSelected = false;
        }

        public string GetTileType()
        {
            return TileData.TileType;
        }
    }
}
