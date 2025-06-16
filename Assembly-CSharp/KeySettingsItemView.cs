using Cofe.Utility;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;

[RequireComponent(typeof (SelectableSettingsItemView))]
public class KeySettingsItemView : MonoBehaviour, ISettingEntity, ISelectable
{
  [SerializeField]
  private SelectableSettingsItemView selectable;
  [SerializeField]
  private Image keyIcon;
  private ActionGroup gameActionGroup;

  public SelectableSettingsItemView Selectable => selectable;

  public ActionGroup GameActionGroup
  {
    get => gameActionGroup;
    set
    {
      gameActionGroup = value;
      Selectable.SetName(InputUtility.GetTagName(value));
      OnJoystickUsedChanged(InputService.Instance.JoystickUsed);
    }
  }

  public bool Selected
  {
    get => Selectable.Selected;
    set => Selectable.Selected = value;
  }

  public bool Interactable
  {
    get => Selectable.Interactable;
    set => Selectable.Interactable = value;
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
      Selectable.SetValue(null);
      string hotKeyNameByGroup = InputUtility.GetHotKeyNameByGroup(gameActionGroup, true);
      Sprite iconSprite = hotKeyNameByGroup.IsNullOrEmpty() ? (Sprite) null : ControlIconsManager.Instance.GetIconSprite(hotKeyNameByGroup);
      keyIcon.sprite = iconSprite;
      keyIcon.gameObject.SetActive(true);
      this.gameObject.SetActive((Object) iconSprite != (Object) null);
      Interactable = true;
    }
    else
    {
      Selectable.SetValue(InputUtility.GetHotKeyNameByGroup(gameActionGroup, false));
      keyIcon.sprite = (Sprite) null;
      keyIcon.gameObject.SetActive(false);
      this.gameObject.SetActive(true);
      Interactable = gameActionGroup.IsChangeble;
    }
  }
}
