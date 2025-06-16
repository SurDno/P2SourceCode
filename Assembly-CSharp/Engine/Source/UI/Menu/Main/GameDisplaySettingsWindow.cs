using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main;

public class GameDisplaySettingsWindow :
	CancelableSimpleWindow,
	IGameDisplaySettingsWindow,
	IWindow,
	IPauseMenu {
	protected override void RegisterLayer() {
		RegisterLayer<IGameDisplaySettingsWindow>(this);
	}
}