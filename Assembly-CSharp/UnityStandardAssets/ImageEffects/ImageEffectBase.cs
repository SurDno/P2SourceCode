using UnityEngine;

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
