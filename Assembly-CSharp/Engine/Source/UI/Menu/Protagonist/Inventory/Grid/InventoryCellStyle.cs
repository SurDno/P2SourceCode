using System;
using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.Inventory.Grid
{
  [Serializable]
  public struct InventoryCellStyle
  {
    public Vector2 Size;
    public Vector2 Offset;
    public bool IsSlot;
    public InventoryCellSizeEnum imageStyle;
    public GameObject Prefab;
    public GameObject OutlinePrefab;
    public Vector2 OutlineOffset;
    public Vector2 BackgroundImageOffset;
  }
}
