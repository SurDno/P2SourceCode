using Engine.Behaviours.Localization;
using Engine.Impl.UI.Controls;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectableSettingsItemView : MonoBehaviour, ISelectable
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
  private Button button;
  public Action<SelectableSettingsItemView> ClickEvent;
  private bool interactable = true;
  private bool selected = false;

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

  private void Awake() => this.button.onClick.AddListener(new UnityAction(this.OnClick));

  private void OnClick()
  {
    if (!this.interactable)
      return;
    Action<SelectableSettingsItemView> clickEvent = this.ClickEvent;
    if (clickEvent == null)
      return;
    clickEvent(this);
  }

  public void SetName(string value)
  {
    if (!((UnityEngine.Object) this.nameText != (UnityEngine.Object) null))
      return;
    this.nameText.Signature = value;
  }

  public void SetValue(string value)
  {
    if (!((UnityEngine.Object) this.valueText != (UnityEngine.Object) null))
      return;
    this.valueText.Signature = value;
  }
}
