using UnityEngine;
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
      postProcessingOverride = GetComponent<PostProcessingStackOverride>();
    }

    private void ConnectToBrain()
    {
      brain = GetComponent<CinemachineBrain>();
      if (!(brain != null))
        return;
      brain.m_CameraCutEvent.RemoveListener(OnCameraCut);
      brain.m_CameraCutEvent.AddListener(OnCameraCut);
    }

    private void OnDestroy()
    {
      if (!(brain != null))
        return;
      brain.m_CameraCutEvent.RemoveListener(OnCameraCut);
    }

    internal void PostFXHandler(CinemachineBrain brain)
    {
      if (!enabled || !(this.brain != null) || !(postProcessingOverride != null))
        return;
      CinemachinePostProcessingOverride processingOverride = GetEffectivePostFX(brain.ActiveVirtualCamera);
      if (processingOverride != null)
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
        if (!depthOfFieldOverride.Override && processingOverride != this)
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
      for (effectivePostFx = null; vcam != null && effectivePostFx == null; vcam = vcam.ParentCamera)
      {
        CinemachineVirtualCameraBase virtualCameraBase = vcam as CinemachineVirtualCameraBase;
        if (virtualCameraBase != null)
          effectivePostFx = virtualCameraBase.GetComponent<CinemachinePostProcessingOverride>();
        if (effectivePostFx != null && !effectivePostFx.enabled)
          effectivePostFx = null;
      }
      return effectivePostFx;
    }

    private void OnCameraCut(CinemachineBrain brain)
    {
      GetComponent<PostProcessingBehaviour>()?.ResetTemporalEffects();
    }

    private static void StaticPostFXHandler(CinemachineBrain brain)
    {
      CinemachinePostProcessingOverride processingComponent = brain.PostProcessingComponent as CinemachinePostProcessingOverride;
      if (processingComponent == null)
      {
        brain.PostProcessingComponent = brain.GetComponent<CinemachinePostProcessingOverride>();
        processingComponent = brain.PostProcessingComponent as CinemachinePostProcessingOverride;
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
