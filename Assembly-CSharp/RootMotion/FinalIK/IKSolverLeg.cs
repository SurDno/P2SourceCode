// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.IKSolverLeg
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLeg : IKSolver
  {
    [Range(0.0f, 1f)]
    public float IKRotationWeight = 1f;
    public Quaternion IKRotation = Quaternion.identity;
    public IKSolver.Point pelvis = new IKSolver.Point();
    public IKSolver.Point thigh = new IKSolver.Point();
    public IKSolver.Point calf = new IKSolver.Point();
    public IKSolver.Point foot = new IKSolver.Point();
    public IKSolver.Point toe = new IKSolver.Point();
    public IKSolverVR.Leg leg = new IKSolverVR.Leg();
    public Vector3 heelOffset;
    private Vector3[] positions = new Vector3[6];
    private Quaternion[] rotations = new Quaternion[6];

    public override bool IsValid(ref string message)
    {
      if ((UnityEngine.Object) this.pelvis.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.thigh.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.calf.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.foot.transform == (UnityEngine.Object) null || (UnityEngine.Object) this.toe.transform == (UnityEngine.Object) null)
      {
        message = "Please assign all bone slots of the Leg IK solver.";
        return false;
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate((UnityEngine.Object[]) new Transform[5]
      {
        this.pelvis.transform,
        this.thigh.transform,
        this.calf.transform,
        this.foot.transform,
        this.toe.transform
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
      this.Initiate(root);
      return this.initiated;
    }

    public override IKSolver.Point[] GetPoints()
    {
      return new IKSolver.Point[5]
      {
        this.pelvis,
        this.thigh,
        this.calf,
        this.foot,
        this.toe
      };
    }

    public override IKSolver.Point GetPoint(Transform transform)
    {
      if ((UnityEngine.Object) this.pelvis.transform == (UnityEngine.Object) transform)
        return this.pelvis;
      if ((UnityEngine.Object) this.thigh.transform == (UnityEngine.Object) transform)
        return this.thigh;
      if ((UnityEngine.Object) this.calf.transform == (UnityEngine.Object) transform)
        return this.calf;
      if ((UnityEngine.Object) this.foot.transform == (UnityEngine.Object) transform)
        return this.foot;
      return (UnityEngine.Object) this.toe.transform == (UnityEngine.Object) transform ? this.toe : (IKSolver.Point) null;
    }

    public override void StoreDefaultLocalState()
    {
      this.thigh.StoreDefaultLocalState();
      this.calf.StoreDefaultLocalState();
      this.foot.StoreDefaultLocalState();
      this.toe.StoreDefaultLocalState();
    }

    public override void FixTransforms()
    {
      if (!this.initiated)
        return;
      this.thigh.FixTransform();
      this.calf.FixTransform();
      this.foot.FixTransform();
      this.toe.FixTransform();
    }

    protected override void OnInitiate()
    {
      this.IKPosition = this.toe.transform.position;
      this.IKRotation = this.toe.transform.rotation;
      this.Read();
    }

    protected override void OnUpdate()
    {
      this.Read();
      this.Solve();
      this.Write();
    }

    private void Solve()
    {
      this.leg.heelPositionOffset += this.heelOffset;
      this.leg.PreSolve();
      this.leg.ApplyOffsets();
      this.leg.Solve();
      this.leg.ResetOffsets();
    }

    private void Read()
    {
      this.leg.IKPosition = this.IKPosition;
      this.leg.positionWeight = this.IKPositionWeight;
      this.leg.IKRotation = this.IKRotation;
      this.leg.rotationWeight = this.IKRotationWeight;
      this.positions[0] = this.root.position;
      this.positions[1] = this.pelvis.transform.position;
      this.positions[2] = this.thigh.transform.position;
      this.positions[3] = this.calf.transform.position;
      this.positions[4] = this.foot.transform.position;
      this.positions[5] = this.toe.transform.position;
      this.rotations[0] = this.root.rotation;
      this.rotations[1] = this.pelvis.transform.rotation;
      this.rotations[2] = this.thigh.transform.rotation;
      this.rotations[3] = this.calf.transform.rotation;
      this.rotations[4] = this.foot.transform.rotation;
      this.rotations[5] = this.toe.transform.rotation;
      this.leg.Read(this.positions, this.rotations, false, false, false, true, 1, 2);
    }

    private void Write()
    {
      this.leg.Write(ref this.positions, ref this.rotations);
      this.thigh.transform.rotation = this.rotations[2];
      this.calf.transform.rotation = this.rotations[3];
      this.foot.transform.rotation = this.rotations[4];
      this.toe.transform.rotation = this.rotations[5];
    }
  }
}
