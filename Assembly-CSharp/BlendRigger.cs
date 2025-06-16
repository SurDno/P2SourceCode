// Decompiled with JetBrains decompiler
// Type: BlendRigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class BlendRigger : MonoBehaviour
{
  public SkinnedMeshRenderer skinnedMesh;
  public string RiggerName;
  public string[] poseNames;
  public string basePose;
  [HideInInspector]
  public VisemeBlendDefine[] Poses;
  [HideInInspector]
  public string editorSelected;
  private AnnoBlendDeformer rtBlendDeformer = (AnnoBlendDeformer) null;

  private void Start() => this.InitializeRuntimeDeformer();

  private void InitializeRuntimeDeformer()
  {
    if (this.rtBlendDeformer != null || this.Poses == null)
      return;
    if ((Object) this.skinnedMesh == (Object) null)
      this.skinnedMesh = this.GetComponent<SkinnedMeshRenderer>();
    MouthRiggerBlends.UpdateBlendIndices(this.skinnedMesh, this.Poses);
    this.rtBlendDeformer = new AnnoBlendDeformer(this.Poses, MouthRiggerBlends.GetActiveBlendIndices(this.skinnedMesh, this.Poses), this.skinnedMesh);
  }

  public IAnnoDeformer BoneDeformer
  {
    get
    {
      if (this.rtBlendDeformer == null)
        this.InitializeRuntimeDeformer();
      return (IAnnoDeformer) this.rtBlendDeformer;
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
    VisemeBlendDefine[] visemeBlendDefineArray = new VisemeBlendDefine[this.poseNames.Length];
    for (int index = 0; index < this.poseNames.Length; ++index)
    {
      VisemeBlendDefine visemeBlendDefine = this.GetPose(this.poseNames[index]) ?? new VisemeBlendDefine(this.poseNames[index]);
      visemeBlendDefineArray[index] = visemeBlendDefine;
    }
    this.Poses = visemeBlendDefineArray;
  }

  public VisemeBlendDefine GetPose(string which)
  {
    if (this.Poses == null)
    {
      Debug.Log((object) "GetPose problem: no poses?");
      return (VisemeBlendDefine) null;
    }
    foreach (VisemeBlendDefine pose in this.Poses)
    {
      if (pose != null && pose.Label == which)
        return pose;
    }
    return (VisemeBlendDefine) null;
  }

  public void CommitSelected()
  {
    MouthRiggerBlends.SaveGUIToBlendDefine(this.skinnedMesh, this.GetPose(this.Selected));
  }

  public void ShowSelected()
  {
    VisemeBlendDefine pose = this.GetPose(this.Selected);
    foreach (KeyValuePair<int, float> activeBlendIndex in MouthRiggerBlends.GetActiveBlendIndices(this.skinnedMesh, this.Poses))
      this.skinnedMesh.SetBlendShapeWeight(activeBlendIndex.Key, 0.0f);
    foreach (WeightedBlendShape theBlend in pose.theBlends)
      this.skinnedMesh.SetBlendShapeWeight(theBlend.blendIdx, theBlend.weight);
  }

  public bool HasBonedPose(string which)
  {
    VisemeBlendDefine pose = this.GetPose(which);
    return pose != null && pose != null && pose.HasPose;
  }

  private void Update()
  {
  }
}
