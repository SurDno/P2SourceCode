using System.Collections.Generic;
using UnityEngine;

public class AnnoBoneDeformer : IAnnoDeformer
{
  private Dictionary<string, VisemeBoneDefine> boneDict;
  private VisemeBoneDefine basePose;
  private BoneDeformMemento DeformMemory = (BoneDeformMemento) null;

  public AnnoBoneDeformer(
    Dictionary<string, VisemeBoneDefine> _labelToBoneDict,
    VisemeBoneDefine _basePose)
  {
    this.boneDict = _labelToBoneDict;
    this.basePose = _basePose;
    Dictionary<VisemeBoneDefine, bool> dictionary = new Dictionary<VisemeBoneDefine, bool>();
    foreach (KeyValuePair<string, VisemeBoneDefine> keyValuePair in this.boneDict)
    {
      if (keyValuePair.Value != this.basePose && !dictionary.ContainsKey(keyValuePair.Value))
      {
        keyValuePair.Value.ConvertPosesToOffsetsFromBase(this.basePose);
        dictionary.Add(keyValuePair.Value, true);
      }
    }
  }

  public void Blend(string sLabel, float weight)
  {
    VisemeBoneDefine visemeBoneDefine;
    if (this.boneDict.TryGetValue(sLabel, out visemeBoneDefine))
    {
      if (visemeBoneDefine == this.basePose)
        return;
      visemeBoneDefine.Deform(weight, this.DeformMemory);
    }
    else
      Debug.Log((object) ("Can't find viseme " + sLabel));
  }

  public void Start()
  {
    if (this.DeformMemory != null)
      this.DeformMemory.ResetBones();
    else
      this.basePose.ResetToThisPose();
  }

  public void End()
  {
  }
}
