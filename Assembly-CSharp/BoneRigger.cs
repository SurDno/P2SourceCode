using System.Collections.Generic;

public class BoneRigger : MonoBehaviour
{
  public string RiggerName;
  public string[] poseNames;
  public string basePose;
  [HideInInspector]
  public Transform[] BoneList;
  [HideInInspector]
  public VisemeBoneDefine[] Poses;
  [HideInInspector]
  public string editorSelected;
  private AnnoBoneDeformer rtBoneDeformer = null;

  private void Start() => InitializeRuntimeDeformer();

  private void InitializeRuntimeDeformer()
  {
    if (rtBoneDeformer != null || Poses.Length == 0)
      return;
    Dictionary<string, VisemeBoneDefine> _labelToBoneDict = new Dictionary<string, VisemeBoneDefine>();
    VisemeBoneDefine _basePose = null;
    foreach (VisemeBoneDefine pose in Poses)
    {
      _labelToBoneDict.Add(pose.Name, pose);
      if (pose.Name == basePose)
        _basePose = pose;
    }
    if (_basePose != null && _labelToBoneDict.Count > 0)
      rtBoneDeformer = new AnnoBoneDeformer(_labelToBoneDict, _basePose);
  }

  public IAnnoDeformer BoneDeformer
  {
    get
    {
      if (rtBoneDeformer == null)
        InitializeRuntimeDeformer();
      return rtBoneDeformer;
    }
  }

  public string Selected
  {
    get
    {
      if (editorSelected == null || editorSelected.Length == 0)
        editorSelected = basePose;
      if ((editorSelected == null || editorSelected.Length == 0) && poseNames != null && poseNames.Length != 0)
        editorSelected = poseNames[0];
      return editorSelected;
    }
    set => editorSelected = value;
  }

  public int SelectedIndex
  {
    get
    {
      string selected = Selected;
      for (int selectedIndex = 0; poseNames != null && selectedIndex < poseNames.Length; ++selectedIndex)
      {
        if (poseNames[selectedIndex] == selected)
          return selectedIndex;
      }
      return -1;
    }
    set
    {
      if (value < 0 || value >= poseNames.Length)
        return;
      Selected = poseNames[value];
    }
  }

  public void RefreshPoseList()
  {
    VisemeBoneDefine[] visemeBoneDefineArray = new VisemeBoneDefine[poseNames.Length];
    for (int index = 0; index < poseNames.Length; ++index)
    {
      VisemeBoneDefine visemeBoneDefine = GetPose(poseNames[index]) ?? new VisemeBoneDefine(poseNames[index]);
      visemeBoneDefineArray[index] = visemeBoneDefine;
    }
    Poses = visemeBoneDefineArray;
  }

  public VisemeBoneDefine GetPose(string which)
  {
    if (Poses == null)
    {
      Debug.Log((object) "GetPose problem: no poses?");
      return null;
    }
    foreach (VisemeBoneDefine pose in Poses)
    {
      if (pose != null && pose.m_visemeLabel == which)
        return pose;
    }
    return null;
  }

  public void CommitSelected() => GetPose(Selected).RecordBonePositions(BoneList);

  public void ShowSelected() => GetPose(Selected).ResetToThisPose();

  public bool HasBonedPose(string which)
  {
    VisemeBoneDefine pose = GetPose(which);
    return pose != null && pose != null && pose.HasPose;
  }

  private void Update()
  {
  }
}
