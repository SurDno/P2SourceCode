using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Gizmos;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Source.Debugs;

public static class SelectionGroupDebugUtility {
	public static void DrawPath(
		GameObject go,
		Vector3 position,
		Quaternion rotation,
		Bounds bounds) {
		var component = go.GetComponent<NavMeshAgent>();
		if (component == null)
			return;
		var path = component.path;
		var service = ServiceLocator.GetService<GizmoService>();
		var num = NavMeshUtility.DrawPath(component, service);
		var text = "NavMeshAgent status : " + component.pathStatus + "\nNavMeshAgent hasPath : " + component.hasPath +
		           "\nNavMeshAgent isOnNavMesh : " + component.isOnNavMesh;
		if (component.hasPath && component.isOnNavMesh)
			text = text + "\nNavMeshAgent remaining distance : " + component.remainingDistance + "\nCorner count : " +
			       num;
		service.DrawText3d(go.transform.position, text, TextCorner.Down, Color.white);
	}

	public static void DrawDetector(
		DetectorComponent detector,
		Vector3 position,
		Quaternion rotation,
		bool eyeVisible,
		bool hearingVisible) {
		var service = ServiceLocator.GetService<GizmoService>();
		if (eyeVisible) {
			var baseEyeDistance = detector.BaseEyeDistance;
			var eyeDistance = detector.EyeDistance;
			var eyeAngle = detector.EyeAngle;
			var yellow = Color.yellow;
			var startAngle = (float)(360.0 - rotation.eulerAngles.y + 90.0) - eyeAngle / 2f;
			service.DrawEyeSector(position, baseEyeDistance, startAngle, startAngle + eyeAngle, yellow, false);
			service.DrawEyeSector(position, eyeDistance, startAngle, startAngle + eyeAngle, yellow);
			var num = 0;
			foreach (var detectableComponent in detector.Visible)
				if (detectableComponent != null && !detectableComponent.IsDisposed) {
					var gameObject = ((IEntityView)detectableComponent.Owner).GameObject;
					if (!(gameObject == null)) {
						service.DrawLine(position, gameObject.transform.position, yellow);
						++num;
					}
				}

			var text = "Eye" + " , Angle : " + eyeAngle + " , Base Distance : " + baseEyeDistance + " , Distance : " +
			           eyeDistance + " , Count : " + num;
			service.DrawText3d(text, TextCorner.Down, yellow);
		}

		if (!hearingVisible)
			return;
		var red = Color.red;
		var baseHearingDistance = detector.BaseHearingDistance;
		var hearingDistance = detector.HearingDistance;
		service.DrawCircle(position, baseHearingDistance, red, false);
		service.DrawCircle(position, hearingDistance, red);
		var component = detector.Owner.GetComponent<LocationItemComponent>();
		var flag = component != null && component.IsIndoor;
		var num1 = 0;
		var vector3 = rotation * new Vector3(0.02f, 0.0f, 0.0f);
		foreach (var detectableComponent in detector.Hearing)
			if (detectableComponent != null && !detectableComponent.IsDisposed) {
				var gameObject = ((IEntityView)detectableComponent.Owner).GameObject;
				if (!(gameObject == null)) {
					service.DrawLine(position + vector3, gameObject.transform.position + vector3, red);
					++num1;
				}
			}

		var text1 = "Hearing" + " , Base Distance : " + baseHearingDistance + " , Distance : " + hearingDistance +
		            " , Count : " + num1;
		service.DrawText3d(text1, TextCorner.Down, red);
	}

	public static void DrawDetectable(
		DetectableComponent detectable,
		Vector3 position,
		Quaternion rotation,
		bool eyeVisible,
		bool hearingVisible) {
		if (eyeVisible) {
			var green = Color.green;
			ServiceLocator.GetService<GizmoService>()
				.DrawCircle(position, detectable.BaseVisibleDistance, green, false);
			ServiceLocator.GetService<GizmoService>().DrawCircle(position, detectable.VisibleDistance, green);
			var text = "Visible" + " , Distance : " + detectable.BaseVisibleDistance + " , Current distance : " +
			           detectable.VisibleDistance + " , Detect type : " + detectable.VisibleDetectType;
			ServiceLocator.GetService<GizmoService>().DrawText3d(text, TextCorner.Down, green);
		}

		if (!hearingVisible)
			return;
		var magenta = Color.magenta;
		ServiceLocator.GetService<GizmoService>().DrawCircle(position, detectable.NoiseDistance, magenta);
		var text1 = "Noise" + " , Distance : " + detectable.NoiseDistance + " , Detect type : " +
		            detectable.NoiseDetectType;
		ServiceLocator.GetService<GizmoService>().DrawText3d(text1, TextCorner.Down, magenta);
	}

	public static Bounds GetBounds(GameObject go) {
		var componentsInChildren = go.GetComponentsInChildren<Collider>();
		if (componentsInChildren.Length == 0)
			return new Bounds();
		var bounds = componentsInChildren[0].bounds;
		for (var index = 1; index < componentsInChildren.Length; ++index)
			bounds.Encapsulate(componentsInChildren[index].bounds);
		return bounds;
	}
}