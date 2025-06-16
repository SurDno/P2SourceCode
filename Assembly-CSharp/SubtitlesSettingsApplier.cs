using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Settings;

public class SubtitlesSettingsApplier : EngineDependent {
	protected override void OnConnectToEngine() {
		Apply();
		InstanceByRequest<SubtitlesGameSettings>.Instance.OnApply += Apply;
	}

	protected override void OnDisconnectFromEngine() {
		InstanceByRequest<SubtitlesGameSettings>.Instance.OnApply -= Apply;
	}

	private void Apply() {
		ServiceLocator.GetService<SubtitlesService>().SubtitlesEnabled =
			InstanceByRequest<SubtitlesGameSettings>.Instance.SubtitlesEnabled.Value;
		ServiceLocator.GetService<SubtitlesService>().DialogSubtitlesEnabled =
			InstanceByRequest<SubtitlesGameSettings>.Instance.DialogSubtitlesEnabled.Value;
	}
}