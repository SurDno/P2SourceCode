using System.Collections.Generic;
using UnityEngine;

public class BoneDeformMemento
{
  private List<DeformData> BoneDeformation = new List<DeformData>();

  public void AccumulateDeform(
    Transform theBone,
    float tx,
    float ty,
    float tz,
    float rx,
    float ry,
    float rz)
  {
    DeformData bone = FindBone(theBone);
    bone.tx += tx;
    bone.ty += ty;
    bone.tz += tz;
    bone.rx += rx;
    bone.ry += ry;
    bone.rz += rz;
  }

  public void ResetBones()
  {
    foreach (DeformData deformData in BoneDeformation)
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

  private DeformData FindBone(Transform theBone)
  {
    foreach (DeformData bone in BoneDeformation)
    {
      if (bone.bone == theBone)
        return bone;
    }
    DeformData bone1 = new DeformData();
    bone1.bone = theBone;
    BoneDeformation.Add(bone1);
    return bone1;
  }

  public class DeformData
  {
    public Transform bone;
    public float tx;
    public float ty;
    public float tz;
    public float rx;
    public float ry;
    public float rz;
  }
}
