using System;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class HerbRootsGroupDebug {
	private static string name = "[HerbRoots]";
	private static KeyCode key = KeyCode.H;
	private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
	private static Color headerColor = Color.green;
	private static Color trueColor = Color.white;
	private static Color falseColor = ColorPreset.LightGray;

	public static bool IsGroupVisible => GroupDebugService.IsGroupVisible(name);

	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized +=
			(Action)(() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
	}

	public static void DrawHeader() {
		var text = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
		ServiceLocator.GetService<GizmoService>().DrawText(text, headerColor);
	}

	private static void Update() { }
}