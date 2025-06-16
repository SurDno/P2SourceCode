// Decompiled with JetBrains decompiler
// Type: ItemsSlidingContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using Engine.Common.Components;
using Engine.Impl.UI;
using Engine.Impl.UI.Menu.Protagonist.Inventory;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Container;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Grid;
using Engine.Source.Components;
using Engine.Source.UI.Menu.Protagonist.Inventory.Grid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class ItemsSlidingContainer : UIControl
{
  [SerializeField]
  private GameObject inventoryContainerPrefab;
  [SerializeField]
  private GameObject groupTitlePrefab;
  [SerializeField]
  private InventoryCellStyle cellStyle;
  [SerializeField]
  private RectTransform ContentArea;
  [SerializeField]
  private RectTransform Mask;
  [SerializeField]
  private GameObject inventoryStorablePrefab;
  [SerializeField]
  private ScrollRect scrollRect;
  private List<StorableUI> items = new List<StorableUI>();
  private List<GameObject> itemGroupTitles = new List<GameObject>();
  private List<InventoryContainerUI> containerViews = new List<InventoryContainerUI>();

  public void Clear(
    Dictionary<InventoryContainerUI, IStorageComponent> containers,
    Dictionary<IStorableComponent, StorableUI> storables)
  {
    foreach (Object itemGroupTitle in this.itemGroupTitles)
      Object.DestroyImmediate(itemGroupTitle);
    this.itemGroupTitles.Clear();
    foreach (InventoryContainerUI containerView in this.containerViews)
    {
      if (containerView is ComplexInventoryContainerUI)
      {
        foreach (IStorableComponent key in (containerView as ComplexInventoryContainerUI).Items)
        {
          if (key != null)
            storables.Remove(key);
        }
      }
      containers.Remove(containerView);
      Object.DestroyImmediate((Object) containerView.gameObject);
    }
    this.containerViews.Clear();
    this.scrollRect.verticalNormalizedPosition = 1f;
    this.items.Clear();
  }

  public List<StorableUI> ItemsUI => this.items;

  public void CreateSlots(
    List<List<StorableComponent>> itemsList,
    List<string> groupSignatures,
    StorageComponent storage,
    Dictionary<InventoryContainerUI, IStorageComponent> containers,
    Dictionary<IStorableComponent, StorableUI> storables)
  {
    for (int index = 0; index < itemsList.Count; ++index)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.groupTitlePrefab, (UnityEngine.Transform) this.ContentArea, false);
      gameObject.GetComponent<Localizer>().Signature = groupSignatures[index];
      this.itemGroupTitles.Add(gameObject);
      this.CreateSlots(itemsList[index], storage, containers, storables);
    }
  }

  public void CreateSlots(
    List<StorableComponent> itemsList,
    StorageComponent storage,
    Dictionary<InventoryContainerUI, IStorageComponent> containers,
    Dictionary<IStorableComponent, StorableUI> storables)
  {
    ComplexInventoryContainerUI key = ComplexInventoryContainerUI.Instantiate(this.cellStyle, this.inventoryContainerPrefab, itemsList, this.Mask);
    key.transform.SetParent((UnityEngine.Transform) this.ContentArea, false);
    this.containerViews.Add((InventoryContainerUI) key);
    List<StorableUI> collection = new List<StorableUI>();
    foreach (StorableComponent items in itemsList)
    {
      StorableUI storableUi = StorableUI.Instantiate((IStorableComponent) items, this.inventoryStorablePrefab, this.cellStyle.imageStyle);
      storableUi.Style = this.cellStyle;
      InventoryCellUI cellUi = key.GetCellUi(items.Cell);
      storableUi.transform.SetParent((UnityEngine.Transform) cellUi.Transform, false);
      storableUi.transform.localPosition = (Vector3) Vector2.zero;
      storables[(IStorableComponent) items] = storableUi;
      collection.Add(storableUi);
    }
    this.items.AddRange((IEnumerable<StorableUI>) collection);
    containers.Add((InventoryContainerUI) key, (IStorageComponent) storage);
  }

  public void FillForSelected(int index)
  {
    float height1 = this.scrollRect.gameObject.GetComponent<RectTransform>().rect.height;
    float height2 = this.inventoryStorablePrefab.GetComponent<RectTransform>().rect.height;
    float num = (float) ((double) (index + 1) * (double) height2 * 2.0);
    Vector2 anchoredPosition = this.ContentArea.anchoredPosition;
    if ((double) num - (double) anchoredPosition.y > (double) height1)
      anchoredPosition.y = (double) num + (double) anchoredPosition.y > (double) height1 ? num - height1 : 0.0f;
    else if ((double) num - (double) height2 * 2.0 - (double) anchoredPosition.y < 0.0)
      anchoredPosition.y = num - height2 * 2f;
    this.ContentArea.anchoredPosition = anchoredPosition;
  }

  public void ScrollTo(int index, int storableCount)
  {
    this.scrollRect.verticalNormalizedPosition = (float) (1.0 - (double) index / (double) (storableCount - 1));
  }
}
