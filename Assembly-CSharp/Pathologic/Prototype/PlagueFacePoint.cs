using UnityEngine;

namespace Pathologic.Prototype;

[ExecuteInEditMode]
public class PlagueFacePoint : MonoBehaviour {
	[Space] public PlagueFacePoint[] neighbors;
	public bool roamable = true;

	private void OnDrawGizmos() {
		var localToWorldMatrix = transform.localToWorldMatrix;
		Gizmos.color = roamable ? Color.green : Color.yellow;
		Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f)), 0.1f);
		if (neighbors == null || neighbors.Length == 0)
			return;
		var from = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f));
		var vector3_1 = from * 0.5f;
		for (var index = 0; index < neighbors.Length; ++index)
			if (!(neighbors[index] == null)) {
				var vector3_2 = neighbors[index].transform.localToWorldMatrix
					.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.1f));
				Gizmos.DrawLine(from, vector3_1 + vector3_2 * 0.5f);
			}
	}

	private void OnDrawGizmosSelected() {
		var localToWorldMatrix = transform.localToWorldMatrix;
		Gizmos.color = roamable ? Color.green : Color.yellow;
		var num = 1f;
		var vector3_1 = localToWorldMatrix.MultiplyPoint(new Vector3(num, 0.0f, 0.0f));
		var vector3_2 = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, num, 0.0f));
		var vector3_3 = localToWorldMatrix.MultiplyPoint(new Vector3(-num, 0.0f, 0.0f));
		var vector3_4 = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, -num, 0.0f));
		var to = localToWorldMatrix.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.25f));
		Gizmos.DrawLine(vector3_1, vector3_2);
		Gizmos.DrawLine(vector3_1, to);
		Gizmos.DrawLine(vector3_2, vector3_3);
		Gizmos.DrawLine(vector3_2, to);
		Gizmos.DrawLine(vector3_3, vector3_4);
		Gizmos.DrawLine(vector3_3, to);
		Gizmos.DrawLine(vector3_4, vector3_1);
		Gizmos.DrawLine(vector3_4, to);
	}
}