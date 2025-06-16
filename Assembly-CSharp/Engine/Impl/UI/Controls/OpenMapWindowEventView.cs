using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.UI;

namespace Engine.Impl.UI.Controls;

public class OpenMapWindowEventView : OpenWindowEventView<IMapWindow> {
	protected override bool PrepareWindow() {
		var service = ServiceLocator.GetService<InterfaceBlockingService>();
		return service == null || !service.BlockMapInterface;
	}
}