using Engine.Common.Services;
using Engine.Impl.Services;

namespace Engine.Impl.UI.Controls;

public class CloseActiveWindowEventView : EventView {
	public override void Invoke() {
		ServiceLocator.GetService<UIService>().Pop();
	}
}