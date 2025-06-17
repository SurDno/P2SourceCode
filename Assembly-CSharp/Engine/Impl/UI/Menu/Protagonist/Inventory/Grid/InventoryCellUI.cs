using System.Collections.Generic;
using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Grid
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof (Image))]
  public class InventoryCellUI : UIControl
  {
    public static GameObject cellGridPrefab = null;
    public static GameObject cellStorablePrefab = null;
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private Image image;
    [SerializeField]
    [FormerlySerializedAs("_State")]
    private CellState state = CellState.Default;
    [SerializeField]
    [FormerlySerializedAs("_StateTextures")]
    private List<Sprite> stateTextures = [];

    public CellState State
    {
      get => state;
      set
      {
        state = value;
        if (image.IsDestroyed())
          return;
        image.sprite = stateTextures[(int) state];
      }
    }

    public static InventoryCellUI Instantiate(Cell cell, InventoryCellStyle style)
    {
      GameObject gameObject = Instantiate(style.Prefab);
      gameObject.name = "[Cell] " + cell.Column + " ; " + cell.Row;
      InventoryCellUI component = gameObject.GetComponent<InventoryCellUI>();
      Vector2 gridPosition = InventoryUtility.CalculateGridPosition(cell, style);
      component.Transform.localPosition = gridPosition;
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, style.Size.x);
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, style.Size.y);
      component.State = CellState.Default;
      return component;
    }

    public static InventoryCellUI Instantiate(Vector2 pos, InventoryCellStyle style)
    {
      GameObject gameObject = Instantiate(style.Prefab);
      gameObject.name = "[Cell]";
      InventoryCellUI component = gameObject.GetComponent<InventoryCellUI>();
      Vector2 gridPosition = InventoryUtility.CalculateGridPosition(pos, style);
      component.Transform.localPosition = gridPosition;
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, style.Size.x);
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, style.Size.y);
      component.State = CellState.Default;
      return component;
    }
  }
}
