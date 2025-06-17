using System;
using System.Collections.Generic;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour, IChangeParameterListener
{
  private static List<IStorableComponent> searchBuffer = [];
  [SerializeField]
  private SwitchingItemView2 view;
  [SerializeField]
  private EntityView itemEntityView;
  [SerializeField]
  public Button previousButton;
  [SerializeField]
  public Button nextButton;
  [SerializeField]
  private List<StorableGroup> groups = [];
  [SerializeField]
  private bool avoidNull;
  [SerializeField]
  [FormerlySerializedAs("filterDurability")]
  private bool sortByDurability;
  [SerializeField]
  private bool ignoreBroken;
  [SerializeField]
  private Image selectedImage;
  [Inspected(Mode = ExecuteMode.Runtime, Mutable = false)]
  private IStorageComponent storage;

  public event Func<ItemSelector, IStorableComponent, bool> ValidateItemEvent;

  public event Action<ItemSelector, IStorableComponent, IStorableComponent> ChangeItemEvent;

  public void SetSelection(bool selection) => selectedImage?.gameObject.SetActive(selection);

  public void CheckButtonsForConsole() => UpdateButtons();

  public bool AvoidNull => avoidNull;

  public List<StorableGroup> Groups
  {
    get => groups;
    set => groups = value;
  }

  public IStorableComponent Item
  {
    get => view?.Storable;
    set
    {
      if (view == null)
        return;
      StorableComponent storable = view.Storable;
      if (storable == value)
        return;
      view.Storable = (StorableComponent) value;
      if (itemEntityView != null)
        itemEntityView.Value = value?.Owner;
      if (storable != null && (sortByDurability || ignoreBroken))
        storable.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability)?.RemoveListener(this);
      if (value != null && (sortByDurability || ignoreBroken))
        value.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability)?.AddListener(this);
      Action<ItemSelector, IStorableComponent, IStorableComponent> changeItemEvent = ChangeItemEvent;
      if (changeItemEvent == null)
        return;
      changeItemEvent(this, storable, value);
    }
  }

  public IStorageComponent Storage
  {
    get => storage;
    set
    {
      if (storage == value)
        return;
      if (storage != null)
      {
        storage.OnAddItemEvent -= OnStorageContentChange;
        storage.OnChangeItemEvent -= OnStorageContentChange;
        storage.OnRemoveItemEvent -= OnRemoveItemInStorage;
      }
      storage = value;
      SelectDefaultItem();
      if (storage == null)
        return;
      storage.OnAddItemEvent += OnStorageContentChange;
      storage.OnChangeItemEvent += OnStorageContentChange;
      storage.OnRemoveItemEvent += OnRemoveItemInStorage;
    }
  }

  private void Awake()
  {
    if (previousButton != null)
      previousButton.onClick.AddListener(SelectPrevious);
    if (!(nextButton != null))
      return;
    nextButton.onClick.AddListener(SelectNext);
  }

  private void SetButtonsInteractable(bool value)
  {
    if (previousButton != null)
      previousButton.interactable = value;
    if (!(nextButton != null))
      return;
    nextButton.interactable = value;
  }

  private void ClearSearchBuffer() => searchBuffer.Clear();

  private void FillSearchBuffer()
  {
    if (Storage == null)
      return;
    foreach (IStorableComponent storableComponent in Storage.Items)
    {
      bool flag = false;
      foreach (StorableGroup group1 in storableComponent.Groups)
      {
        foreach (StorableGroup group2 in groups)
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
      if (flag && (!ignoreBroken || GetDurability(storableComponent) > 0.0) && (ValidateItemEvent == null || ValidateItemEvent(this, storableComponent)))
        searchBuffer.Add(storableComponent);
    }
    if (sortByDurability)
      searchBuffer.Sort((x, y) => GetDurability(x).CompareTo(GetDurability(y)));
    SetButtonsInteractable(searchBuffer.Count > (avoidNull ? 1 : 0));
  }

  private float GetDurability(IStorableComponent item)
  {
    IParameter<float> byName = item.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Durability);
    return byName != null ? byName.Value : 1f;
  }

  private void OnRemoveItemInStorage(IStorableComponent item, IInventoryComponent container)
  {
    if (item != Item)
      return;
    SelectDefaultItem();
  }

  private void OnStorageContentChange(IStorableComponent item, IInventoryComponent container)
  {
    UpdateButtons();
  }

  private void SelectDefaultItem()
  {
    view.ReversedDirection = false;
    FillSearchBuffer();
    Item = !avoidNull || searchBuffer.Count <= 0 ? null : searchBuffer[0];
    ClearSearchBuffer();
  }

  public void SelectNext()
  {
    view.ReversedDirection = false;
    FillSearchBuffer();
    int num = -1;
    if (Item != null)
      num = searchBuffer.IndexOf(Item);
    int index = num + 1;
    if (index == searchBuffer.Count)
      index = !avoidNull || searchBuffer.Count <= 0 ? -1 : 0;
    Item = index == -1 ? null : searchBuffer[index];
    ClearSearchBuffer();
  }

  public void SelectPrevious()
  {
    view.ReversedDirection = true;
    FillSearchBuffer();
    int num = -1;
    if (Item != null)
      num = searchBuffer.IndexOf(Item);
    int index = num - 1;
    if (index == -1 && avoidNull || index == -2)
      index = searchBuffer.Count - 1;
    Item = index == -1 ? null : searchBuffer[index];
    ClearSearchBuffer();
  }

  private void UpdateButtons()
  {
    FillSearchBuffer();
    ClearSearchBuffer();
  }

  public void OnParameterChanged(IParameter parameter)
  {
    Debug.Log(ObjectInfoUtility.GetStream().Append("Item Selector : Durability changed to ").Append(((IParameter<float>) parameter).Value));
    if (((IParameter<float>) parameter).Value > 0.0)
      return;
    SelectDefaultItem();
  }
}
