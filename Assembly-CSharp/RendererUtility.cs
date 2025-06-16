using System.Collections.Generic;
using UnityEngine;

public static class RendererUtility
{
  private static List<Renderer> searchBuffer;

  public static Renderer GetBiggestRenderer(GameObject gameObject)
  {
    if ((Object) gameObject == (Object) null)
      return (Renderer) null;
    if (RendererUtility.searchBuffer == null)
      RendererUtility.searchBuffer = new List<Renderer>();
    gameObject.GetComponentsInChildren<Renderer>(RendererUtility.searchBuffer);
    float num1 = 0.0f;
    Renderer biggestRenderer = (Renderer) null;
    for (int index = 0; index < RendererUtility.searchBuffer.Count; ++index)
    {
      Renderer renderer = RendererUtility.searchBuffer[index];
      if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
      {
        Vector3 extents = renderer.bounds.extents;
        float num2 = extents.x * extents.y * extents.z;
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          biggestRenderer = renderer;
        }
      }
    }
    RendererUtility.searchBuffer.Clear();
    return biggestRenderer;
  }
}
