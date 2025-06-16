// Decompiled with JetBrains decompiler
// Type: JerboaAnimationInstancing.RuntimeHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

#nullable disable
namespace JerboaAnimationInstancing
{
  public class RuntimeHelper
  {
    public static Transform[] MergeBone(SkinnedMeshRenderer[] meshRender, List<Matrix4x4> bindPose)
    {
      if (Profiler.enabled)
        Profiler.BeginSample("MergeBone()");
      List<Transform> transformList = new List<Transform>(150);
      for (int index1 = 0; index1 != meshRender.Length; ++index1)
      {
        Transform[] bones = meshRender[index1].bones;
        Matrix4x4[] bindposes = meshRender[index1].sharedMesh.bindposes;
        for (int j = 0; j != bones.Length; ++j)
        {
          int index2 = transformList.FindIndex((Predicate<Transform>) (q => (UnityEngine.Object) q == (UnityEngine.Object) bones[j]));
          if (index2 < 0)
          {
            transformList.Add(bones[j]);
            bindPose?.Add(bindposes[j]);
          }
          else
            bindPose[index2] = bindposes[j];
        }
        meshRender[index1].enabled = false;
      }
      if (Profiler.enabled)
        Profiler.EndSample();
      return transformList.ToArray();
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 mat)
    {
      Vector3 forward;
      forward.x = mat.m02;
      forward.y = mat.m12;
      forward.z = mat.m22;
      Vector3 upwards;
      upwards.x = mat.m01;
      upwards.y = mat.m11;
      upwards.z = mat.m21;
      return Quaternion.LookRotation(forward, upwards);
    }
  }
}
