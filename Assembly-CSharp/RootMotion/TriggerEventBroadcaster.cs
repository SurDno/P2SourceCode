using UnityEngine;

namespace RootMotion;

public class TriggerEventBroadcaster : MonoBehaviour {
	public GameObject target;

	private void OnTriggerEnter(Collider collider) {
		if (!(target != null))
			return;
		target.SendMessage(nameof(OnTriggerEnter), collider, SendMessageOptions.DontRequireReceiver);
	}

	private void OnTriggerStay(Collider collider) {
		if (!(target != null))
			return;
		target.SendMessage(nameof(OnTriggerStay), collider, SendMessageOptions.DontRequireReceiver);
	}

	private void OnTriggerExit(Collider collider) {
		if (!(target != null))
			return;
		target.SendMessage(nameof(OnTriggerExit), collider, SendMessageOptions.DontRequireReceiver);
	}
}