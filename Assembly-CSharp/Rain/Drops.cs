using System;
using UnityEngine;

namespace Rain;

public class Drops : MonoBehaviour {
	public float animationLength = 1f;
	public float nearRadius = 5f;
	public float farRadius = 25f;
	public int maxParticles = 8192;
	public DropsMesh[] dropsMeshes;
	private float _animationPhase;
	private int _dropMeshToUpdate;
	private Material _material;
	private VertexBuffer _buffer;
	private int _activeMeshes;

	private void DecreaseActiveMeshCount() {
		if (_activeMeshes == 0)
			Debug.LogError("Drops : Trying to decrease active mesh count when it is aready zero.");
		else {
			--_activeMeshes;
			if (_activeMeshes != 0)
				return;
			GameCamera.Instance.Camera.GetComponent<BlurBehind>().enabled = false;
		}
	}

	private void IncreaseActiveMeshCount() {
		if (_activeMeshes == 0)
			GameCamera.Instance.Camera.GetComponent<BlurBehind>().enabled = true;
		++_activeMeshes;
	}

	private void Start() {
		var component = dropsMeshes[0].GetComponent<MeshRenderer>();
		_material = new Material(component.sharedMaterial);
		_material.name += " (Instance)";
		component.sharedMaterial = _material;
		_buffer = new VertexBuffer();
	}

	private void Update() {
		var dropsMesh = dropsMeshes[_dropMeshToUpdate];
		var instance = RainManager.Instance;
		var num1 = !(instance != null)
			? 0
			: Mathf.RoundToInt(instance.actualRainIntensity * maxParticles / dropsMeshes.Length);
		if (num1 > 0) {
			var num2 = Mathf.Lerp(nearRadius, farRadius, _dropMeshToUpdate / (float)(dropsMeshes.Length - 1));
			if (dropsMesh == null) {
				var gameObject = Instantiate(dropsMeshes[0].gameObject);
				gameObject.transform.SetParent(transform, false);
				gameObject.name = "Drops Mesh " + _dropMeshToUpdate;
				var component = gameObject.GetComponent<DropsMesh>();
				component.count = num1;
				component.radius = num2;
				component.gameObject.SetActive(true);
				dropsMeshes[_dropMeshToUpdate] = component;
				IncreaseActiveMeshCount();
			} else {
				dropsMesh.count = num1;
				dropsMesh.radius = num2;
				if (dropsMesh.gameObject.activeSelf)
					dropsMesh.UpdateMesh(_buffer);
				else {
					dropsMesh.CreateMesh(_buffer);
					dropsMesh.gameObject.SetActive(true);
					IncreaseActiveMeshCount();
				}
			}
		} else if (_dropMeshToUpdate == 0) {
			if (dropsMesh == null)
				throw new Exception("updatingMesh == null");
			if (dropsMesh.gameObject.activeSelf) {
				dropsMesh.gameObject.SetActive(false);
				DecreaseActiveMeshCount();
			}
		} else if (dropsMesh != null) {
			Destroy(dropsMesh.gameObject);
			dropsMeshes[_dropMeshToUpdate] = null;
			DecreaseActiveMeshCount();
		}

		++_dropMeshToUpdate;
		if (_dropMeshToUpdate >= dropsMeshes.Length)
			_dropMeshToUpdate = 0;
		_animationPhase += Time.deltaTime / animationLength;
		double num3 = Mathf.Repeat(_animationPhase, 1f);
		_material.SetFloat("_Phase", _animationPhase);
	}
}