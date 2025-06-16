using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main;

public class StartKeySettingsWindow :
	SimpleWindow,
	IStartKeySettingsWindow,
	IWindow,
	IMainMenu,
	IPauseMenu {
	protected override void RegisterLayer() {
		RegisterLayer<IStartKeySettingsWindow>(this);
	}
}