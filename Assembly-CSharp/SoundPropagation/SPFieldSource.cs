using System.Collections.Generic;
using UnityEngine;

namespace SoundPropagation;

[RequireComponent(typeof(AudioSource))]
public class SPFieldSource : MonoBehaviour {
	private static Dictionary<SPFieldSource, SPFieldSource> sources;
	private List<SPFieldPoint> points;
	private AudioSource audioSource;

	private void LateUpdate() {
		UpdatePosition();
	}

	private void UpdatePosition() {
		if (SPAudioListener.Instance == null)
			audioSource.mute = true;
		else {
			var position1 = SPAudioListener.Instance.Position;
			var maxDistance = audioSource.maxDistance;
			var num1 = maxDistance * maxDistance;
			var zero = Vector3.zero;
			var num2 = 0.0f;
			var num3 = maxDistance;
			for (var index = 0; index < points.Count; ++index) {
				var position2 = points[index].Position;
				if (position1.x + (double)maxDistance > position2.x &&
				    position1.x - (double)maxDistance < position2.x &&
				    position1.z + (double)maxDistance > position2.z &&
				    position1.z - (double)maxDistance < position2.z) {
					var vector3 = position2 - position1;
					var sqrMagnitude = vector3.sqrMagnitude;
					if (sqrMagnitude < (double)num1) {
						var num4 = Mathf.Sqrt(sqrMagnitude);
						var num5 = (float)(1.0 - num4 / (double)maxDistance);
						var num6 = num5 * num5;
						zero += vector3 / num4 * num6;
						num2 += num6;
						if (num4 < (double)num3)
							num3 = num4;
					}
				}
			}

			if (num2 > 0.0) {
				var magnitude = zero.magnitude;
				if (magnitude == 0.0) {
					audioSource.spread = 360f;
					transform.position = new Vector3(position1.x, position1.y + num3, position1.z);
				} else {
					audioSource.spread = (float)((1.0 - magnitude / (double)num2) * 360.0);
					transform.position = position1 + zero * (num3 / magnitude);
				}

				audioSource.mute = false;
			} else
				audioSource.mute = true;
		}
	}

	public static void AddPoint(SPFieldSource prefab, SPFieldPoint point) {
		SPFieldSource spFieldSource = null;
		if (sources == null)
			sources = new Dictionary<SPFieldSource, SPFieldSource>();
		else
			sources.TryGetValue(prefab, out spFieldSource);
		if (spFieldSource == null) {
			var group = UnityFactory.GetOrCreateGroup("[Sounds]");
			spFieldSource = Instantiate(prefab, group.transform, false);
			spFieldSource.name = prefab.name;
			spFieldSource.audioSource = spFieldSource.GetComponent<AudioSource>();
			spFieldSource.audioSource.dopplerLevel = 0.0f;
			spFieldSource.points = new List<SPFieldPoint> {
				point
			};
			spFieldSource.UpdatePosition();
			sources.Add(prefab, spFieldSource);
		} else
			spFieldSource.points.Add(point);
	}

	public static void RemovePoint(SPFieldSource prefab, SPFieldPoint point) {
		var source = sources[prefab];
		source.points.Remove(point);
		if (source.points.Count != 0)
			return;
		sources.Remove(prefab);
		Destroy(source.gameObject);
	}
}