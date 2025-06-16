using Engine.Common.Components;
using Engine.Source.Settings.External;

public static class PlatformUtility {
	public static string GetPath(string fileName) {
		return fileName;
	}

	public static int StrategyIndex => !ScriptableObjectInstance<BuildData>.Instance.Release
		? ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DevelopmentStrategyIndex
		: ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReleaseStrategyIndex;

	public static bool IsChangeLocationLoadingWindow(IRegionComponent region) {
		return ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ChangeLocationLoadingWindow;
	}
}