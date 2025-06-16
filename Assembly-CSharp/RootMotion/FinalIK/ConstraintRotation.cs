// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.ConstraintRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintRotation : Constraint
  {
    public Quaternion rotation;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.rotation, this.weight);
    }

    public ConstraintRotation()
    {
    }

    public ConstraintRotation(Transform transform) => this.transform = transform;
  }
}
