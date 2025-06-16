using Inspectors;
using UnityEngine;

namespace Engine.Source.Settings;

public class ScreenGameSettings : SettingsInstanceByRequest<ScreenGameSettings> {
	[Inspected] public int ScreenWidth;
	[Inspected] public int ScreenHeight;
	[Inspected] public FullScreenMode FullScreenMode;
}