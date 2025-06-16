using Inspectors;

namespace Engine.Source.Settings;

public class CommonSettings : SettingsInstanceByRequest<CommonSettings> {
	[Inspected] public IValue<bool> NotFirstStart = new BoolValue(nameof(NotFirstStart));
}