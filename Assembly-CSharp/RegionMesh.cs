using System;
using Engine.Common.Components.Regions;
using Inspectors;
using UnityEngine;
using Random = UnityEngine.Random;

public class RegionMesh : MonoBehaviour {
	[SerializeField] [HideInInspector] private Vector3 center;
	[SerializeField] [HideInInspector] private float radius;
	[SerializeField] private RegionEnum region;
	[SerializeField] private RegionMesh[] nearRegions;
	private Vector3[] trianglesCache = new Vector3[0];
	private float[] areaCache = new float[0];

	public RegionEnum Region => region;

	[Inspected] public Vector3 Center => center;

	[Inspected] public float Radius => radius;

	[Inspected] public RegionMesh[] NearRegions => nearRegions;

	public Vector3[] Triangles => trianglesCache;

	public void Initialise() {
		var component = GetComponent<MeshCollider>();
		if (component == null)
			return;
		var sharedMesh = component.sharedMesh;
		var numArray = !(sharedMesh == null) ? sharedMesh.triangles : throw new Exception();
		var length = numArray.Length / 3;
		if (length == 0)
			throw new Exception();
		trianglesCache = new Vector3[numArray.Length];
		areaCache = new float[length];
		var vertices = sharedMesh.vertices;
		var num = 0.0f;
		for (var index = 0; index < length; ++index) {
			var vector3_1 = vertices[numArray[index * 3]];
			var vector3_2 = vertices[numArray[index * 3 + 1]];
			var vector3_3 = vertices[numArray[index * 3 + 2]];
			num += Vector3.Cross(vector3_3 - vector3_1, vector3_2 - vector3_1).magnitude;
			areaCache[index] = num;
			trianglesCache[index * 3] = vector3_1;
			trianglesCache[index * 3 + 1] = vector3_2;
			trianglesCache[index * 3 + 2] = vector3_3;
		}

		for (var index = 0; index < length; ++index)
			areaCache[index] = areaCache[index] / num;
		areaCache[areaCache.Length - 1] = 1.0001f;
	}

	public Vector3 GetRandomPoint() {
		var num1 = Random.value;
		var index = 0;
		while (num1 > (double)areaCache[index])
			++index;
		var vector3_1 = trianglesCache[index * 3];
		var vector3_2 = trianglesCache[index * 3 + 1];
		var vector3_3 = trianglesCache[index * 3 + 2];
		var num2 = Random.value;
		var num3 = Random.value;
		Vector3 worldPosition;
		if (num2 + (double)num3 < 1.0)
			worldPosition = vector3_1 + (vector3_2 - vector3_1) * num2 + (vector3_3 - vector3_1) * num3;
		else {
			var vector3_4 = vector3_1 + (vector3_2 - vector3_1) * 1f + (vector3_3 - vector3_1) * 1f;
			worldPosition = vector3_4 + (vector3_2 - vector3_4) * num2 + (vector3_3 - vector3_4) * num3;
		}

		worldPosition.y = Terrain.activeTerrain.SampleHeight(worldPosition);
		return worldPosition;
	}
}