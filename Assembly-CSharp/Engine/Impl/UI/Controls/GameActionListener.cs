using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class GameActionListener : MonoBehaviour {
	[SerializeField] private EventView view;
	[SerializeField] private GameActionType action;

	private void OnDisable() {
		ServiceLocator.GetService<GameActionService>().RemoveListener(action, OnGameAction);
	}

	private void OnEnable() {
		ServiceLocator.GetService<GameActionService>().AddListener(action, OnGameAction);
	}

	private bool OnGameAction(GameActionType type, bool down) {
		if (!down || view == null)
			return false;
		view.Invoke();
		return true;
	}
}