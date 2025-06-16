using System.Collections.Generic;
using UnityEngine;

namespace Pingle;

public static class UISelectableHelper {
	public static GameObject Select(
		IEnumerable<GameObject> objects,
		GameObject origin,
		Vector3 dirrection,
		bool IsDirectionPriority = true) {
		var transform = origin.transform as RectTransform;
		return Select(objects,
			transform != null
				? transform.TransformPoint(GetPointOnRectEdge(transform, dirrection))
				: origin.transform.position, dirrection, IsDirectionPriority);
	}

	public static GameObject Select(
		IEnumerable<GameObject> objects,
		Vector3 origin,
		Vector3 dirrection,
		bool isDirectionPriority = true) {
		var num1 = float.NegativeInfinity;
		GameObject gameObject1 = null;
		foreach (var gameObject2 in objects)
			if (!(gameObject2 == null) && gameObject2.activeInHierarchy) {
				var transform = gameObject2.transform as RectTransform;
				Vector3 position = transform != null ? transform.rect.center : Vector3.zero;
				var vector3 = gameObject2.transform.TransformPoint(position) - origin;
				var f = Vector3.Dot(dirrection, vector3.normalized);
				if (f > 0.0) {
					if (isDirectionPriority)
						f = Mathf.Pow(f, 2f);
					var num2 = f / vector3.magnitude;
					if (num2 > (double)num1) {
						num1 = num2;
						gameObject1 = gameObject2;
					}
				}
			}

		return gameObject1;
	}

	public static GameObject SelectClosest(IEnumerable<GameObject> objects, Vector3 origin) {
		var num = float.MaxValue;
		GameObject gameObject1 = null;
		foreach (var gameObject2 in objects) {
			var sqrMagnitude = ((Vector2)(gameObject2.transform.position - origin)).sqrMagnitude;
			if (sqrMagnitude < (double)num) {
				gameObject1 = gameObject2;
				num = sqrMagnitude;
			}
		}

		return gameObject1;
	}

	private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir) {
		if (rect == null)
			return Vector3.zero;
		if (dir != Vector2.zero)
			dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
		dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
		return dir;
	}
}