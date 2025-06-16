using SRDebugger.Services;
using SRF.Service;

public static class SRDebug {
	public static IDebugService Instance => SRServiceManager.GetService<IDebugService>();

	public static void Init() {
		SRServiceManager.GetService<IDebugService>();
	}
}