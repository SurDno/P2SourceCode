using Engine.Source.Commons;
using Engine.Source.Settings;
using System;
using UnityEngine;

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

  public Camera Camera => this.camera;

  public Transform CameraTransform => this.cameraTransfrom;

  public float AdditionalFov
  {
    get => this.additionalFov;
    set
    {
      if ((double) this.additionalFov == (double) value)
        return;
      this.additionalFov = value;
      if (!this.Connected)
        return;
      this.ApplyFov();
    }
  }

  public PostProcessingStackOverride GamePostProcessingOverride => this.gamePostProcessingOverride;

  public PostProcessingStackOverride SettingsPostProcessingOverride
  {
    get => this.settingsPostProcessingOverride;
  }

  private void Awake()
  {
    GameCamera.Instance = this;
    this.animator = this.GetComponent<Animator>();
  }

  private void OnPauseEvent()
  {
    if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
      return;
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      this.animator.SetFloat("Mecanim.Speed", 0.0f);
    else
      this.animator.SetFloat("Mecanim.Speed", 1f);
  }

  protected override void OnConnectToEngine()
  {
    this.ApplyFov();
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += new Action(this.ApplyFov);
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent += new Action(this.OnPauseEvent);
    InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent += new Action<bool>(this.OnViewEnabledEvent);
  }

  protected override void OnDisconnectFromEngine()
  {
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= new Action(this.ApplyFov);
    InstanceByRequest<EngineApplication>.Instance.OnViewEnabledEvent -= new Action<bool>(this.OnViewEnabledEvent);
    InstanceByRequest<EngineApplication>.Instance.OnPauseEvent -= new Action(this.OnPauseEvent);
  }

  private void ApplyFov()
  {
    float num = (double) this.cutsceneFov != 0.0 ? this.cutsceneFov : InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value + this.additionalFov;
    this.Camera.fieldOfView = num;
    for (int index = 0; index < this.additionalCameras.Length; ++index)
      this.additionalCameras[index].fieldOfView = num;
  }

  private void OnViewEnabledEvent(bool enable)
  {
    if (enable)
      return;
    this.animator.SetTrigger("Stop");
  }

  public void ResetCutsceneFov() => this.SetCutsceneFov(0.0f);

  public void SetCutsceneFov(float fov)
  {
    if ((double) this.cutsceneFov == (double) fov)
      return;
    this.cutsceneFov = fov;
    this.ApplyFov();
  }
}
