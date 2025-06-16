// Decompiled with JetBrains decompiler
// Type: Cinemachine.PostFX.CinemachinePostFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;

#nullable disable
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
      this.mBrain = this.GetComponent<CinemachineBrain>();
      if ((Object) this.mBrain != (Object) null)
      {
        this.mBrain.m_CameraCutEvent.RemoveListener(new UnityAction<CinemachineBrain>(this.OnCameraCut));
        this.mBrain.m_CameraCutEvent.AddListener(new UnityAction<CinemachineBrain>(this.OnCameraCut));
      }
      this.mPostProcessingBehaviour = this.GetComponent<PostProcessingBehaviour>();
      if (!((Object) this.mPostProcessingBehaviour == (Object) null) || !((Object) this.mBrain != (Object) null))
        return;
      this.mPostProcessingBehaviour = this.gameObject.AddComponent<PostProcessingBehaviour>();
    }

    private void OnDestroy()
    {
      if (!((Object) this.mBrain != (Object) null))
        return;
      this.mBrain.m_CameraCutEvent.RemoveListener(new UnityAction<CinemachineBrain>(this.OnCameraCut));
    }

    internal void PostFXHandler(CinemachineBrain brain)
    {
      ICinemachineCamera activeVirtualCamera = brain.ActiveVirtualCamera;
      if (!this.enabled || !((Object) this.mBrain != (Object) null) || !((Object) this.mPostProcessingBehaviour != (Object) null))
        return;
      CinemachinePostFX cinemachinePostFx = this.GetEffectivePostFX(activeVirtualCamera);
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
      if ((Object) this.mPostProcessingBehaviour.profile != (Object) cinemachinePostFx.m_Profile)
      {
        this.mPostProcessingBehaviour.profile = cinemachinePostFx.m_Profile;
        this.mPostProcessingBehaviour.ResetTemporalEffects();
      }
    }

    private CinemachinePostFX GetEffectivePostFX(ICinemachineCamera vcam)
    {
      while (vcam != null && vcam.LiveChildOrSelf != vcam)
        vcam = vcam.LiveChildOrSelf;
      CinemachinePostFX effectivePostFx;
      for (effectivePostFx = (CinemachinePostFX) null; vcam != null && (Object) effectivePostFx == (Object) null; vcam = vcam.ParentCamera)
      {
        CinemachineVirtualCameraBase virtualCameraBase = vcam as CinemachineVirtualCameraBase;
        if ((Object) virtualCameraBase != (Object) null)
          effectivePostFx = virtualCameraBase.GetComponent<CinemachinePostFX>();
        if ((Object) effectivePostFx != (Object) null && !effectivePostFx.enabled)
          effectivePostFx = (CinemachinePostFX) null;
      }
      return effectivePostFx;
    }

    private void OnCameraCut(CinemachineBrain brain)
    {
      if (!((Object) this.mPostProcessingBehaviour != (Object) null))
        return;
      this.mPostProcessingBehaviour.ResetTemporalEffects();
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
      CinemachineBrain.sPostProcessingHandler.RemoveListener(new UnityAction<CinemachineBrain>(CinemachinePostFX.StaticPostFXHandler));
      CinemachineBrain.sPostProcessingHandler.AddListener(new UnityAction<CinemachineBrain>(CinemachinePostFX.StaticPostFXHandler));
    }
  }
}
