// Decompiled with JetBrains decompiler
// Type: IndoorCubemap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class IndoorCubemap : MonoBehaviour
{
  [SerializeField]
  private Cubemap cubemap;

  private void OnDisable()
  {
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", 0);
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", (Texture) null);
  }

  private void OnEnable()
  {
    if (!((Object) this.cubemap != (Object) null))
      return;
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", (int) Mathf.Log((float) this.cubemap.width, 2f));
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", (Texture) this.cubemap);
  }
}
