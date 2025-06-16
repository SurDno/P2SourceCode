using Engine.Source.Commons;
using Engine.Source.Settings;

public class GameCamera : EngineDependent
{
  private float additionalFov;
  private float cutsceneFov;
  [SerializeField]
  private Camera camera;
  [SerializeField]
  private Transform cameraTransfrom;
  [SerializeField]
  private Camera[] additionalCameras;
  [SerializeField]
  private PostProcessingStackOverride settingsPostProcessingOverride;
  [SerializeField]
  private PostProcessingStackOverride gamePostProcessingOverride;
  private Animator animator;

  public static GameCamera Instance { get; private set; }

  public Camera Camera => camera;

  public Transform CameraTransform => cameraTransfrom;

  public float AdditionalFov
  {
    get => additionalFov;
    set
    {
      if (additionalFov == (double) value)
        return;
      additionalFov = value;
      if (!Connected)
        return;
      ApplyFov();
    }
  }

  public PostProcessingStackOverride GamePostProcessingOverride => gamePostProcessingOverride;

  public PostProcessingStackOverride SettingsPostProcessingOverride
  {
    get => settingsPostProcessingOverride;
  }

  private void Awake()
  {
    Instance = this;
    animator = this.GetComponent<Animator>();
  }

  private void OnPauseEvent()
  {
    if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      animator.SetFloat("Mecanim.Speed", 0.0f);
    else
      animator.SetFloat("Mecanim.Speed", 1f);
  }

  protected override void OnConnectToEngine()
  {
    ApplyFov();
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += ApplyFov;
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += OnPauseEvent;
    InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent += OnViewEnabledEvent;
  }

  protected override void OnDisconnectFromEngine()
  {
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= ApplyFov;
    InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent -= OnViewEnabledEvent;
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= OnPauseEvent;
  }

  private void ApplyFov()
  {
    float num = cutsceneFov != 0.0 ? cutsceneFov : InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value + additionalFov;
    Camera.fieldOfView = num;
    for (int index = 0; index < additionalCameras.Length; ++index)
      additionalCameras[index].fieldOfView = num;
  }

  private void OnViewEnabledEvent(bool enable)
  {
    if (enable)
      return;
    animator.SetTrigger("Stop");
  }

  public void ResetCutsceneFov() => SetCutsceneFov(0.0f);

  public void SetCutsceneFov(float fov)
  {
    if (cutsceneFov == (double) fov)
      return;
    cutsceneFov = fov;
    ApplyFov();
  }
}
