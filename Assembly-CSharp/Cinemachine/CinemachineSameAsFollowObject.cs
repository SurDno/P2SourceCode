using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(27f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineSameAsFollowObject : CinemachineComponentBase
  {
    public override bool IsValid => this.enabled && (Object) this.FollowTarget != (Object) null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if (!this.IsValid)
        return;
      curState.RawOrientation = this.FollowTarget.transform.rotation;
    }
  }
}
