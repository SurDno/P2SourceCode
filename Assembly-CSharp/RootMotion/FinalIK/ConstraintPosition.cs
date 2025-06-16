// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.ConstraintPosition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintPosition : Constraint
  {
    public Vector3 position;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      this.transform.position = Vector3.Lerp(this.transform.position, this.position, this.weight);
    }

    public ConstraintPosition()
    {
    }

    public ConstraintPosition(Transform transform) => this.transform = transform;
  }
}
