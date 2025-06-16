using System.Collections.Generic;
using UnityEngine;

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
