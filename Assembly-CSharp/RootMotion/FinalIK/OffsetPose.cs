﻿using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
  public class OffsetPose : MonoBehaviour
  {
    public EffectorLink[] effectorLinks = [];

    public void Apply(IKSolverFullBodyBiped solver, float weight)
    {
      for (int index = 0; index < effectorLinks.Length; ++index)
        effectorLinks[index].Apply(solver, weight, solver.GetRoot().rotation);
    }

    public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation)
    {
      for (int index = 0; index < effectorLinks.Length; ++index)
        effectorLinks[index].Apply(solver, weight, rotation);
    }

    [Serializable]
    public class EffectorLink
    {
      public FullBodyBipedEffector effector;
      public Vector3 offset;
      public Vector3 pin;
      public Vector3 pinWeight;

      public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation)
      {
        solver.GetEffector(effector).positionOffset += rotation * offset * weight;
        Vector3 vector3_1 = solver.GetRoot().position + rotation * pin - solver.GetEffector(effector).bone.position;
        Vector3 vector3_2 = pinWeight * Mathf.Abs(weight);
        solver.GetEffector(effector).positionOffset = new Vector3(Mathf.Lerp(solver.GetEffector(effector).positionOffset.x, vector3_1.x, vector3_2.x), Mathf.Lerp(solver.GetEffector(effector).positionOffset.y, vector3_1.y, vector3_2.y), Mathf.Lerp(solver.GetEffector(effector).positionOffset.z, vector3_1.z, vector3_2.z));
      }
    }
  }
}
