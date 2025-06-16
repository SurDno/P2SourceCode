// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.ConstraintPositionOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  [Serializable]
  public class ConstraintPositionOffset : Constraint
  {
    public Vector3 offset;
    private Vector3 defaultLocalPosition;
    private Vector3 lastLocalPosition;
    private bool initiated;

    public override void UpdateConstraint()
    {
      if ((double) this.weight <= 0.0 || !this.isValid)
        return;
      if (!this.initiated)
      {
        this.defaultLocalPosition = this.transform.localPosition;
        this.lastLocalPosition = this.transform.localPosition;
        this.initiated = true;
      }
      if (this.positionChanged)
        this.defaultLocalPosition = this.transform.localPosition;
      this.transform.localPosition = this.defaultLocalPosition;
      this.transform.position += this.offset * this.weight;
      this.lastLocalPosition = this.transform.localPosition;
    }

    public ConstraintPositionOffset()
    {
    }

    public ConstraintPositionOffset(Transform transform) => this.transform = transform;

    private bool positionChanged => this.transform.localPosition != this.lastLocalPosition;
  }
}
