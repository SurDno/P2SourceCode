// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.OffsetPose
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class OffsetPose : MonoBehaviour
  {
    public OffsetPose.EffectorLink[] effectorLinks = new OffsetPose.EffectorLink[0];

    public void Apply(IKSolverFullBodyBiped solver, float weight)
    {
      for (int index = 0; index < this.effectorLinks.Length; ++index)
        this.effectorLinks[index].Apply(solver, weight, solver.GetRoot().rotation);
    }

    public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation)
    {
      for (int index = 0; index < this.effectorLinks.Length; ++index)
        this.effectorLinks[index].Apply(solver, weight, rotation);
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
        solver.GetEffector(this.effector).positionOffset += rotation * this.offset * weight;
        Vector3 vector3_1 = solver.GetRoot().position + rotation * this.pin - solver.GetEffector(this.effector).bone.position;
        Vector3 vector3_2 = this.pinWeight * Mathf.Abs(weight);
        solver.GetEffector(this.effector).positionOffset = new Vector3(Mathf.Lerp(solver.GetEffector(this.effector).positionOffset.x, vector3_1.x, vector3_2.x), Mathf.Lerp(solver.GetEffector(this.effector).positionOffset.y, vector3_1.y, vector3_2.y), Mathf.Lerp(solver.GetEffector(this.effector).positionOffset.z, vector3_1.z, vector3_2.z));
      }
    }
  }
}
