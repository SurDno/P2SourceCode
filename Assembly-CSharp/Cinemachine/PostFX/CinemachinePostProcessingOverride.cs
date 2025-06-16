using UnityEngine.PostProcessing;

namespace Cinemachine.PostFX
{
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachinePostProcessingOverride")]
  [RequireComponent(typeof (PostProcessingStackOverride))]
  [SaveDuringPlay]
  public class CinemachinePostProcessingOverride : MonoBehaviour
  {
    [SerializeField]
    [Tooltip("If checked, then the Focus Distance will be set to the distance between the camera and the LookAt target.")]
    private bool focusTracksTarget;
    [SerializeField]
    [Tooltip("Offset from target distance, to be used with Focus Tracks Target.")]
    private float focusOffset;
    private CinemachineBrain brain;
    private PostProcessingStackOverride postProcessingOverride;

    private void OnEnable()
    {
      postProcessingOverride = this.GetComponent<PostProcessingStackOverride>();
    }

    private void ConnectToBrain()
    {
      brain = this.GetComponent<CinemachineBrain>();
      if (!((Object) brain != (Object) null))
        return;
      brain.m_CameraCutEvent.RemoveListener(new UnityAction<CinemachineBrain>(OnCameraCut));
      brain.m_CameraCutEvent.AddListener(new UnityAction<CinemachineBrain>(OnCameraCut));
    }

    private void OnDestroy()
    {
      if (!((Object) brain != (Object) null))
        return;
      brain.m_CameraCutEvent.RemoveListener(new UnityAction<CinemachineBrain>(OnCameraCut));
    }

    internal void PostFXHandler(CinemachineBrain brain)
    {
      if (!this.enabled || !((Object) this.brain != (Object) null) || !((Object) postProcessingOverride != (Object) null))
        return;
      CinemachinePostProcessingOverride processingOverride = GetEffectivePostFX(brain.ActiveVirtualCamera);
      if ((Object) processingOverride != (Object) null)
      {
        postProcessingOverride.NestedOverride = processingOverride.postProcessingOverride;
      }
      else
      {
        processingOverride = this;
        postProcessingOverride.NestedOverride = null;
      }
      CameraState currentCameraState = brain.CurrentCameraState;
      if (currentCameraState.HasLookAt && processingOverride.focusTracksTarget)
      {
        PostProcessingStackOverride.DepthOfFieldOverride depthOfFieldOverride = postProcessingOverride.DepthOfField;
        if (!depthOfFieldOverride.Override && (Object) processingOverride != (Object) this)
          depthOfFieldOverride = processingOverride.postProcessingOverride?.DepthOfField;
        if (depthOfFieldOverride != null && !depthOfFieldOverride.Override)
          depthOfFieldOverride = null;
        if (depthOfFieldOverride != null)
        {
          float num = (currentCameraState.FinalPosition - currentCameraState.ReferenceLookAt).magnitude + focusOffset;
          depthOfFieldOverride.FocusDistance = num;
        }
      }
    }

    private CinemachinePostProcessingOverride GetEffectivePostFX(ICinemachineCamera vcam)
    {
      while (vcam != null && vcam.LiveChildOrSelf != vcam)
        vcam = vcam.LiveChildOrSelf;
      CinemachinePostProcessingOverride effectivePostFx;
      for (effectivePostFx = null; vcam != null && (Object) effectivePostFx == (Object) null; vcam = vcam.ParentCamera)
      {
        CinemachineVirtualCameraBase virtualCameraBase = vcam as CinemachineVirtualCameraBase;
        if ((Object) virtualCameraBase != (Object) null)
          effectivePostFx = virtualCameraBase.GetComponent<CinemachinePostProcessingOverride>();
        if ((Object) effectivePostFx != (Object) null && !effectivePostFx.enabled)
          effectivePostFx = null;
      }
      return effectivePostFx;
    }

    private void OnCameraCut(CinemachineBrain brain)
    {
      this.GetComponent<PostProcessingBehaviour>()?.ResetTemporalEffects();
    }

    private static void StaticPostFXHandler(CinemachineBrain brain)
    {
      CinemachinePostProcessingOverride processingComponent = brain.PostProcessingComponent as CinemachinePostProcessingOverride;
      if ((Object) processingComponent == (Object) null)
      {
        brain.PostProcessingComponent = (Component) brain.GetComponent<CinemachinePostProcessingOverride>();
        processingComponent = brain.PostProcessingComponent as CinemachinePostProcessingOverride;
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
