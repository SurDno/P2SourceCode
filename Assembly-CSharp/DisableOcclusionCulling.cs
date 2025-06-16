public class DisableOcclusionCulling : MonoBehaviour
{
  private void Start()
  {
    foreach (Camera allCamera in Camera.allCameras)
      allCamera.useOcclusionCulling = false;
    Object.FindObjectOfType<TerrainDetails>().gameObject.SetActive(false);
    Terrain.activeTerrain.drawTreesAndFoliage = false;
  }
}
