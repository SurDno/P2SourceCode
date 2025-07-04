﻿using System;
using Cofe.Proxies;
using Engine.Common.Components;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Container
{
  [DisallowMultipleComponent]
  public class InfinitedInventoryContainerUI : InventoryContainerUI
  {
    [SerializeField]
    [FormerlySerializedAs("_ScrollBarHorizontal")]
    private Scrollbar scrollBarHorizontal;
    [SerializeField]
    [FormerlySerializedAs("_ScrollBarVertical")]
    private Scrollbar scrollBarVertical;

    public static InfinitedInventoryContainerUI Instantiate(
      IInventoryComponent container,
      InventoryCellStyle style,
      GameObject prefab)
    {
      GameObject gameObject = Instantiate(prefab);
      gameObject.name = "[Container] " + container.Owner.Name;
      InfinitedInventoryContainerUI component1 = gameObject.GetComponent<InfinitedInventoryContainerUI>();
      component1.InventoryContainer = container;
      component1.Transform.localPosition = container.GetPosition().To();
      if (container.GetGrid() is InventoryGridInfinited)
      {
        InventoryGridInfinited grid = (InventoryGridInfinited) container.GetGrid();
        for (int index1 = 0; index1 < grid.Columns; ++index1)
        {
          for (int index2 = 0; index2 < grid.Rows; ++index2)
          {
            Cell cell = ProxyFactory.Create<Cell>();
            cell.Column = index1;
            cell.Row = index2;
            InventoryCellUI inventoryCellUi = InventoryCellUI.Instantiate(cell, style);
            inventoryCellUi.transform.SetParent(component1.grid.transform, false);
            component1.cells.Add(cell, inventoryCellUi);
          }
        }
        RectTransform component2 = component1.content.GetComponent<RectTransform>();
        Vector2 outerSize = InventoryUtility.CalculateOuterSize(grid, style);
        component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, outerSize.x);
        component1.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, outerSize.y);
        component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, outerSize.x);
        component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, outerSize.y);
        if (style.OutlinePrefab != null)
        {
          RectTransform component3 = Instantiate(style.OutlinePrefab).GetComponent<RectTransform>();
          component3.sizeDelta = new Vector2(outerSize.x + style.OutlineOffset.x * 2f, outerSize.y + style.OutlineOffset.y * 2f);
          component3.SetParent(component1.grid.transform, false);
        }
        switch (grid.Direction)
        {
          case DirectionKind.Vertical:
            component1.scrollBarVertical.value = 0.0f;
            break;
          case DirectionKind.Horizontal:
            component1.scrollBarHorizontal.value = 0.0f;
            break;
        }
      }
      if (component1.button != null)
      {
        component1.button.OpenBeginEvent += component1.FireOpenBegin;
        component1.button.OpenEndEvent += component1.FireOpenEnd;
      }
      return component1;
    }
  }
}
