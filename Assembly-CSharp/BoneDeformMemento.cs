// Decompiled with JetBrains decompiler
// Type: BoneDeformMemento
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BoneDeformMemento
{
  private List<BoneDeformMemento.DeformData> BoneDeformation = new List<BoneDeformMemento.DeformData>();

  public void AccumulateDeform(
    Transform theBone,
    float tx,
    float ty,
    float tz,
    float rx,
    float ry,
    float rz)
  {
    BoneDeformMemento.DeformData bone = this.FindBone(theBone);
    bone.tx += tx;
    bone.ty += ty;
    bone.tz += tz;
    bone.rx += rx;
    bone.ry += ry;
    bone.rz += rz;
  }

  public void ResetBones()
  {
    foreach (BoneDeformMemento.DeformData deformData in this.BoneDeformation)
    {
      Vector3 localPosition = deformData.bone.localPosition;
      localPosition.x -= deformData.tx;
      localPosition.y -= deformData.ty;
      localPosition.z -= deformData.tz;
      deformData.bone.localPosition = localPosition;
      Vector3 localEulerAngles = deformData.bone.localEulerAngles;
      localEulerAngles.x -= deformData.rx;
      localEulerAngles.y -= deformData.ry;
      localEulerAngles.z -= deformData.rz;
      deformData.bone.localEulerAngles = localEulerAngles;
      deformData.tx = deformData.ty = deformData.tz = deformData.rx = deformData.ry = deformData.rz = 0.0f;
    }
  }

  private BoneDeformMemento.DeformData FindBone(Transform theBone)
  {
    foreach (BoneDeformMemento.DeformData bone in this.BoneDeformation)
    {
      if ((Object) bone.bone == (Object) theBone)
        return bone;
    }
    BoneDeformMemento.DeformData bone1 = new BoneDeformMemento.DeformData();
    bone1.bone = theBone;
    this.BoneDeformation.Add(bone1);
    return bone1;
  }

  public class DeformData
  {
    public Transform bone = (Transform) null;
    public float tx = 0.0f;
    public float ty = 0.0f;
    public float tz = 0.0f;
    public float rx = 0.0f;
    public float ry = 0.0f;
    public float rz = 0.0f;
  }
}
