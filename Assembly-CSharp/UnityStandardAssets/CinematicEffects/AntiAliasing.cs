namespace UnityStandardAssets.CinematicEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Anti-aliasing")]
  [ImageEffectAllowedInSceneView]
  public class AntiAliasing : MonoBehaviour
  {
    [SerializeField]
    private SMAA m_SMAA = new SMAA();
    [SerializeField]
    private FXAA m_FXAA = new FXAA();
    [SerializeField]
    [HideInInspector]
    private int m_Method;
    private Camera m_Camera;

    public int method
    {
      get => m_Method;
      set
      {
        if (m_Method == value)
          return;
        m_Method = value;
      }
    }

    public IAntiAliasing current
    {
      get => method == 0 ? m_SMAA : m_FXAA;
    }

    public Camera cameraComponent
    {
      get
      {
        if ((Object) m_Camera == (Object) null)
          m_Camera = this.GetComponent<Camera>();
        return m_Camera;
      }
    }

    private void OnEnable()
    {
      m_SMAA.OnEnable(this);
      m_FXAA.OnEnable(this);
    }

    private void OnDisable()
    {
      m_SMAA.OnDisable();
      m_FXAA.OnDisable();
    }

    private void OnPreCull() => current.OnPreCull(cameraComponent);

    private void OnPostRender() => current.OnPostRender(cameraComponent);

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      current.OnRenderImage(cameraComponent, source, destination);
    }

    public enum Method
    {
      Smaa,
      Fxaa,
    }
  }
}
