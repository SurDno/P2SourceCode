using System.Collections.Generic;

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
    PoseList = new BonePose[bonePoseList.Count];
    int index = 0;
    foreach (BonePose bonePose in bonePoseList)
    {
      PoseList[index] = bonePose;
      ++index;
    }
  }

  public void ShowBones()
  {
    if (PoseList == null)
      return;
    foreach (BonePose pose in PoseList)
      pose.ResetToThisTransform();
  }
}
