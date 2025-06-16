using Cofe.Utility;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (SelectableSettingsItemView))]
public class KeySettingsItemView : MonoBehaviour, ISettingEntity, ISelectable
{
  [SerializeField]
  private SelectableSettingsItemView selectable;
  [SerializeField]
  private Image keyIcon;
  private ActionGroup gameActionGroup;

  public SelectableSettingsItemView Selectable => this.selectable;

  public ActionGroup GameActionGroup
  {
    get => this.gameActionGroup;
    set
    {
      this.gameActionGroup = value;
      this.Selectable.SetName(InputUtility.GetTagName(value));
      this.OnJoystickUsedChanged(InputService.Instance.JoystickUsed);
    }
  }

  public bool Selected
  {
    get => this.Selectable.Selected;
    set => this.Selectable.Selected = value;
  }

  public bool Interactable
  {
    get => this.Selectable.Interactable;
    set => this.Selectable.Interactable = value;
  }

  public void DecrementValue()
  {
  }

  public void IncrementValue()
  {
  }

  public bool IsActive() => this.gameObject.activeInHierarchy;

  public void OnSelect()
  {
  }

  public void OnDeSelect()
  {
  }

  public void OnJoystickUsedChanged(bool value)
  {
    if (value)
    {
      this.Selectable.SetValue((string) null);
      string hotKeyNameByGroup = InputUtility.GetHotKeyNameByGroup(this.gameActionGroup, true);
      Sprite iconSprite = hotKeyNameByGroup.IsNullOrEmpty() ? (Sprite) null : ControlIconsManager.Instance.GetIconSprite(hotKeyNameByGroup);
      this.keyIcon.sprite = iconSprite;
      this.keyIcon.gameObject.SetActive(true);
      this.gameObject.SetActive((Object) iconSprite != (Object) null);
      this.Interactable = true;
    }
    else
    {
      this.Selectable.SetValue(InputUtility.GetHotKeyNameByGroup(this.gameActionGroup, false));
      this.keyIcon.sprite = (Sprite) null;
      this.keyIcon.gameObject.SetActive(false);
      this.gameObject.SetActive(true);
      this.Interactable = this.gameActionGroup.IsChangeble;
    }
  }
}
