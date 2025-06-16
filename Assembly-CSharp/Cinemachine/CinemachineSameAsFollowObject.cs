namespace Cinemachine
{
  [DocumentationSorting(27f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineSameAsFollowObject : CinemachineComponentBase
  {
    public override bool IsValid => this.enabled && (Object) FollowTarget != (Object) null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if (!IsValid)
        return;
      curState.RawOrientation = FollowTarget.transform.rotation;
    }
  }
}
