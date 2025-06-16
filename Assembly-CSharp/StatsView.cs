using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using UnityEngine;

public class StatsView : MonoBehaviour {
	[SerializeField] private EntityView playerEntityView;
	[SerializeField] private HideableView fullVersionView;

	private void Start() {
		if (fullVersionView != null)
			fullVersionView.SkipAnimation();
		if (!(playerEntityView != null))
			return;
		playerEntityView.Value = ServiceLocator.GetService<ISimulation>()?.Player;
		playerEntityView.SkipAnimation();
	}

	private void Update() {
		if (!(playerEntityView != null))
			return;
		var player = ServiceLocator.GetService<ISimulation>()?.Player;
		if (playerEntityView.Value != player)
			playerEntityView.Value = player;
	}

	public void SetFullVersion(bool value) {
		if (!(fullVersionView != null))
			return;
		fullVersionView.Visible = value;
	}
}