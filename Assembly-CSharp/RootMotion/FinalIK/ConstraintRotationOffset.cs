// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.ConstraintRotationOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintRotationOffset : Constraint
  {
    public Quaternion offset;
    private Quaternion defaultRotation;
    private Quaternion defaultLocalRotation;
    private Quaternion lastLocalRotation;
    private Quaternion defaultTargetLocalRotation;
    private bool initiated;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      if (!this.initiated)
      {
        this.defaultLocalRotation = this.transform.localRotation;
        this.lastLocalRotation = this.transform.localRotation;
        this.initiated = true;
      }
      if (this.rotationChanged)
        this.defaultLocalRotation = this.transform.localRotation;
      this.transform.localRotation = this.defaultLocalRotation;
      this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.offset, this.weight);
      this.lastLocalRotation = this.transform.localRotation;
    }

    public ConstraintRotationOffset()
    {
    }

    public ConstraintRotationOffset(Transform transform) => this.transform = transform;

    private bool rotationChanged => this.transform.localRotation != this.lastLocalRotation;
  }
}
