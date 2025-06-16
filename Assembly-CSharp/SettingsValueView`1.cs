using Engine.Behaviours.Localization;
using Engine.Impl.UI.Controls;
using Engine.Source.Settings;
using System;
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
  private bool selected = false;
  public Action<SettingsValueView<T>> VisibleValueChangeEvent;
  public Action<SettingsValueView<T>> PointerEnterEvent;
  public Action<SettingsValueView<T>> PointerExitEvent;

  protected IValue<T> SettingsValue { get; private set; }

  protected void FireVisibleValueChangeEvent()
  {
    Action<SettingsValueView<T>> valueChangeEvent = this.VisibleValueChangeEvent;
    if (valueChangeEvent == null)
      return;
    valueChangeEvent(this);
  }

  public void ResetValue()
  {
    if (this.SettingsValue == null)
      return;
    this.SettingsValue.Value = this.SettingsValue.DefaultValue;
    this.RevertVisibleValue();
  }

  public bool Interactable
  {
    get => this.interactable;
    set
    {
      if (this.interactable == value)
        return;
      this.interactable = value;
      if (!((UnityEngine.Object) this.interactableView != (UnityEngine.Object) null))
        return;
      this.interactableView.Visible = this.interactable;
    }
  }

  public bool Selected
  {
    get => this.selected;
    set
    {
      if (this.selected == value)
        return;
      this.selected = value;
      if (!((UnityEngine.Object) this.selectedView != (UnityEngine.Object) null))
        return;
      this.selectedView.Visible = value;
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    Action<SettingsValueView<T>> pointerEnterEvent = this.PointerEnterEvent;
    if (pointerEnterEvent == null)
      return;
    pointerEnterEvent(this);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    Action<SettingsValueView<T>> pointerExitEvent = this.PointerExitEvent;
    if (pointerExitEvent == null)
      return;
    pointerExitEvent(this);
  }

  public void SetName(string signature)
  {
    if (!((UnityEngine.Object) this.nameText != (UnityEngine.Object) null))
      return;
    this.nameText.Signature = signature;
  }

  public void SetSetting(IValue<T> settingsValue) => this.SettingsValue = settingsValue;

  public abstract T VisibleValue { get; set; }

  public abstract void ApplyVisibleValue();

  public abstract void RevertVisibleValue();

  public abstract void IncrementValue();

  public abstract void DecrementValue();

  public bool IsActive() => this.gameObject.activeInHierarchy;

  public void OnSelect()
  {
    Action<SettingsValueView<T>> pointerEnterEvent = this.PointerEnterEvent;
    if (pointerEnterEvent == null)
      return;
    pointerEnterEvent(this);
  }

  public void OnDeSelect()
  {
    Action<SettingsValueView<T>> pointerExitEvent = this.PointerExitEvent;
    if (pointerExitEvent == null)
      return;
    pointerExitEvent(this);
  }
}
