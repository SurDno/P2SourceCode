using UnityEngine.PostProcessing;

namespace Cinemachine.PostFX
{
  [DocumentationSorting(100f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachinePostFX")]
  [SaveDuringPlay]
  public class CinemachinePostFX : MonoBehaviour
  {
    [Tooltip("When this behaviour is on a Unity Camera, this setting is the default Post-Processing profile for the camera, and will be applied whenever it is not overridden by a virtual camera.  When the behaviour is on a virtual camera, then this is the Post-Processing profile that will become active whenever this virtual camera is live")]
    public PostProcessingProfile m_Profile;
    [Tooltip("If checked, then the Focus Distance will be set to the distance between the camera and the LookAt target.")]
    public bool m_FocusTracksTarget;
    [Tooltip("Offset from target distance, to be used with Focus Tracks Target.")]
    public float m_FocusOffset;
    private CinemachineBrain mBrain;
    private PostProcessingBehaviour mPostProcessingBehaviour;

    private void Update()
    {
    }

    private void ConnectToBrain()
    {
      mBrain = this.GetComponent<CinemachineBrain>();
      if ((Object) mBrain != (Object) null)
      {
        mBrain.m_CameraCutEvent.RemoveListener(new UnityAction<CinemachineBrain>(OnCameraCut));
        mBrain.m_CameraCutEvent.AddListener(new UnityAction<CinemachineBrain>(OnCameraCut));
      }
      mPostProcessingBehaviour = this.GetComponent<PostProcessingBehaviour>();
      if (!((Object) mPostProcessingBehaviour == (Object) null) || !((Object) mBrain != (Object) null))
        return;
      mPostProcessingBehaviour = this.gameObject.AddComponent<PostProcessingBehaviour>();
    }

    private void OnDestroy()
    {
      if (!((Object) mBrain != (Object) null))
        return;
      mBrain.m_CameraCutEvent.RemoveListener(new UnityAction<CinemachineBrain>(OnCameraCut));
    }

    internal void PostFXHandler(CinemachineBrain brain)
    {
      ICinemachineCamera activeVirtualCamera = brain.ActiveVirtualCamera;
      if (!this.enabled || !((Object) mBrain != (Object) null) || !((Object) mPostProcessingBehaviour != (Object) null))
        return;
      CinemachinePostFX cinemachinePostFx = GetEffectivePostFX(activeVirtualCamera);
      if ((Object) cinemachinePostFx == (Object) null)
        cinemachinePostFx = this;
      if ((Object) cinemachinePostFx.m_Profile != (Object) null)
      {
        CameraState currentCameraState = brain.CurrentCameraState;
        DepthOfFieldModel.Settings settings = cinemachinePostFx.m_Profile.depthOfField.settings;
        if (cinemachinePostFx.m_FocusTracksTarget && currentCameraState.HasLookAt)
          settings.focusDistance = (currentCameraState.FinalPosition - currentCameraState.ReferenceLookAt).magnitude + cinemachinePostFx.m_FocusOffset;
        cinemachinePostFx.m_Profile.depthOfField.settings = settings;
      }
      if ((Object) mPostProcessingBehaviour.profile != (Object) cinemachinePostFx.m_Profile)
      {
        mPostProcessingBehaviour.profile = cinemachinePostFx.m_Profile;
        mPostProcessingBehaviour.ResetTemporalEffects();
      }
    }

    private CinemachinePostFX GetEffectivePostFX(ICinemachineCamera vcam)
    {
      while (vcam != null && vcam.LiveChildOrSelf != vcam)
        vcam = vcam.LiveChildOrSelf;
      CinemachinePostFX effectivePostFx;
      for (effectivePostFx = null; vcam != null && (Object) effectivePostFx == (Object) null; vcam = vcam.ParentCamera)
      {
        CinemachineVirtualCameraBase virtualCameraBase = vcam as CinemachineVirtualCameraBase;
        if ((Object) virtualCameraBase != (Object) null)
          effectivePostFx = virtualCameraBase.GetComponent<CinemachinePostFX>();
        if ((Object) effectivePostFx != (Object) null && !effectivePostFx.enabled)
          effectivePostFx = null;
      }
      return effectivePostFx;
    }

    private void OnCameraCut(CinemachineBrain brain)
    {
      if (!((Object) mPostProcessingBehaviour != (Object) null))
        return;
      mPostProcessingBehaviour.ResetTemporalEffects();
    }

    private static void StaticPostFXHandler(CinemachineBrain brain)
    {
      CinemachinePostFX processingComponent = brain.PostProcessingComponent as CinemachinePostFX;
      if ((Object) processingComponent == (Object) null)
      {
        brain.PostProcessingComponent = (Component) brain.GetComponent<CinemachinePostFX>();
        processingComponent = brain.PostProcessingComponent as CinemachinePostFX;
        if ((Object) processingComponent != (Object) null)
          processingComponent.ConnectToBrain();
      }
      if (!((Object) processingComponent != (Object) null))
        return;
      processingComponent.PostFXHandler(brain);
    }

    [RuntimeInitializeOnLoadMethod]
    public static void InitializeModule()
    {
      CinemachineBrain.sPostProcessingHandler.RemoveListener(new UnityAction<CinemachineBrain>(StaticPostFXHandler));
      CinemachineBrain.sPostProcessingHandler.AddListener(new UnityAction<CinemachineBrain>(StaticPostFXHandler));
    }
  }
}
