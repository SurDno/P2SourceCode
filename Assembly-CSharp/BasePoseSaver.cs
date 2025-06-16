// Decompiled with JetBrains decompiler
// Type: BasePoseSaver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BasePoseSaver : MonoBehaviour
{
  public BonePose[] PoseList;

  public void CommitBones()
  {
    Transform[] componentsInChildren = this.gameObject.GetComponentsInChildren<Transform>();
    List<BonePose> bonePoseList = new List<BonePose>();
    foreach (Transform bx in componentsInChildren)
    {
      if ((Object) bx != (Object) this.transform)
      {
        BonePose bonePose = new BonePose();
        bonePose.InitializeBone(bx);
        bonePoseList.Add(bonePose);
      }
    }
    this.PoseList = new BonePose[bonePoseList.Count];
    int index = 0;
    foreach (BonePose bonePose in bonePoseList)
    {
      this.PoseList[index] = bonePose;
      ++index;
    }
  }

  public void ShowBones()
  {
    if (this.PoseList == null)
      return;
    foreach (BonePose pose in this.PoseList)
      pose.ResetToThisTransform();
  }
}
