// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.Inventory.Grid.InventoryCellStyle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
