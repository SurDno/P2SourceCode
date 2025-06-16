using System.Collections.Generic;

public static class RendererUtility
{
  private static List<Renderer> searchBuffer;

  public static Renderer GetBiggestRenderer(GameObject gameObject)
  {
    if ((Object) gameObject == (Object) null)
      return (Renderer) null;
    if (searchBuffer == null)
      searchBuffer = new List<Renderer>();
    gameObject.GetComponentsInChildren<Renderer>(searchBuffer);
    float num1 = 0.0f;
    Renderer biggestRenderer = (Renderer) null;
    for (int index = 0; index < searchBuffer.Count; ++index)
    {
      Renderer renderer = searchBuffer[index];
      if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
      {
        Vector3 extents = renderer.bounds.extents;
        float num2 = extents.x * extents.y * extents.z;
        if (num2 > (double) num1)
        {
          num1 = num2;
          biggestRenderer = renderer;
        }
      }
    }
    searchBuffer.Clear();
    return biggestRenderer;
  }
}
