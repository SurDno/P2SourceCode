// Decompiled with JetBrains decompiler
// Type: DisableOcclusionCulling
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
