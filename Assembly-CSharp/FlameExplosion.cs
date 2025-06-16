using System.Collections.Generic;
using UnityEngine;

public class FlameExplosion : MonoBehaviour {
	[SerializeField] private FlamePatch flamePatchPrefab;
	[SerializeField] private float radius;
	[SerializeField] private LayerMask collisionLayers;
	[SerializeField] private float rayOriginShift;
	[SerializeField] private int rayCount;
	[SerializeField] private AnimationCurve strengthAnimation;
	[SerializeField] private AnimationCurve decalsAnimation;
	private FlamePatch[] patches;
	private float[] strengths;
	private float time;
	private AudioSource audioSource;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
		var position = this.transform.position;
		var origin = position - this.transform.forward * rayOriginShift;
		var flamePatchList = new List<FlamePatch>();
		var floatList = new List<float>();
		for (var index = 0; index < rayCount; ++index) {
			var onUnitSphere = Random.onUnitSphere;
			RaycastHit hitInfo;
			if (Physics.Raycast(origin, onUnitSphere, out hitInfo, radius + rayOriginShift, collisionLayers,
				    QueryTriggerInteraction.Ignore)) {
				var num1 = Vector3.Distance(hitInfo.point, position);
				if (num1 < (double)radius) {
					var flamePatch = Instantiate(flamePatchPrefab);
					var transform = flamePatch.transform;
					transform.SetParent(this.transform, false);
					transform.position = hitInfo.point;
					transform.rotation = Quaternion.LookRotation(hitInfo.normal, Random.onUnitSphere);
					flamePatch.Initialize(0.0f);
					flamePatchList.Add(flamePatch);
					var num2 = num1 / radius;
					floatList.Add((float)(-(double)num2 * num2 * 0.5));
				}
			}
		}

		patches = flamePatchList.ToArray();
		strengths = floatList.ToArray();
		time = 0.0f;
	}

	private void Update() {
		time += Time.deltaTime;
		var num1 = Mathf.Clamp01(strengthAnimation.Evaluate(time));
		if (audioSource != null)
			audioSource.volume = num1;
		var num2 = Mathf.Clamp01(decalsAnimation.Evaluate(time));
		for (var index = 0; index < patches.Length; ++index) {
			patches[index].Strength = strengths[index] + num1;
			patches[index].DecalOpacity = strengths[index] + num2;
		}

		if (time < (double)decalsAnimation.keys[decalsAnimation.keys.Length - 1].time)
			return;
		gameObject.SetActive(false);
	}
}