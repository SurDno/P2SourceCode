using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider), typeof(LODGroup))]
[ExecuteInEditMode]
public class PlagueZone : MonoBehaviour {
	private static List<ZoneHit> tmp = new();
	private static List<RaycastHit> hits = new();
	[SerializeField] private float _level;
	[SerializeField] private byte importance;

	private static void GetSortedZoneHits(Vector2 worldPosition, List<ZoneHit> result) {
		result.Clear();
		PhysicsUtility.Raycast(hits, new Vector3(worldPosition.x, 1000f, worldPosition.y), Vector3.down, 2000f);
		if (hits.Count <= 0)
			return;
		for (var index = 0; index < hits.Count; ++index) {
			var componentNonAlloc = hits[index].collider.GetComponentNonAlloc<PlagueZone>();
			if (componentNonAlloc != null)
				result.Add(new ZoneHit(componentNonAlloc, hits[index].textureCoord.x, componentNonAlloc.importance,
					componentNonAlloc._level));
		}

		result.Sort(ZoneHit.Comparison);
	}

	public static float GetLevel(Vector2 worldPosition) {
		var level = 0.0f;
		var num = 1f;
		GetSortedZoneHits(worldPosition, tmp);
		for (var index = 0; index < tmp.Count; ++index) {
			var zoneHit = tmp[index];
			level += zoneHit.Level * zoneHit.Opacity * num;
			num *= 1f - zoneHit.Opacity;
			if (num == 0.0)
				break;
		}

		tmp.Clear();
		return level;
	}

	public void ApplyLevel() {
		var loDs = GetComponent<LODGroup>().GetLODs();
		if (_level == 0.0)
			for (var index1 = 0; index1 < loDs.Length; ++index1) {
				for (var index2 = 0; index2 < loDs[index1].renderers.Length; ++index2)
					loDs[index1].renderers[index2].enabled = false;
			}
		else {
			var properties = new MaterialPropertyBlock();
			properties.SetFloat("_Level", _level);
			for (var index3 = 0; index3 < loDs.Length; ++index3) {
				for (var index4 = 0; index4 < loDs[index3].renderers.Length; ++index4) {
					var renderer = loDs[index3].renderers[index4];
					renderer.SetPropertyBlock(properties);
					renderer.enabled = true;
				}
			}
		}
	}

	private void OnEnable() {
		ApplyLevel();
	}
}