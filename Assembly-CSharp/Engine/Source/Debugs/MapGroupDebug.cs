using System;
using System.Collections.Generic;
using System.Linq;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl;
using Engine.Impl.Services;
using Engine.Impl.Services.HierarchyServices;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Maps;
using Engine.Source.Components.Regions;
using Engine.Source.Components.Utilities;
using Engine.Source.Services;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class MapGroupDebug {
	private static string name = "[Map]";
	private static KeyCode key = KeyCode.M;
	private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
	private static Color headerColor = Color.cyan;
	private static Color bodyColor = Color.white;
	private static Color trueColor = Color.white;
	private static Color falseColor = ColorPreset.LightGray;
	private static KeyCode teleportKey = KeyCode.Mouse0;
	private static KeyModifficator teleportModifficator = KeyModifficator.Control;
	private static KeyCode mapItemsKey = KeyCode.F1;
	private static BoolPlayerProperty mapItemsVisible;

	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized +=
			(Action)(() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
	}

	private static void Update() {
		if (InputUtility.IsKeyDown(mapItemsKey, KeyModifficator.Control))
			mapItemsVisible.Value = !mapItemsVisible;
		var service1 = ServiceLocator.GetService<GizmoService>();
		var text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
		service1.DrawText(text1, headerColor);
		var text2 = "  Teleport : (" + InputUtility.GetHotKeyText(teleportKey, teleportModifficator) + ")";
		service1.DrawText(text2, bodyColor);
		var str = "\n  ";
		var service2 = ServiceLocator.GetService<MapService>();
		if (service2 == null)
			return;
		var current = service2.Current;
		var text3 = current == null ? str + "Selected is null" : str + current.GetInfo();
		service1.DrawText(text3, bodyColor);
		if (InputUtility.IsKeyDown(teleportKey, teleportModifficator))
			ComputeTeleport();
		DrawMapItems();
		DrawHelp();
	}

	private static void DrawHelp() {
		var text1 = "\n[Gizmos]";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		var text2 = "  MapItems " + (mapItemsVisible ? "True" : (object)"False") + " [Control + " + mapItemsKey + "]";
		ServiceLocator.GetService<GizmoService>().DrawText(text2, mapItemsVisible ? trueColor : falseColor);
	}

	private static void DrawMapItems() {
		if (!mapItemsVisible)
			return;
		var active = ServiceLocator.GetService<UIService>().Active as MapWindow;
		if (active == null)
			return;
		var service1 = ServiceLocator.GetService<GizmoService>();
		var service2 = ServiceLocator.GetService<ITemplateService>();
		var distance = 2f / (24f / Screen.height);
		var num1 = 0.5f;
		var detectorPosition = new Vector3(Input.mousePosition.x, 0.0f, Screen.height - Input.mousePosition.y);
		var num2 = 0.7f;
		foreach (var mapItemComponent in MapItemComponent.Items) {
			var position = ((IEntityView)mapItemComponent.Owner).Position;
			if (!(position == Vector3.zero)) {
				var screenPosition = active.GetScreenPosition(position);
				if (mapItemComponent.Region != null) {
					if (active.Scroll < (double)num2) {
						var region = mapItemComponent.Region.Region;
						if (region != RegionEnum.Steppe) {
							var text = region.ToString();
							service1.DrawText(screenPosition, text, TextCorner.None, Color.yellow);
						}
					}
				} else if (active.Scroll >= (double)num2) {
					var text = mapItemComponent.Owner.Name;
					if (text.EndsWith("_Loader")) {
						text = text.Substring(0, text.Length - "_Loader".Length);
						var num3 = text.IndexOf("_");
						if (num3 != -1)
							text = text.Substring(num3 + 1);
						var childs1 = mapItemComponent.Owner.Childs;
						var entity1 = childs1 != null ? childs1.FirstOrDefault(o => o.Name == "Indoor") : null;
						if (entity1 != null) {
							var childs2 = entity1.Childs;
							var entity2 = childs2 != null ? childs2.FirstOrDefault(o => o.Name == "Common") : null;
							if (entity2 != null) {
								var childs3 = entity2.Childs;
								var entity3 = childs3 != null
									? childs3.FirstOrDefault(o => o.Name.StartsWith("Furniture") && o.IsEnabled)
									: null;
								if (entity3 != null) {
									var hierarchyItem = ((IEntityHierarchy)entity3).HierarchyItem;
									if (hierarchyItem != null && hierarchyItem.Container != null) {
										var container = hierarchyItem.Container;
										if (container != null) {
											var template = service2.GetTemplate<SceneObject>(container.Id);
											if (template != null)
												text = text + "\n" + template.Name;
										}
									}
								}
							}
						}
					} else if (text.EndsWith("_MapItem"))
						text = text.Substring(0, text.Length - "_MapItem".Length);

					var green = Color.green;
					if (!DetectorUtility.CheckRadiusDistance(detectorPosition,
						    new Vector3(screenPosition.x, 0.0f, screenPosition.y), distance))
						green.a = num1;
					service1.DrawText(screenPosition, text, TextCorner.None, green);
				}
			}
		}
	}

	private static void ComputeTeleport() {
		var active = ServiceLocator.GetService<UIService>().Active as MapWindow;
		if (active == null)
			return;
		var point = active.GetWorldPosition(Input.mousePosition);
		var player = ServiceLocator.GetService<ISimulation>().Player;
		var component = RegionUtility
			.GetRegionByName(ScriptableObjectInstance<GameSettingsData>.Instance.DefaultRegionName)
			.GetComponent<LocationComponent>();
		point = new Vector3(point.x, 0.0f, point.y);
		point.y = Terrain.activeTerrain.SampleHeight(point);
		NavMeshUtility.SamplePosition(ref point, AreaEnum.All.ToMask());
		Debug.Log(ObjectInfoUtility.GetStream().Append("Teleport to : ").Append(point));
		player.GetComponent<NavigationComponent>().TeleportTo(component, point, Quaternion.identity);
		ServiceLocator.GetService<UIService>().Pop();
	}
}