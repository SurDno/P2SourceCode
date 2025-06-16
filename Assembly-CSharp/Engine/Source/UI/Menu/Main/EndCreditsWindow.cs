using System;
using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main;

public class EndCreditsWindow : CancelableSimpleWindow, IEndCreditsWindow, IWindow {
	protected override void RegisterLayer() {
		RegisterLayer<IEndCreditsWindow>(this);
	}

	public override Type GetWindowType() {
		return typeof(IEndCreditsWindow);
	}
}