using UnityEngine;

[CreateAssetMenu(menuName = "Utility/Terrain Detail Densities")]
public class TerrainDetailDensities : ScriptableObject {
	[Range(0.0f, 1f)] public float[] Densities;
}