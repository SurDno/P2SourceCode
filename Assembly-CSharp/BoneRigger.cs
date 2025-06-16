using System.Collections.Generic;
using UnityEngine;

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
  private AnnoBoneDeformer rtBoneDeformer = (AnnoBoneDeformer) null;

  private void Start() => this.InitializeRuntimeDeformer();

  private void InitializeRuntimeDeformer()
  {
    if (this.rtBoneDeformer != null || this.Poses.Length == 0)
      return;
    Dictionary<string, VisemeBoneDefine> _labelToBoneDict = new Dictionary<string, VisemeBoneDefine>();
    VisemeBoneDefine _basePose = (VisemeBoneDefine) null;
    foreach (VisemeBoneDefine pose in this.Poses)
    {
      _labelToBoneDict.Add(pose.Name, pose);
      if (pose.Name == this.basePose)
        _basePose = pose;
    }
    if (_basePose != null && _labelToBoneDict.Count > 0)
      this.rtBoneDeformer = new AnnoBoneDeformer(_labelToBoneDict, _basePose);
  }

  public IAnnoDeformer BoneDeformer
  {
    get
    {
      if (this.rtBoneDeformer == null)
        this.InitializeRuntimeDeformer();
      return (IAnnoDeformer) this.rtBoneDeformer;
    }
  }

  public string Selected
  {
    get
    {
      if (this.editorSelected == null || this.editorSelected.Length == 0)
        this.editorSelected = this.basePose;
      if ((this.editorSelected == null || this.editorSelected.Length == 0) && this.poseNames != null && this.poseNames.Length != 0)
        this.editorSelected = this.poseNames[0];
      return this.editorSelected;
    }
    set => this.editorSelected = value;
  }

  public int SelectedIndex
  {
    get
    {
      string selected = this.Selected;
      for (int selectedIndex = 0; this.poseNames != null && selectedIndex < this.poseNames.Length; ++selectedIndex)
      {
        if (this.poseNames[selectedIndex] == selected)
          return selectedIndex;
      }
      return -1;
    }
    set
    {
      if (value < 0 || value >= this.poseNames.Length)
        return;
      this.Selected = this.poseNames[value];
    }
  }

  public void RefreshPoseList()
  {
    VisemeBoneDefine[] visemeBoneDefineArray = new VisemeBoneDefine[this.poseNames.Length];
    for (int index = 0; index < this.poseNames.Length; ++index)
    {
      VisemeBoneDefine visemeBoneDefine = this.GetPose(this.poseNames[index]) ?? new VisemeBoneDefine(this.poseNames[index]);
      visemeBoneDefineArray[index] = visemeBoneDefine;
    }
    this.Poses = visemeBoneDefineArray;
  }

  public VisemeBoneDefine GetPose(string which)
  {
    if (this.Poses == null)
    {
      Debug.Log((object) "GetPose problem: no poses?");
      return (VisemeBoneDefine) null;
    }
    foreach (VisemeBoneDefine pose in this.Poses)
    {
      if (pose != null && pose.m_visemeLabel == which)
        return pose;
    }
    return (VisemeBoneDefine) null;
  }

  public void CommitSelected() => this.GetPose(this.Selected).RecordBonePositions(this.BoneList);

  public void ShowSelected() => this.GetPose(this.Selected).ResetToThisPose();

  public bool HasBonedPose(string which)
  {
    VisemeBoneDefine pose = this.GetPose(which);
    return pose != null && pose != null && pose.HasPose;
  }

  private void Update()
  {
  }
}
