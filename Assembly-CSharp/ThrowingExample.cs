using UnityEngine;

public class ThrowingExample : MonoBehaviour {
	[SerializeField] private KeyCode key;
	[SerializeField] private GameObject prefab;
	[SerializeField] private LayerMask layerMask;

	private void Update() {
		if (!Input.GetKeyDown(key))
			return;
		Throw();
	}

	private void Throw() {
		var position = transform.position;
		var forward = transform.forward;
		RaycastHit hitInfo;
		if (!Physics.Raycast(position + forward, forward, out hitInfo, 50f, layerMask, QueryTriggerInteraction.Ignore))
			return;
		var gameObject = Instantiate(prefab);
		gameObject.transform.position = hitInfo.point;
		gameObject.transform.rotation = Quaternion.LookRotation(forward);
	}
}