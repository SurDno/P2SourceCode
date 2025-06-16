using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Engine.Impl.UI.Controls;

public class SelectWithGameActions : MonoBehaviour {
	[SerializeField] private GameObject[] selectables;
	[SerializeField] private GameActionType[] decreaseActions;
	[SerializeField] private GameActionType[] increaseActions;
	[SerializeField] private int defaultSelected = -1;
	[SerializeField] private bool wrap;
	private GameActionHandle onDecreaseAction;
	private GameActionHandle onIncreaseAction;

	private int Current() {
		var selectedGameObject = EventSystem.current.currentSelectedGameObject;
		if (selectedGameObject == null)
			return -1;
		for (var index = 0; index < selectables.Length; ++index)
			if (selectedGameObject == selectables[index])
				return index;
		return -1;
	}

	private bool OnDecrease(GameActionType type, bool down) {
		if (!down)
			return false;
		var index = Current();
		switch (index) {
			case -1:
				index = 0;
				break;
			case 0:
				if (wrap) index = selectables.Length - 1;
				break;
			default:
				--index;
				break;
		}

		Select(index);
		return true;
	}

	private bool OnIncrease(GameActionType type, bool down) {
		if (!down)
			return false;
		var index = Current();
		var num = selectables.Length - 1;
		if (index == -1)
			index = num;
		else if (index == num) {
			if (wrap)
				index = 0;
		} else
			++index;

		Select(index);
		return true;
	}

	private void OnDisable() {
		if (Current() != -1)
			EventSystem.current.SetSelectedGameObject(null);
		var service = ServiceLocator.GetService<GameActionService>();
		for (var index = 0; index < decreaseActions.Length; ++index)
			service.RemoveListener(decreaseActions[index], onDecreaseAction);
		for (var index = 0; index < increaseActions.Length; ++index)
			service.RemoveListener(increaseActions[index], onIncreaseAction);
	}

	private void OnEnable() {
		if (onDecreaseAction == null)
			onDecreaseAction = OnDecrease;
		if (onIncreaseAction == null)
			onIncreaseAction = OnIncrease;
		var service = ServiceLocator.GetService<GameActionService>();
		for (var index = 0; index < decreaseActions.Length; ++index)
			service.AddListener(decreaseActions[index], onDecreaseAction);
		for (var index = 0; index < increaseActions.Length; ++index)
			service.AddListener(increaseActions[index], onIncreaseAction);
		if (defaultSelected < 0 || defaultSelected >= selectables.Length)
			return;
		Select(defaultSelected);
	}

	private void Select(int index) {
		EventSystem.current.SetSelectedGameObject(selectables[index]);
	}
}