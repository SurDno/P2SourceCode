using UnityEngine;

public class HerbRootsSpawnPoint : MonoBehaviour {
	private void OnDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawCube(transform.position + new Vector3(0.0f, 0.5f, 0.0f), new Vector3(0.05f, 1f, 0.05f));
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(transform.position, 0.1f);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(0.2f, 0.05f, 0.2f));
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(transform.position + Vector3.up * 1.05f, new Vector3(0.05f, 0.05f, 0.05f));
	}
}