// Decompiled with JetBrains decompiler
// Type: RootMotion.FinalIK.HandPoser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion.FinalIK
{
  public class HandPoser : Poser
  {
    private Transform _poseRoot;
    private Transform[] children;
    private Transform[] poseChildren;
    private Vector3[] defaultLocalPositions;
    private Quaternion[] defaultLocalRotations;

    public override void AutoMapping()
    {
      this.poseChildren = !((Object) this.poseRoot == (Object) null) ? this.poseRoot.GetComponentsInChildren<Transform>() : new Transform[0];
      this._poseRoot = this.poseRoot;
    }

    protected override void InitiatePoser()
    {
      this.children = this.GetComponentsInChildren<Transform>();
      this.StoreDefaultState();
    }

    protected override void FixPoserTransforms()
    {
      for (int index = 0; index < this.children.Length; ++index)
      {
        this.children[index].localPosition = this.defaultLocalPositions[index];
        this.children[index].localRotation = this.defaultLocalRotations[index];
      }
    }

    protected override void UpdatePoser()
    {
      if ((double) this.weight <= 0.0 || (double) this.localPositionWeight <= 0.0 && (double) this.localRotationWeight <= 0.0)
        return;
      if ((Object) this._poseRoot != (Object) this.poseRoot)
        this.AutoMapping();
      if ((Object) this.poseRoot == (Object) null)
        return;
      if (this.children.Length != this.poseChildren.Length)
      {
        Warning.Log("Number of children does not match with the pose", this.transform);
      }
      else
      {
        float t1 = this.localRotationWeight * this.weight;
        float t2 = this.localPositionWeight * this.weight;
        for (int index = 0; index < this.children.Length; ++index)
        {
          if ((Object) this.children[index] != (Object) this.transform)
          {
            this.children[index].localRotation = Quaternion.Lerp(this.children[index].localRotation, this.poseChildren[index].localRotation, t1);
            this.children[index].localPosition = Vector3.Lerp(this.children[index].localPosition, this.poseChildren[index].localPosition, t2);
          }
        }
      }
    }

    private void StoreDefaultState()
    {
      this.defaultLocalPositions = new Vector3[this.children.Length];
      this.defaultLocalRotations = new Quaternion[this.children.Length];
      for (int index = 0; index < this.children.Length; ++index)
      {
        this.defaultLocalPositions[index] = this.children[index].localPosition;
        this.defaultLocalRotations[index] = this.children[index].localRotation;
      }
    }
  }
}
