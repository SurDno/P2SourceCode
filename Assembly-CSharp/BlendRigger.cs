using System.Collections.Generic;

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
  private AnnoBlendDeformer rtBlendDeformer = null;

  private void Start() => InitializeRuntimeDeformer();

  private void InitializeRuntimeDeformer()
  {
    if (rtBlendDeformer != null || Poses == null)
      return;
    if ((Object) skinnedMesh == (Object) null)
      skinnedMesh = this.GetComponent<SkinnedMeshRenderer>();
    MouthRiggerBlends.UpdateBlendIndices(skinnedMesh, Poses);
    rtBlendDeformer = new AnnoBlendDeformer(Poses, MouthRiggerBlends.GetActiveBlendIndices(skinnedMesh, Poses), skinnedMesh);
  }

  public IAnnoDeformer BoneDeformer
  {
    get
    {
      if (rtBlendDeformer == null)
        InitializeRuntimeDeformer();
      return rtBlendDeformer;
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
    VisemeBlendDefine[] visemeBlendDefineArray = new VisemeBlendDefine[poseNames.Length];
    for (int index = 0; index < poseNames.Length; ++index)
    {
      VisemeBlendDefine visemeBlendDefine = GetPose(poseNames[index]) ?? new VisemeBlendDefine(poseNames[index]);
      visemeBlendDefineArray[index] = visemeBlendDefine;
    }
    Poses = visemeBlendDefineArray;
  }

  public VisemeBlendDefine GetPose(string which)
  {
    if (Poses == null)
    {
      Debug.Log((object) "GetPose problem: no poses?");
      return null;
    }
    foreach (VisemeBlendDefine pose in Poses)
    {
      if (pose != null && pose.Label == which)
        return pose;
    }
    return null;
  }

  public void CommitSelected()
  {
    MouthRiggerBlends.SaveGUIToBlendDefine(skinnedMesh, GetPose(Selected));
  }

  public void ShowSelected()
  {
    VisemeBlendDefine pose = GetPose(Selected);
    foreach (KeyValuePair<int, float> activeBlendIndex in MouthRiggerBlends.GetActiveBlendIndices(skinnedMesh, Poses))
      skinnedMesh.SetBlendShapeWeight(activeBlendIndex.Key, 0.0f);
    foreach (WeightedBlendShape theBlend in pose.theBlends)
      skinnedMesh.SetBlendShapeWeight(theBlend.blendIdx, theBlend.weight);
  }

  public bool HasBonedPose(string which)
  {
    VisemeBlendDefine pose = GetPose(which);
    return pose != null && pose != null && pose.HasPose;
  }

  private void Update()
  {
  }
}
