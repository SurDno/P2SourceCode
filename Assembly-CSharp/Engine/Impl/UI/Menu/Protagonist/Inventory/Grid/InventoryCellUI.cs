using Engine.Source.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Grid
{
  [DisallowMultipleComponent]
  [RequireComponent(typeof (Image))]
  public class InventoryCellUI : UIControl
  {
    public static GameObject cellGridPrefab = (GameObject) null;
    public static GameObject cellStorablePrefab = (GameObject) null;
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private Image image = (Image) null;
    [SerializeField]
    [FormerlySerializedAs("_State")]
    private CellState state = CellState.Default;
    [SerializeField]
    [FormerlySerializedAs("_StateTextures")]
    private List<Sprite> stateTextures = new List<Sprite>();

    public CellState State
    {
      get => this.state;
      set
      {
        this.state = value;
        if (this.image.IsDestroyed())
          return;
        this.image.sprite = this.stateTextures[(int) this.state];
      }
    }

    public static InventoryCellUI Instantiate(Cell cell, InventoryCellStyle style)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(style.Prefab);
      gameObject.name = "[Cell] " + (object) cell.Column + " ; " + (object) cell.Row;
      InventoryCellUI component = gameObject.GetComponent<InventoryCellUI>();
      Vector2 gridPosition = InventoryUtility.CalculateGridPosition(cell, style);
      component.Transform.localPosition = (Vector3) gridPosition;
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, style.Size.x);
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, style.Size.y);
      component.State = CellState.Default;
      return component;
    }

    public static InventoryCellUI Instantiate(Vector2 pos, InventoryCellStyle style)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(style.Prefab);
      gameObject.name = "[Cell]";
      InventoryCellUI component = gameObject.GetComponent<InventoryCellUI>();
      Vector2 gridPosition = InventoryUtility.CalculateGridPosition(pos, style);
      component.Transform.localPosition = (Vector3) gridPosition;
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, style.Size.x);
      component.Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, style.Size.y);
      component.State = CellState.Default;
      return component;
    }
  }
}
