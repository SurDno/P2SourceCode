using System;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLeg : IKSolver
  {
    [Range(0.0f, 1f)]
    public float IKRotationWeight = 1f;
    public Quaternion IKRotation = Quaternion.identity;
    public Point pelvis = new Point();
    public Point thigh = new Point();
    public Point calf = new Point();
    public Point foot = new Point();
    public Point toe = new Point();
    public IKSolverVR.Leg leg = new IKSolverVR.Leg();
    public Vector3 heelOffset;
    private Vector3[] positions = new Vector3[6];
    private Quaternion[] rotations = new Quaternion[6];

    public override bool IsValid(ref string message)
    {
      if ((UnityEngine.Object) pelvis.transform == (UnityEngine.Object) null || (UnityEngine.Object) thigh.transform == (UnityEngine.Object) null || (UnityEngine.Object) calf.transform == (UnityEngine.Object) null || (UnityEngine.Object) foot.transform == (UnityEngine.Object) null || (UnityEngine.Object) toe.transform == (UnityEngine.Object) null)
      {
        message = "Please assign all bone slots of the Leg IK solver.";
        return false;
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate((UnityEngine.Object[]) new Transform[5]
      {
        pelvis.transform,
        thigh.transform,
        calf.transform,
        foot.transform,
        toe.transform
      });
      if (!((UnityEngine.Object) transform != (UnityEngine.Object) null))
        return true;
      message = transform.name + " is represented multiple times in the LegIK.";
      return false;
    }

    public bool SetChain(
      Transform pelvis,
      Transform thigh,
      Transform calf,
      Transform foot,
      Transform toe,
      Transform root)
    {
      this.pelvis.transform = pelvis;
      this.thigh.transform = thigh;
      this.calf.transform = calf;
      this.foot.transform = foot;
      this.toe.transform = toe;
      Initiate(root);
      return initiated;
    }

    public override Point[] GetPoints()
    {
      return new Point[5]
      {
        pelvis,
        thigh,
        calf,
        foot,
        toe
      };
    }

    public override Point GetPoint(Transform transform)
    {
      if ((UnityEngine.Object) pelvis.transform == (UnityEngine.Object) transform)
        return pelvis;
      if ((UnityEngine.Object) thigh.transform == (UnityEngine.Object) transform)
        return thigh;
      if ((UnityEngine.Object) calf.transform == (UnityEngine.Object) transform)
        return calf;
      if ((UnityEngine.Object) foot.transform == (UnityEngine.Object) transform)
        return foot;
      return (UnityEngine.Object) toe.transform == (UnityEngine.Object) transform ? toe : null;
    }

    public override void StoreDefaultLocalState()
    {
      thigh.StoreDefaultLocalState();
      calf.StoreDefaultLocalState();
      foot.StoreDefaultLocalState();
      toe.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!initiated)
        return;
      thigh.FixTransform();
      calf.FixTransform();
      foot.FixTransform();
      toe.FixTransform();
    }

    protected override void OnInitiate()
    {
      IKPosition = toe.transform.position;
      IKRotation = toe.transform.rotation;
      Read();
    }

    protected override void OnUpdate()
    {
      Read();
      Solve();
      Write();
    }

    private void Solve()
    {
      leg.heelPositionOffset += heelOffset;
      leg.PreSolve();
      leg.ApplyOffsets();
      leg.Solve();
      leg.ResetOffsets();
    }

    private void Read()
    {
      leg.IKPosition = IKPosition;
      leg.positionWeight = IKPositionWeight;
      leg.IKRotation = IKRotation;
      leg.rotationWeight = IKRotationWeight;
      positions[0] = root.position;
      positions[1] = pelvis.transform.position;
      positions[2] = thigh.transform.position;
      positions[3] = calf.transform.position;
      positions[4] = foot.transform.position;
      positions[5] = toe.transform.position;
      rotations[0] = root.rotation;
      rotations[1] = pelvis.transform.rotation;
      rotations[2] = thigh.transform.rotation;
      rotations[3] = calf.transform.rotation;
      rotations[4] = foot.transform.rotation;
      rotations[5] = toe.transform.rotation;
      leg.Read(positions, rotations, false, false, false, true, 1, 2);
    }

    private void Write()
    {
      leg.Write(ref positions, ref rotations);
      thigh.transform.rotation = rotations[2];
      calf.transform.rotation = rotations[3];
      foot.transform.rotation = rotations[4];
      toe.transform.rotation = rotations[5];
    }
  }
}
