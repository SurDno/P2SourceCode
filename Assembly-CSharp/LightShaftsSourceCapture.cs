// Decompiled with JetBrains decompiler
// Type: LightShaftsSourceCapture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[RequireComponent(typeof (Light))]
public class LightShaftsSourceCapture : MonoBehaviour
{
  public LayerMask shadowCastingLayers;

  private void OnDisable()
  {
    LightShafts.isLightActive = false;
    Shader.SetGlobalColor("_SunlightColor", Color.black);
  }

  private void Update()
  {
    Light component = this.GetComponent<Light>();
    LightShafts.isLightActive = (double) component.intensity > 0.0;
    LightShafts.LightDirection = this.transform.forward;
    LightShafts.shadowCastingColliders = this.shadowCastingLayers;
    Shader.SetGlobalColor("_SunlightColor", (component.color * component.intensity) with
    {
      a = 1f
    });
  }
}
