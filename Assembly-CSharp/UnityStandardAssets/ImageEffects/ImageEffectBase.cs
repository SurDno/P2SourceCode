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
        if ((bool) (Object) shader && shader.isSupported)
          return;
        this.enabled = false;
      }
    }

    protected Material material
    {
      get
      {
        if ((Object) m_Material == (Object) null)
        {
          m_Material = new Material(shader);
          m_Material.hideFlags = HideFlags.HideAndDontSave;
        }
        return m_Material;
      }
    }

    protected virtual void OnDisable()
    {
      if (!(bool) (Object) m_Material)
        return;
      Object.DestroyImmediate((Object) m_Material);
    }
  }
}
