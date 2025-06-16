using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(23f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineHardLockToTarget : CinemachineComponentBase
  {
    public override bool IsValid => this.enabled && (Object) this.FollowTarget != (Object) null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Body;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if (!this.IsValid)
        return;
      curState.RawPosition = this.FollowTarget.position;
    }
  }
}
