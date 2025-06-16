using System.Collections.Generic;
using UnityEngine;

public class AnnoBoneDeformer : IAnnoDeformer
{
  private Dictionary<string, VisemeBoneDefine> boneDict;
  private VisemeBoneDefine basePose;
  private BoneDeformMemento DeformMemory = null;

  public AnnoBoneDeformer(
    Dictionary<string, VisemeBoneDefine> _labelToBoneDict,
    VisemeBoneDefine _basePose)
  {
    boneDict = _labelToBoneDict;
    basePose = _basePose;
    Dictionary<VisemeBoneDefine, bool> dictionary = new Dictionary<VisemeBoneDefine, bool>();
    foreach (KeyValuePair<string, VisemeBoneDefine> keyValuePair in boneDict)
    {
      if (keyValuePair.Value != basePose && !dictionary.ContainsKey(keyValuePair.Value))
      {
        keyValuePair.Value.ConvertPosesToOffsetsFromBase(basePose);
        dictionary.Add(keyValuePair.Value, true);
      }
    }
  }

  public void Blend(string sLabel, float weight)
  {
    VisemeBoneDefine visemeBoneDefine;
    if (boneDict.TryGetValue(sLabel, out visemeBoneDefine))
    {
      if (visemeBoneDefine == basePose)
        return;
      visemeBoneDefine.Deform(weight, DeformMemory);
    }
    else
      Debug.Log("Can't find viseme " + sLabel);
  }

  public void Start()
  {
    if (DeformMemory != null)
      DeformMemory.ResetBones();
    else
      basePose.ResetToThisPose();
  }

  public void End()
  {
  }
}
