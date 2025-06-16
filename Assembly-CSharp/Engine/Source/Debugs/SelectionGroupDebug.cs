using System;
using Cofe.Meta;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Services;
using Engine.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs;

[Initialisable]
public static class SelectionGroupDebug {
	private static string name = "[Selection]";
	private static KeyCode key = KeyCode.E;
	private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
	private static KeyCode eyeKey = KeyCode.F1;
	private static KeyCode hearingKey = KeyCode.F2;
	private static KeyCode movableKey = KeyCode.F3;
	private static Color headerColor = Color.green;
	private static Color trueColor = Color.white;
	private static Color falseColor = ColorPreset.LightGray;
	private static BoolPlayerProperty eyeVisible;
	private static BoolPlayerProperty hearingVisible;
	private static BoolPlayerProperty movableVisible;

	[Initialise]
	private static void Initialise() {
		InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action)(() => {
			GroupDebugService.RegisterGroup(name, key, modifficators, Update);
			eyeVisible = BoolPlayerProperty.Create(() => eyeVisible);
			hearingVisible = BoolPlayerProperty.Create(() => hearingVisible);
			movableVisible = BoolPlayerProperty.Create(() => movableVisible);
		});
	}

	private static void Update() {
		if (InputUtility.IsKeyDown(eyeKey, KeyModifficator.Control))
			eyeVisible.Value = !eyeVisible;
		if (InputUtility.IsKeyDown(hearingKey, KeyModifficator.Control))
			hearingVisible.Value = !hearingVisible;
		if (InputUtility.IsKeyDown(movableKey, KeyModifficator.Control))
			movableVisible.Value = !movableVisible;
		var characterPosition = Vector3.zero;
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player != null) {
			var gameObject = ((IEntityView)player).GameObject;
			if (gameObject != null) {
				characterPosition = gameObject.transform.position;
				var service = ServiceLocator.GetService<GizmoService>();
				var component1 = player.GetComponent<DetectableComponent>();
				if (component1 != null) {
					var position = gameObject.transform.position +
					               gameObject.transform.rotation * new Vector3(0.0f, 0.0f, 2f);
					service.DrawText3d(position, "", TextCorner.None, Color.clear);
					SelectionGroupDebugUtility.DrawDetectable(component1, gameObject.transform.position,
						gameObject.transform.rotation, eyeVisible, hearingVisible);
				}

				var component2 = player.GetComponent<PlayerControllerComponent>();
				if (component2 != null)
					foreach (var near in component2.Nears)
						if (!near.IsDisposed) {
							var position = ((IEntityView)near.Owner).Position;
							service.DrawLine(position, position + Vector3.up, Color.cyan);
						}
			}
		}

		ComputeHotkeys();
		DrawInfo();
		DrawSelected(characterPosition);
		DrawHelp();
	}

	private static void DrawInfo() {
		var text1 = "\n" + name + " (" + InputUtility.GetHotKeyText(key, modifficators) + ")";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		var service = ServiceLocator.GetService<PickingService>();
		if (service == null)
			return;
		if (service.TargetGameObject == null) {
			var text2 = "  Object not found";
			ServiceLocator.GetService<GizmoService>().DrawText(text2, falseColor);
		} else {
			var vector3 = new Vector3(0.01f, 0.01f, 0.01f);
			var bounds = SelectionGroupDebugUtility.GetBounds(service.TargetGameObject);
			var color1 = service.TargetEntity != null ? new Color(1f, 0.1f, 0.0f, 1f) : Color.yellow;
			ServiceLocator.GetService<GizmoService>().DrawBox(bounds.min - vector3, bounds.max + vector3, color1);
			var color2 = service.TargetEntity != null ? ColorPreset.Orange : Color.yellow;
			var str = "  Distance : " + service.TargetGameObjectDistance;
			var targetEntity = service.TargetEntity;
			if (targetEntity != null)
				str = str + "\n  Entity : " + targetEntity.GetInfo() + "\n  Context : " +
				      (targetEntity.Context ?? "null");
			var text3 = str + "\n  GameObject : " + service.TargetGameObject.GetFullName();
			ServiceLocator.GetService<GizmoService>().DrawText(text3, color2);
		}
	}

	private static void ComputeHotkeys() {
		var service = ServiceLocator.GetService<PickingService>();
		if (service == null)
			return;
		var index = -1;
		if (InputUtility.IsKeyDown(KeyCode.Alpha0, KeyModifficator.Shift))
			index = 0;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha1, KeyModifficator.Shift))
			index = 1;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha2, KeyModifficator.Shift))
			index = 2;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha3, KeyModifficator.Shift))
			index = 3;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha4, KeyModifficator.Shift))
			index = 4;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha5, KeyModifficator.Shift))
			index = 5;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha6, KeyModifficator.Shift))
			index = 6;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha7, KeyModifficator.Shift))
			index = 7;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha8, KeyModifficator.Shift))
			index = 8;
		else if (InputUtility.IsKeyDown(KeyCode.Alpha9, KeyModifficator.Shift))
			index = 9;
		if (index == -1)
			return;
		ServiceLocator.GetService<SelectionService>().SetSelection(index, null);
		var targetEntity = service.TargetEntity;
		if (targetEntity != null) {
			ServiceLocator.GetService<SelectionService>().SetSelection(index, targetEntity);
			Debug.Log(ObjectInfoUtility.GetStream().Append("Store object : ").GetInfo(targetEntity)
				.Append(" , index : ").Append(index).Append(" , type : ")
				.Append(TypeUtility.GetTypeName(targetEntity.GetType())));
		} else {
			var targetGameObject = service.TargetGameObject;
			if (targetGameObject != null) {
				ServiceLocator.GetService<SelectionService>().SetSelection(index, targetGameObject);
				Debug.Log(ObjectInfoUtility.GetStream().Append("Store object : ").GetFullName(targetGameObject)
					.Append(" , index : ").Append(index).Append(" , type : ")
					.Append(TypeUtility.GetTypeName(targetGameObject.GetType())));
			}
		}
	}

	private static void DrawSelected(Vector3 characterPosition) {
		var text1 = "\n[Slots]";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		var service = ServiceLocator.GetService<SelectionService>();
		if (service == null)
			return;
		var selectionCount = service.SelectionCount;
		for (var index = 0; index < selectionCount; ++index) {
			var selection = service.GetSelection(index);
			var engineObject = selection as IObject;
			var go = selection as GameObject;
			var flag = false;
			var str = "  Slot : " + index + " , ";
			string text2;
			if (engineObject != null)
				text2 = str + "Name : " + engineObject.Name + " , Type : " + engineObject.GetType().Name;
			else if (go != null)
				text2 = str + "Name : " + go.name + " , Type : " + typeof(GameObject).Name;
			else {
				text2 = str + "Empty";
				flag = true;
			}

			DrawEntityGizmo(engineObject, go, index, characterPosition);
			ServiceLocator.GetService<GizmoService>().DrawText(text2, flag ? falseColor : trueColor);
		}
	}

	private static void DrawHelp() {
		var text1 = "\n[Gizmos]";
		ServiceLocator.GetService<GizmoService>().DrawText(text1, headerColor);
		var text2 = "  Eye " + (eyeVisible ? "True" : (object)"False") + " [Control + " + eyeKey + "]";
		ServiceLocator.GetService<GizmoService>().DrawText(text2, eyeVisible ? trueColor : falseColor);
		var text3 = "  Hearing " + (hearingVisible ? "True" : (object)"False") + " [Control + " + hearingKey + "]";
		ServiceLocator.GetService<GizmoService>().DrawText(text3, hearingVisible ? trueColor : falseColor);
		var text4 = "  Movable " + (movableVisible ? "True" : (object)"False") + " [Control + " + movableKey + "]";
		ServiceLocator.GetService<GizmoService>().DrawText(text4, movableVisible ? trueColor : falseColor);
	}

	private static void DrawEntityGizmo(
		IObject engineObject,
		GameObject go,
		int index,
		Vector3 characterPosition) {
		var entity = engineObject as IEntity;
		string name;
		if (go == null) {
			if (entity == null || entity.IsDisposed)
				return;
			go = ((IEntityView)entity).GameObject;
			if (go == null)
				return;
			name = " , Name : " + entity.Name + "\nId : " + entity.Id + "\nContext : " + (entity.Context ?? "null");
		} else
			name = " , Name : " + go.name;

		var bounds = SelectionGroupDebugUtility.GetBounds(go);
		DrawEntity(index, name, bounds, go.transform.position, characterPosition);
		if (entity == null)
			return;
		if (movableVisible)
			SelectionGroupDebugUtility.DrawPath(go, go.transform.position, go.transform.rotation, bounds);
		var position = ((bounds.max - bounds.min) / 2f + bounds.min) with {
			y = bounds.min.y
		};
		ServiceLocator.GetService<GizmoService>().DrawText3d(position, "", TextCorner.None, Color.clear);
		if (eyeVisible) {
			var component1 = entity.GetComponent<DetectorComponent>();
			if (component1 != null)
				SelectionGroupDebugUtility.DrawDetector(component1, go.transform.position, go.transform.rotation,
					eyeVisible, false);
			var component2 = entity.GetComponent<DetectableComponent>();
			if (component2 != null)
				SelectionGroupDebugUtility.DrawDetectable(component2, go.transform.position, go.transform.rotation,
					eyeVisible, false);
		}

		if (!hearingVisible)
			return;
		var component3 = entity.GetComponent<DetectorComponent>();
		if (component3 != null)
			SelectionGroupDebugUtility.DrawDetector(component3, go.transform.position, go.transform.rotation, false,
				hearingVisible);
		var component4 = entity.GetComponent<DetectableComponent>();
		if (component4 != null)
			SelectionGroupDebugUtility.DrawDetectable(component4, go.transform.position, go.transform.rotation, false,
				hearingVisible);
	}

	private static void DrawEntity(
		int index,
		string name,
		Bounds bounds,
		Vector3 position,
		Vector3 characterPosition) {
		ServiceLocator.GetService<GizmoService>().DrawBox(bounds.min, bounds.max, Color.green);
		var text = "Slot : " + index + name + "\nPosition : " + position + " , Distance : " +
		           (characterPosition - position).magnitude.ToString("F2");
		var position1 = ((bounds.max - bounds.min) / 2f + bounds.min) with {
			y = bounds.max.y
		};
		ServiceLocator.GetService<GizmoService>().DrawText3d(position1, text, TextCorner.Up, Color.green);
	}
}