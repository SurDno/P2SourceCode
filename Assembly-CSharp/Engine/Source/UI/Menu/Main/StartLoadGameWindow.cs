using System;
using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main;

public class StartLoadGameWindow :
	CancelableSimpleWindow,
	IStartLoadGameWindow,
	IWindow,
	IMainMenu,
	IPauseMenu {
	protected override void RegisterLayer() {
		RegisterLayer<IStartLoadGameWindow>(this);
	}

	public override Type GetWindowType() {
		return typeof(IStartLoadGameWindow);
	}
}