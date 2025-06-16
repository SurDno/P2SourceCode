using System;
using System.Linq;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Interactable;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class InteractableGroupDebug {
	private static string name = "[Interactable]";
	private static KeyCode key = KeyCode.O;
	private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
	private static Color headerColor = Color.cyan;
	private static Color trueColor = Color.white;
	private static Color falseColor = ColorPreset.LightGray;

	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized +=
			(Action)(() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
	}

	private static void Update() {
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			return;
		var component = player.GetComponent<PlayerInteractableComponent>();
		if (component == null)
			return;
		var text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		if (component.Interactable == null) {
			var text2 = "  Interactable not found";
			ServiceLocator.GetService<GizmoService>().DrawText(text2, falseColor);
		} else {
			var text3 = "  Interactable found, count : " + component.Interactable.Items.Count();
			ServiceLocator.GetService<GizmoService>().DrawText(text3, falseColor);
			foreach (var interactItem in component.Interactable.Items) {
				var item = interactItem;
				var interactItemInfo = component.ValidateItems.FirstOrDefault(o => o.Item.Type == item.Type);
				var text4 = "  " + item.Type + " , validate : " + (interactItemInfo != null);
				if (interactItemInfo != null && interactItemInfo.Invalid)
					text4 += " , debug";
				ServiceLocator.GetService<GizmoService>()
					.DrawText(text4, interactItemInfo != null ? trueColor : falseColor);
			}
		}
	}
}