using SRDebugger.Services;
using SRF.Service;

namespace SRDebugger.Internal;

public static class Service {
	private static IDebugPanelService _debugPanelService;
	private static IPinnedUIService _pinnedUiService;

	public static IDebugPanelService Panel {
		get {
			if (_debugPanelService == null)
				_debugPanelService = SRServiceManager.GetService<IDebugPanelService>();
			return _debugPanelService;
		}
	}

	public static IPinnedUIService PinnedUI {
		get {
			if (_pinnedUiService == null)
				_pinnedUiService = SRServiceManager.GetService<IPinnedUIService>();
			return _pinnedUiService;
		}
	}
}