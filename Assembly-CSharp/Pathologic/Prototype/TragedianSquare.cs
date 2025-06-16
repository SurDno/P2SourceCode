using UnityEngine;
using UnityEngine.AI;

namespace Pathologic.Prototype;

public class TragedianSquare : MonoBehaviour {
	private Vector3 _lastPosition;
	public Transform CapsuleTransform;
	public bool TragedianA;

	private void Start() {
		_lastPosition = Vector3.zero;
		if (TragedianA)
			GetComponent<Animator>().SetBool("TragedianA", true);
		else
			GetComponent<Animator>().SetBool("TragedianB", true);
	}

	private void Update() {
		GetComponent<CapsuleCollider>().center = gameObject.transform.InverseTransformPoint(CapsuleTransform.position);
		if ((_lastPosition - CapsuleTransform.position).magnitude <= 0.30000001192092896)
			return;
		GetComponent<NavMeshObstacle>().center = GetComponent<CapsuleCollider>().center;
		_lastPosition = CapsuleTransform.position;
	}
}