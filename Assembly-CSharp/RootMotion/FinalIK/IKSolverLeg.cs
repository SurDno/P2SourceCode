﻿using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class IKSolverLeg : IKSolver
  {
    [Range(0.0f, 1f)]
    public float IKRotationWeight = 1f;
    public Quaternion IKRotation = Quaternion.identity;
    public Point pelvis = new();
    public Point thigh = new();
    public Point calf = new();
    public Point foot = new();
    public Point toe = new();
    public IKSolverVR.Leg leg = new();
    public Vector3 heelOffset;
    private Vector3[] positions = new Vector3[6];
    private Quaternion[] rotations = new Quaternion[6];

    public override bool IsValid(ref string message)
    {
      if (pelvis.transform == null || thigh.transform == null || calf.transform == null || foot.transform == null || toe.transform == null)
      {
        message = "Please assign all bone slots of the Leg IK solver.";
        return false;
      }
      Transform transform = (Transform) Hierarchy.ContainsDuplicate(new Transform[5]
      {
        pelvis.transform,
        thigh.transform,
        calf.transform,
        foot.transform,
        toe.transform
      });
      if (!(transform != null))
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
      return [
        pelvis,
        thigh,
        calf,
        foot,
        toe
      ];
    }

    public override Point GetPoint(Transform transform)
    {
      if (pelvis.transform == transform)
        return pelvis;
      if (thigh.transform == transform)
        return thigh;
      if (calf.transform == transform)
        return calf;
      if (foot.transform == transform)
        return foot;
      return toe.transform == transform ? toe : null;
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
