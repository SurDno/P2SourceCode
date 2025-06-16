// Decompiled with JetBrains decompiler
// Type: RendererUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
