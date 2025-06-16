using System;
using Engine.Behaviours.Localization;
using Engine.Impl.UI.Controls;

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
  private bool selected;

  public bool Selected
  {
    get => selected;
    set
    {
      if (selected == value)
        return;
      selected = value;
      if (!((UnityEngine.Object) selectedView != (UnityEngine.Object) null))
        return;
      selectedView.Visible = value;
    }
  }

  public bool Interactable
  {
    get => interactable;
    set
    {
      if (interactable == value)
        return;
      interactable = value;
      if (!((UnityEngine.Object) interactableView != (UnityEngine.Object) null))
        return;
      interactableView.Visible = interactable;
    }
  }

  private void Awake() => button.onClick.AddListener(new UnityAction(OnClick));

  private void OnClick()
  {
    if (!interactable)
      return;
    Action<SelectableSettingsItemView> clickEvent = ClickEvent;
    if (clickEvent == null)
      return;
    clickEvent(this);
  }

  public void SetName(string value)
  {
    if (!((UnityEngine.Object) nameText != (UnityEngine.Object) null))
      return;
    nameText.Signature = value;
  }

  public void SetValue(string value)
  {
    if (!((UnityEngine.Object) valueText != (UnityEngine.Object) null))
      return;
    valueText.Signature = value;
  }
}
