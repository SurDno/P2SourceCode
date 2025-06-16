using UnityEngine;

public class ProtagonistLight : MonoBehaviour {
	private void LateUpdate() {
		var instance = MonoBehaviourInstance<ProtagonistShadersSettings>.Instance;
		if (instance == null)
			return;
		var position = transform.parent.position;
		transform.position = instance.ProtagonistToWorld(position);
	}
}