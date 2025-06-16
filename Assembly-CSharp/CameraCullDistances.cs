using System;
using Inspectors;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCullDistances : MonoBehaviour {
	[SerializeField] private LayerFarClipping[] layerFarClippings = new LayerFarClipping[0];
	[SerializeField] private float defaultFarClippingPlane = 150f;

	private void Awake() {
		ApplyImpl();
	}

	[Inspected]
	private void Apply() {
		ApplyImpl();
	}

	private void ApplyImpl() {
		var component = GetComponent<Camera>();
		var numArray = new float[32];
		for (var index = 0; index < 32; ++index)
			numArray[index] = defaultFarClippingPlane;
		foreach (var layerFarClipping in layerFarClippings) {
			var index = layerFarClipping.Layer.GetIndex();
			numArray[index] = layerFarClipping.FarClippingPlane;
		}

		component.layerCullDistances = numArray;
	}

	[Serializable]
	public class LayerFarClipping {
		public LayerMask Layer;
		public float FarClippingPlane;
	}
}