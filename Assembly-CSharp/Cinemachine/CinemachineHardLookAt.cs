using UnityEngine;

namespace Cinemachine
{
  [DocumentationSorting(23f, DocumentationSortingAttribute.Level.UserRef)]
  [AddComponentMenu("")]
  [RequireComponent(typeof (CinemachinePipeline))]
  [SaveDuringPlay]
  public class CinemachineHardLookAt : CinemachineComponentBase
  {
    public override bool IsValid => enabled && LookAtTarget != null;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
      if (!IsValid || !curState.HasLookAt)
        return;
      Vector3 vector3_1 = curState.ReferenceLookAt - curState.CorrectedPosition;
      if (vector3_1.magnitude > 9.9999997473787516E-05)
      {
        Vector3 vector3_2 = Vector3.Cross(vector3_1.normalized, curState.ReferenceUp);
        curState.RawOrientation = vector3_2.magnitude >= 9.9999997473787516E-05 ? Quaternion.LookRotation(vector3_1, curState.ReferenceUp) : Quaternion.FromToRotation(Vector3.forward, vector3_1);
      }
    }
  }
}
