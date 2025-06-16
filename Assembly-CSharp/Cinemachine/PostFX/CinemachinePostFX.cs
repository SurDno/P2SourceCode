using UnityEngine;
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
      mBrain = GetComponent<CinemachineBrain>();
      if (mBrain != null)
      {
        mBrain.m_CameraCutEvent.RemoveListener(OnCameraCut);
        mBrain.m_CameraCutEvent.AddListener(OnCameraCut);
      }
      mPostProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
      if (!(mPostProcessingBehaviour == null) || !(mBrain != null))
        return;
      mPostProcessingBehaviour = gameObject.AddComponent<PostProcessingBehaviour>();
    }

    private void OnDestroy()
    {
      if (!(mBrain != null))
        return;
      mBrain.m_CameraCutEvent.RemoveListener(OnCameraCut);
    }

    internal void PostFXHandler(CinemachineBrain brain)
    {
      ICinemachineCamera activeVirtualCamera = brain.ActiveVirtualCamera;
      if (!enabled || !(mBrain != null) || !(mPostProcessingBehaviour != null))
        return;
      CinemachinePostFX cinemachinePostFx = GetEffectivePostFX(activeVirtualCamera);
      if (cinemachinePostFx == null)
        cinemachinePostFx = this;
      if (cinemachinePostFx.m_Profile != null)
      {
        CameraState currentCameraState = brain.CurrentCameraState;
        DepthOfFieldModel.Settings settings = cinemachinePostFx.m_Profile.depthOfField.settings;
        if (cinemachinePostFx.m_FocusTracksTarget && currentCameraState.HasLookAt)
          settings.focusDistance = (currentCameraState.FinalPosition - currentCameraState.ReferenceLookAt).magnitude + cinemachinePostFx.m_FocusOffset;
        cinemachinePostFx.m_Profile.depthOfField.settings = settings;
      }
      if (mPostProcessingBehaviour.profile != cinemachinePostFx.m_Profile)
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
      for (effectivePostFx = null; vcam != null && effectivePostFx == null; vcam = vcam.ParentCamera)
      {
        CinemachineVirtualCameraBase virtualCameraBase = vcam as CinemachineVirtualCameraBase;
        if (virtualCameraBase != null)
          effectivePostFx = virtualCameraBase.GetComponent<CinemachinePostFX>();
        if (effectivePostFx != null && !effectivePostFx.enabled)
          effectivePostFx = null;
      }
      return effectivePostFx;
    }

    private void OnCameraCut(CinemachineBrain brain)
    {
      if (!(mPostProcessingBehaviour != null))
        return;
      mPostProcessingBehaviour.ResetTemporalEffects();
    }

    private static void StaticPostFXHandler(CinemachineBrain brain)
    {
      CinemachinePostFX processingComponent = brain.PostProcessingComponent as CinemachinePostFX;
      if (processingComponent == null)
      {
        brain.PostProcessingComponent = brain.GetComponent<CinemachinePostFX>();
        processingComponent = brain.PostProcessingComponent as CinemachinePostFX;
        if (processingComponent != null)
          processingComponent.ConnectToBrain();
      }
      if (!(processingComponent != null))
        return;
      processingComponent.PostFXHandler(brain);
    }

    [RuntimeInitializeOnLoadMethod]
    public static void InitializeModule()
    {
      CinemachineBrain.sPostProcessingHandler.RemoveListener(StaticPostFXHandler);
      CinemachineBrain.sPostProcessingHandler.AddListener(StaticPostFXHandler);
    }
  }
}
