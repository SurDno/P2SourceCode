using System;
using System.Linq;
using Cofe.Meta;
using Engine.Common.Blenders;
using Engine.Common.Services;
using Engine.Common.Weather;
using Engine.Impl.Services;
using Engine.Source.Blenders;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class WeatherGroupDebug {
	private static string name = "[Weather]";
	private static KeyCode key = KeyCode.W;
	private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
	private static Color headerColor = ColorPreset.Orange;
	private static Color bodyColor = Color.white;

	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized +=
			(Action)(() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
	}

	private static void Update() {
		var service1 = ServiceLocator.GetService<WeatherController>();
		if (InputUtility.IsKeyDown(KeyCode.KeypadDivide))
			service1.IsEnabled = !service1.IsEnabled;
		if (InputUtility.IsKeyDown(KeyCode.KeypadPeriod)) {
			var weatherSnapshot =
				ServiceLocator.GetService<ITemplateService>().GetTemplates<IWeatherSnapshot>().Random();
			if (weatherSnapshot != null && service1.IsEnabled) {
				var layerBlenderItem = service1.Blender.Items.FirstOrDefault();
				if (layerBlenderItem != null) {
					var service2 = ServiceLocator.GetService<ITimeService>();
					layerBlenderItem.Blender.BlendTo(weatherSnapshot,
						TimeSpan.FromSeconds(1.0 * service2.GameTimeFactor));
					Debug.Log(ObjectInfoUtility.GetStream().Append("Change weather to : [").Append(weatherSnapshot.Name)
						.Append("] : [").Append(weatherSnapshot.Source).Append("]"));
				}
			}
		}

		var text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		var text2 = "  Enabled : " + service1.IsEnabled;
		var num = 0;
		foreach (var layerBlenderItem in service1.Blender.Items) {
			text2 = text2 + "\n  Weather Layer " + (WeatherLayer)num + " : ";
			var blender = (WeatherSmoothBlender)layerBlenderItem.Blender;
			text2 = text2 + "[" + blender.Current.Name + "]";
			if (blender.Target != null)
				text2 = text2 + " > [" + blender.Target.Name + "] , Progress : " + blender.Progress.ToString("F2");
			text2 = text2 + " , Opacity : " + layerBlenderItem.Opacity;
			++num;
		}

		ServiceLocator.GetService<GizmoService>().DrawText(text2, bodyColor);
	}
}