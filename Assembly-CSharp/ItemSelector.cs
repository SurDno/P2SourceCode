// Decompiled with JetBrains decompiler
// Type: ItemSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
public class ItemSelector : MonoBehaviour, IChangeParameterListener
{
  private static List<IStorableComponent> searchBuffer = new List<IStorableComponent>();
  [SerializeField]
  private SwitchingItemView2 view = (SwitchingItemView2) null;
  [SerializeField]
  private EntityView itemEntityView = (EntityView) null;
  [SerializeField]
  public Button previousButton = (Button) null;
  [SerializeField]
  public Button nextButton = (Button) null;
  [SerializeField]
  private List<StorableGroup> groups = new List<StorableGroup>();
  [SerializeField]
  private bool avoidNull = false;
  [SerializeField]
  [FormerlySerializedAs("filterDurability")]
  private bool sortByDurability = false;
  [SerializeField]
  private bool ignoreBroken = false;
  [SerializeField]
  private Image selectedImage;
  [Inspected(Mode = ExecuteMode.Runtime, Mutable = false)]
  private IStorageComponent storage = (IStorageComponent) null;

  public event Func<ItemSelector, IStorableComponent, bool> ValidateItemEvent;

  public event Action<ItemSelector, IStorableComponent, IStorableComponent> ChangeItemEvent;

  public void SetSelection(bool selection) => this.selectedImage?.gameObject.SetActive(selection);

  public void CheckButtonsForConsole() => this.UpdateButtons();

  public bool AvoidNull => this.avoidNull;

  public List<StorableGroup> Groups
  {
    get => this.groups;
    set => this.groups = value;
  }

  public IStorableComponent Item
  {
    get => (IStorableComponent) this.view?.Storable;
    set
    {
      if ((UnityEngine.Object) this.view == (UnityEngine.Object) null)
        return;
      StorableComponent storable = this.view.Storable;
      if (storable == value)
        return;
      this.view.Storable = (StorableComponent) value;
      if ((UnityEngine.Object) this.itemEntityView != (UnityEngine.Object) null)
        this.itemEntityView.Value = value?.Owner;
      if (storable != null && (this.sortByDurability || this.ignoreBroken))
        storable.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability)?.RemoveListener((IChangeParameterListener) this);
      if (value != null && (this.sortByDurability || this.ignoreBroken))
        value.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability)?.AddListener((IChangeParameterListener) this);
      Action<ItemSelector, IStorableComponent, IStorableComponent> changeItemEvent = this.ChangeItemEvent;
      if (changeItemEvent == null)
        return;
      changeItemEvent(this, (IStorableComponent) storable, value);
    }
  }

  public IStorageComponent Storage
  {
    get => this.storage;
    set
    {
      if (this.storage == value)
        return;
      if (this.storage != null)
      {
        this.storage.OnAddItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnStorageContentChange);
        this.storage.OnChangeItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnStorageContentChange);
        this.storage.OnRemoveItemEvent -= new Action<IStorableComponent, IInventoryComponent>(this.OnRemoveItemInStorage);
      }
      this.storage = value;
      this.SelectDefaultItem();
      if (this.storage == null)
        return;
      this.storage.OnAddItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnStorageContentChange);
      this.storage.OnChangeItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnStorageContentChange);
      this.storage.OnRemoveItemEvent += new Action<IStorableComponent, IInventoryComponent>(this.OnRemoveItemInStorage);
    }
  }

  private void Awake()
  {
    if ((UnityEngine.Object) this.previousButton != (UnityEngine.Object) null)
      this.previousButton.onClick.AddListener(new UnityAction(this.SelectPrevious));
    if (!((UnityEngine.Object) this.nextButton != (UnityEngine.Object) null))
      return;
    this.nextButton.onClick.AddListener(new UnityAction(this.SelectNext));
  }

  private void SetButtonsInteractable(bool value)
  {
    if ((UnityEngine.Object) this.previousButton != (UnityEngine.Object) null)
      this.previousButton.interactable = value;
    if (!((UnityEngine.Object) this.nextButton != (UnityEngine.Object) null))
      return;
    this.nextButton.interactable = value;
  }

  private void ClearSearchBuffer() => ItemSelector.searchBuffer.Clear();

  private void FillSearchBuffer()
  {
    if (this.Storage == null)
      return;
    foreach (IStorableComponent storableComponent in this.Storage.Items)
    {
      bool flag = false;
      foreach (StorableGroup group1 in storableComponent.Groups)
      {
        foreach (StorableGroup group2 in this.groups)
        {
          if (group2 == group1)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          break;
      }
      if (flag && (!this.ignoreBroken || (double) this.GetDurability(storableComponent) > 0.0) && (this.ValidateItemEvent == null || this.ValidateItemEvent(this, storableComponent)))
        ItemSelector.searchBuffer.Add(storableComponent);
    }
    if (this.sortByDurability)
      ItemSelector.searchBuffer.Sort((Comparison<IStorableComponent>) ((x, y) => this.GetDurability(x).CompareTo(this.GetDurability(y))));
    this.SetButtonsInteractable(ItemSelector.searchBuffer.Count > (this.avoidNull ? 1 : 0));
  }

  private float GetDurability(IStorableComponent item)
  {
    IParameter<float> byName = item.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
    return byName != null ? byName.Value : 1f;
  }

  private void OnRemoveItemInStorage(IStorableComponent item, IInventoryComponent container)
  {
    if (item != this.Item)
      return;
    this.SelectDefaultItem();
  }

  private void OnStorageContentChange(IStorableComponent item, IInventoryComponent container)
  {
    this.UpdateButtons();
  }

  private void SelectDefaultItem()
  {
    this.view.ReversedDirection = false;
    this.FillSearchBuffer();
    this.Item = !this.avoidNull || ItemSelector.searchBuffer.Count <= 0 ? (IStorableComponent) null : ItemSelector.searchBuffer[0];
    this.ClearSearchBuffer();
  }

  public void SelectNext()
  {
    this.view.ReversedDirection = false;
    this.FillSearchBuffer();
    int num = -1;
    if (this.Item != null)
      num = ItemSelector.searchBuffer.IndexOf(this.Item);
    int index = num + 1;
    if (index == ItemSelector.searchBuffer.Count)
      index = !this.avoidNull || ItemSelector.searchBuffer.Count <= 0 ? -1 : 0;
    this.Item = index == -1 ? (IStorableComponent) null : ItemSelector.searchBuffer[index];
    this.ClearSearchBuffer();
  }

  public void SelectPrevious()
  {
    this.view.ReversedDirection = true;
    this.FillSearchBuffer();
    int num = -1;
    if (this.Item != null)
      num = ItemSelector.searchBuffer.IndexOf(this.Item);
    int index = num - 1;
    if (index == -1 && this.avoidNull || index == -2)
      index = ItemSelector.searchBuffer.Count - 1;
    this.Item = index == -1 ? (IStorableComponent) null : ItemSelector.searchBuffer[index];
    this.ClearSearchBuffer();
  }

  private void UpdateButtons()
  {
    this.FillSearchBuffer();
    this.ClearSearchBuffer();
  }

  public void OnParameterChanged(IParameter parameter)
  {
    Debug.Log((object) ObjectInfoUtility.GetStream().Append("Item Selector : Durability changed to ").Append(((IParameter<float>) parameter).Value));
    if ((double) ((IParameter<float>) parameter).Value > 0.0)
      return;
    this.SelectDefaultItem();
  }
}
