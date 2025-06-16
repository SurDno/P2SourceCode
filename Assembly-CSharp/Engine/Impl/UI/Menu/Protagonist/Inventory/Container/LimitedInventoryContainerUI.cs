// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.Container.LimitedInventoryContainerUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container
{
  [DisallowMultipleComponent]
  public class LimitedInventoryContainerUI : InventoryContainerUI
  {
    public static LimitedInventoryContainerUI Instantiate(
      IInventoryComponent container,
      InventoryCellStyle style,
      GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = "[Container] " + container.Owner.Name;
      LimitedInventoryContainerUI component1 = gameObject.GetComponent<LimitedInventoryContainerUI>();
      component1.content = (UIControl) component1;
      component1.InventoryContainer = container;
      Vector2 gridPosition = InventoryUtility.CalculateGridPosition(container.GetPosition().To(), style);
      component1.Transform.localPosition = (Vector3) gridPosition;
      component1.Transform.pivot = container.GetPivot().To();
      component1.Transform.anchorMax = container.GetAnchor().To();
      component1.Transform.anchorMin = container.GetAnchor().To();
      if (container.GetGrid() is IInventoryGridLimited)
      {
        IInventoryGridLimited grid = (IInventoryGridLimited) container.GetGrid();
        foreach (Cell cell in (IEnumerable<Cell>) ((InventoryGridLimited) grid).Cells)
        {
          InventoryCellUI inventoryCellUi = InventoryCellUI.Instantiate(cell, style);
          inventoryCellUi.Transform.SetParent(component1.grid.transform, false);
          component1.cells.Add(cell, inventoryCellUi);
        }
        Vector2 outerSize = InventoryUtility.CalculateOuterSize((IInventoryGridBase) grid, style);
        component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, outerSize.x);
        component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, outerSize.y);
        if ((UnityEngine.Object) style.OutlinePrefab != (UnityEngine.Object) null)
        {
          RectTransform component2 = UnityEngine.Object.Instantiate<GameObject>(style.OutlinePrefab).GetComponent<RectTransform>();
          component2.sizeDelta = new Vector2(outerSize.x + style.OutlineOffset.x * 2f, outerSize.y + style.OutlineOffset.y * 2f);
          component2.SetParent(component1.grid.transform, false);
        }
      }
      if ((UnityEngine.Object) component1.button != (UnityEngine.Object) null)
      {
        component1.button.OpenBeginEvent += new Action(((InventoryContainerUI) component1).FireOpenBegin);
        component1.button.OpenEndEvent += new Action<bool>(((InventoryContainerUI) component1).FireOpenEnd);
      }
      return component1;
    }
  }
}
