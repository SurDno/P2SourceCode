using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RootMotion;

public class Hierarchy {
	public static bool HierarchyIsValid(Transform[] bones) {
		for (var index = 1; index < bones.Length; ++index)
			if (!IsAncestor(bones[index], bones[index - 1]))
				return false;
		return true;
	}

	public static Object ContainsDuplicate(Object[] objects) {
		for (var index1 = 0; index1 < objects.Length; ++index1) {
			for (var index2 = 0; index2 < objects.Length; ++index2)
				if (index1 != index2 && objects[index1] == objects[index2])
					return objects[index1];
		}

		return null;
	}

	public static bool IsAncestor(Transform transform, Transform ancestor) {
		if (transform == null || ancestor == null)
			return true;
		if (transform.parent == null)
			return false;
		return transform.parent == ancestor || IsAncestor(transform.parent, ancestor);
	}

	public static bool ContainsChild(Transform transform, Transform child) {
		if (transform == child)
			return true;
		foreach (Object componentsInChild in transform.GetComponentsInChildren<Transform>())
			if (componentsInChild == child)
				return true;
		return false;
	}

	public static void AddAncestors(Transform transform, Transform blocker, ref Transform[] array) {
		if (!(transform.parent != null) || !(transform.parent != blocker))
			return;
		if (transform.parent.position != transform.position && transform.parent.position != blocker.position) {
			Array.Resize(ref array, array.Length + 1);
			array[array.Length - 1] = transform.parent;
		}

		AddAncestors(transform.parent, blocker, ref array);
	}

	public static Transform GetAncestor(Transform transform, int minChildCount) {
		if (transform == null || !(transform.parent != null))
			return null;
		return transform.parent.childCount >= minChildCount
			? transform.parent
			: GetAncestor(transform.parent, minChildCount);
	}

	public static Transform GetFirstCommonAncestor(Transform t1, Transform t2) {
		if (t1 == null || t2 == null || t1.parent == null || t2.parent == null)
			return null;
		return IsAncestor(t2, t1.parent) ? t1.parent : GetFirstCommonAncestor(t1.parent, t2);
	}

	public static Transform GetFirstCommonAncestor(Transform[] transforms) {
		if (transforms == null) {
			Debug.LogWarning("Transforms is null.");
			return null;
		}

		if (transforms.Length == 0) {
			Debug.LogWarning("Transforms.Length is 0.");
			return null;
		}

		for (var index = 0; index < transforms.Length; ++index) {
			if (transforms[index] == null)
				return null;
			if (IsCommonAncestor(transforms[index], transforms))
				return transforms[index];
		}

		return GetFirstCommonAncestorRecursive(transforms[0], transforms);
	}

	public static Transform GetFirstCommonAncestorRecursive(
		Transform transform,
		Transform[] transforms) {
		if (transform == null) {
			Debug.LogWarning("Transform is null.");
			return null;
		}

		if (transforms == null) {
			Debug.LogWarning("Transforms is null.");
			return null;
		}

		if (transforms.Length == 0) {
			Debug.LogWarning("Transforms.Length is 0.");
			return null;
		}

		if (IsCommonAncestor(transform, transforms))
			return transform;
		return transform.parent == null ? null : GetFirstCommonAncestorRecursive(transform.parent, transforms);
	}

	public static bool IsCommonAncestor(Transform transform, Transform[] transforms) {
		if (transform == null) {
			Debug.LogWarning("Transform is null.");
			return false;
		}

		for (var index = 0; index < transforms.Length; ++index) {
			if (transforms[index] == null) {
				Debug.Log("Transforms[" + index + "] is null.");
				return false;
			}

			if (!IsAncestor(transforms[index], transform) && transforms[index] != transform)
				return false;
		}

		return true;
	}
}