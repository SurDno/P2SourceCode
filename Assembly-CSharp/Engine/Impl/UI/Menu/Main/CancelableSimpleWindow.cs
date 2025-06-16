using Engine.Common.Services;
using Engine.Source.Services.Inputs;

namespace Engine.Impl.UI.Menu.Main;

public abstract class CancelableSimpleWindow : SimpleWindow {
	protected override void OnEnable() {
		base.OnEnable();
		ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, CancelListener);
	}

	protected override void OnDisable() {
		ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, CancelListener);
		base.OnDisable();
	}
}