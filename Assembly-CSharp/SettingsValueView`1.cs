using System;
using Engine.Behaviours.Localization;
using Engine.Impl.UI.Controls;
using Engine.Source.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SettingsValueView<T> : 
  MonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  ISettingEntity,
  ISelectable
{
  [SerializeField]
  private HideableView interactableView;
  [SerializeField]
  private HideableView selectedView;
  [SerializeField]
  private Localizer nameText;
  [SerializeField]
  protected Localizer valueText;
  [SerializeField]
  protected Slider slider;
  private bool interactable = true;
  private bool selected;
  public Action<SettingsValueView<T>> VisibleValueChangeEvent;
  public Action<SettingsValueView<T>> PointerEnterEvent;
  public Action<SettingsValueView<T>> PointerExitEvent;

  protected IValue<T> SettingsValue { get; private set; }

  protected void FireVisibleValueChangeEvent()
  {
    Action<SettingsValueView<T>> valueChangeEvent = VisibleValueChangeEvent;
    if (valueChangeEvent == null)
      return;
    valueChangeEvent(this);
  }

  public void ResetValue()
  {
    if (SettingsValue == null)
      return;
    SettingsValue.Value = SettingsValue.DefaultValue;
    RevertVisibleValue();
  }

  public bool Interactable
  {
    get => interactable;
    set
    {
      if (interactable == value)
        return;
      interactable = value;
      if (!(interactableView != null))
        return;
      interactableView.Visible = interactable;
    }
  }

  public bool Selected
  {
    get => selected;
    set
    {
      if (selected == value)
        return;
      selected = value;
      if (!(selectedView != null))
        return;
      selectedView.Visible = value;
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    Action<SettingsValueView<T>> pointerEnterEvent = PointerEnterEvent;
    if (pointerEnterEvent == null)
      return;
    pointerEnterEvent(this);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    Action<SettingsValueView<T>> pointerExitEvent = PointerExitEvent;
    if (pointerExitEvent == null)
      return;
    pointerExitEvent(this);
  }

  public void SetName(string signature)
  {
    if (!(nameText != null))
      return;
    nameText.Signature = signature;
  }

  public void SetSetting(IValue<T> settingsValue) => SettingsValue = settingsValue;

  public abstract T VisibleValue { get; set; }

  public abstract void ApplyVisibleValue();

  public abstract void RevertVisibleValue();

  public abstract void IncrementValue();

  public abstract void DecrementValue();

  public bool IsActive() => gameObject.activeInHierarchy;

  public void OnSelect()
  {
    Action<SettingsValueView<T>> pointerEnterEvent = PointerEnterEvent;
    if (pointerEnterEvent == null)
      return;
    pointerEnterEvent(this);
  }

  public void OnDeSelect()
  {
    Action<SettingsValueView<T>> pointerExitEvent = PointerExitEvent;
    if (pointerExitEvent == null)
      return;
    pointerExitEvent(this);
  }
}
