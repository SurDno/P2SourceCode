using System.Collections.Generic;
using Scripts.Utility;

namespace UnityEngine;

public static class PhysicsUtility {
	private const int chankSize = 1024;
	private static RaycastHit[] tmp = new RaycastHit[1024];

	public static void Raycast(
		List<RaycastHit> result,
		Vector3 origin,
		Vector3 direction,
		float maxDistance) {
		result.Clear();
		int num;
		while (true) {
			num = Physics.RaycastNonAlloc(origin, direction, tmp, maxDistance);
			if (num == tmp.Length)
				tmp = new RaycastHit[tmp.Length + 1024];
			else
				break;
		}

		for (var index = 0; index < num; ++index)
			result.Add(tmp[index]);
		result.Sort(RaycastComparer.Instance);
	}

	public static void Raycast(
		List<RaycastHit> result,
		Vector3 origin,
		Vector3 direction,
		float maxDistance,
		int layerMask,
		QueryTriggerInteraction queryTriggerInteraction) {
		result.Clear();
		int num;
		while (true) {
			num = Physics.RaycastNonAlloc(origin, direction, tmp, maxDistance, layerMask, queryTriggerInteraction);
			if (num == tmp.Length)
				tmp = new RaycastHit[tmp.Length + 1024];
			else
				break;
		}

		for (var index = 0; index < num; ++index)
			result.Add(tmp[index]);
		result.Sort(RaycastComparer.Instance);
	}

	public static void Raycast(
		List<RaycastHit> result,
		Vector3 origin,
		Vector3 direction,
		float maxDistance,
		int layerMask) {
		result.Clear();
		int num;
		while (true) {
			num = Physics.RaycastNonAlloc(origin, direction, tmp, maxDistance, layerMask);
			if (num == tmp.Length)
				tmp = new RaycastHit[tmp.Length + 1024];
			else
				break;
		}

		for (var index = 0; index < num; ++index)
			result.Add(tmp[index]);
		result.Sort(RaycastComparer.Instance);
	}

	public static void Raycast(
		List<RaycastHit> result,
		Ray ray,
		float maxDistance,
		int layerMask,
		QueryTriggerInteraction queryTriggerInteraction) {
		result.Clear();
		int num;
		while (true) {
			num = Physics.RaycastNonAlloc(ray, tmp, maxDistance, layerMask, queryTriggerInteraction);
			if (num == tmp.Length)
				tmp = new RaycastHit[tmp.Length + 1024];
			else
				break;
		}

		for (var index = 0; index < num; ++index)
			result.Add(tmp[index]);
		result.Sort(RaycastComparer.Instance);
	}

	public static void SphereCast(
		List<RaycastHit> result,
		Ray ray,
		float radius,
		float maxDistance,
		int layerMask,
		QueryTriggerInteraction queryTriggerInteraction) {
		result.Clear();
		int num;
		while (true) {
			num = Physics.SphereCastNonAlloc(ray, radius, tmp, maxDistance, layerMask, queryTriggerInteraction);
			if (num == tmp.Length)
				tmp = new RaycastHit[tmp.Length + 1024];
			else
				break;
		}

		for (var index = 0; index < num; ++index)
			result.Add(tmp[index]);
		result.Sort(RaycastComparer.Instance);
	}
}