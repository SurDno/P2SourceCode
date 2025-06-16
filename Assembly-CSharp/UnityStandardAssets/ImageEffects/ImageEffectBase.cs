// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.ImageEffectBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("")]
  public class ImageEffectBase : MonoBehaviour
  {
    public Shader shader;
    private Material m_Material;

    protected virtual void Start()
    {
      if (!SystemInfo.supportsImageEffects)
      {
        this.enabled = false;
      }
      else
      {
        if ((bool) (Object) this.shader && this.shader.isSupported)
          return;
        this.enabled = false;
      }
    }

    protected Material material
    {
      get
      {
        if ((Object) this.m_Material == (Object) null)
        {
          this.m_Material = new Material(this.shader);
          this.m_Material.hideFlags = HideFlags.HideAndDontSave;
        }
        return this.m_Material;
      }
    }

    protected virtual void OnDisable()
    {
      if (!(bool) (Object) this.m_Material)
        return;
      Object.DestroyImmediate((Object) this.m_Material);
    }
  }
}
