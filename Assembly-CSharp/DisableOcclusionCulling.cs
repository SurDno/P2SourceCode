using UnityEngine;

public class DisableOcclusionCulling : MonoBehaviour
{
  private void Start()
  {
    foreach (Camera allCamera in Camera.allCameras)
      allCamera.useOcclusionCulling = false;
    FindObjectOfType<TerrainDetails>().gameObject.SetActive(false);
    Terrain.activeTerrain.drawTreesAndFoliage = false;
  }
}
