using Cofe.Utility;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SelectableSettingsItemView))]
public class KeySettingsItemView : MonoBehaviour, ISettingEntity, ISelectable {
	[SerializeField] private SelectableSettingsItemView selectable;
	[SerializeField] private Image keyIcon;
	private ActionGroup gameActionGroup;

	public SelectableSettingsItemView Selectable => selectable;

	public ActionGroup GameActionGroup {
		get => gameActionGroup;
		set {
			gameActionGroup = value;
			Selectable.SetName(InputUtility.GetTagName(value));
			OnJoystickUsedChanged(InputService.Instance.JoystickUsed);
		}
	}

	public bool Selected {
		get => Selectable.Selected;
		set => Selectable.Selected = value;
	}

	public bool Interactable {
		get => Selectable.Interactable;
		set => Selectable.Interactable = value;
	}

	public void DecrementValue() { }

	public void IncrementValue() { }

	public bool IsActive() {
		return gameObject.activeInHierarchy;
	}

	public void OnSelect() { }

	public void OnDeSelect() { }

	public void OnJoystickUsedChanged(bool value) {
		if (value) {
			Selectable.SetValue(null);
			var hotKeyNameByGroup = InputUtility.GetHotKeyNameByGroup(gameActionGroup, true);
			var iconSprite = hotKeyNameByGroup.IsNullOrEmpty()
				? null
				: ControlIconsManager.Instance.GetIconSprite(hotKeyNameByGroup);
			keyIcon.sprite = iconSprite;
			keyIcon.gameObject.SetActive(true);
			gameObject.SetActive(iconSprite != null);
			Interactable = true;
		} else {
			Selectable.SetValue(InputUtility.GetHotKeyNameByGroup(gameActionGroup, false));
			keyIcon.sprite = null;
			keyIcon.gameObject.SetActive(false);
			gameObject.SetActive(true);
			Interactable = gameActionGroup.IsChangeble;
		}
	}
}