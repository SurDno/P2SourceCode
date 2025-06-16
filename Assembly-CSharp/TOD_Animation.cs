using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TOD_Animation : MonoBehaviour {
	[Tooltip("Wind direction in degrees.")]
	public float WindDegrees;

	[Tooltip("Speed of the wind that is acting on the clouds.")]
	public float WindSpeed = 1f;

	private TOD_Sky sky;

	public Vector3 CloudUV { get; set; }

	public Vector3 OffsetUV {
		get {
			var vector3 = transform.position * 0.0001f;
			return Quaternion.Euler(0.0f, -transform.rotation.eulerAngles.y, 0.0f) * vector3;
		}
	}

	protected void Start() {
		sky = GetComponent<TOD_Sky>();
		CloudUV = new Vector3(Random.value, Random.value, Random.value);
	}

	protected void Update() {
		var num1 = 1f / 1000f * Time.deltaTime;
		var f1 = WindSpeed * num1;
		if (float.IsNaN(f1) || float.IsNaN(f1))
			return;
		var num2 = Mathf.Sin((float)Math.PI / 180f * WindDegrees);
		var num3 = Mathf.Cos((float)Math.PI / 180f * WindDegrees);
		var x1 = CloudUV.x;
		var y1 = CloudUV.y;
		var z1 = CloudUV.z;
		var f2 = y1 + num1 * 0.1f;
		var f3 = x1 - f1 * num2;
		var f4 = z1 - f1 * num3;
		var x2 = f3 - Mathf.Floor(f3);
		var y2 = f2 - Mathf.Floor(f2);
		var z2 = f4 - Mathf.Floor(f4);
		CloudUV = new Vector3(x2, y2, z2);
		Quaternion.Euler(0.0f, (float)(WindSpeed * (double)y2 * 360.0), 0.0f);
		sky.Components.BillboardTransform.localRotation =
			Quaternion.Euler(0.0f, (float)(WindSpeed * (double)y2 * 360.0), 0.0f);
	}
}