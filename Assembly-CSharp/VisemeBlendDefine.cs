using System;
using UnityEngine;

[Serializable]
public class VisemeBlendDefine
{
  public WeightedBlendShape[] theBlends;
  public string[] thePhones;

  public VisemeBlendDefine()
  {
  }

  public VisemeBlendDefine(string poseName)
  {
    this.thePhones = new string[1];
    this.thePhones[0] = poseName.ToString();
    this.theBlends = new WeightedBlendShape[0];
  }

  public string Label => this.thePhones[0];

  public bool HasPose => this.theBlends != null;

  public float GetWeight(string blendName)
  {
    if (this.theBlends == null)
      return 0.0f;
    foreach (WeightedBlendShape theBlend in this.theBlends)
    {
      if (string.Compare(theBlend.blendName, blendName, false) == 0)
        return theBlend.weight;
    }
    return 0.0f;
  }

  public bool DetectChanges(SkinnedMeshRenderer obj)
  {
    int blendShapeCount = obj.sharedMesh.blendShapeCount;
    bool flag = false;
    for (int index = 0; index < blendShapeCount; ++index)
    {
      string blendShapeName = obj.sharedMesh.GetBlendShapeName(index);
      float blendShapeWeight = obj.GetBlendShapeWeight(index);
      if ((double) this.GetWeight(blendShapeName) != (double) blendShapeWeight)
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  public VisemeBlendDefine(string[] phns)
  {
    this.thePhones = phns;
    this.theBlends = new WeightedBlendShape[0];
  }

  public VisemeBlendDefine(string[] phns, string blendShape1, float pct1)
  {
    this.thePhones = phns;
    this.theBlends = new WeightedBlendShape[1];
    this.theBlends[0] = new WeightedBlendShape(blendShape1, pct1);
  }

  public VisemeBlendDefine(
    string[] phns,
    string blendShape1,
    float pct1,
    string blendShape2,
    float pct2)
  {
    this.thePhones = phns;
    this.theBlends = new WeightedBlendShape[2];
    this.theBlends[0] = new WeightedBlendShape(blendShape1, pct1);
    this.theBlends[1] = new WeightedBlendShape(blendShape2, pct2);
  }

  public VisemeBlendDefine(
    string[] phns,
    string blendShape1,
    float pct1,
    string blendShape2,
    float pct2,
    string blendShape3,
    float pct3)
  {
    this.thePhones = phns;
    this.theBlends = new WeightedBlendShape[3];
    this.theBlends[0] = new WeightedBlendShape(blendShape1, pct1);
    this.theBlends[1] = new WeightedBlendShape(blendShape2, pct2);
    this.theBlends[2] = new WeightedBlendShape(blendShape3, pct3);
  }

  public VisemeBlendDefine(
    string[] phns,
    string blendShape1,
    float pct1,
    string blendShape2,
    float pct2,
    string blendShape3,
    float pct3,
    string blendShape4,
    float pct4)
  {
    this.thePhones = phns;
    this.theBlends = new WeightedBlendShape[4];
    this.theBlends[0] = new WeightedBlendShape(blendShape1, pct1);
    this.theBlends[1] = new WeightedBlendShape(blendShape2, pct2);
    this.theBlends[2] = new WeightedBlendShape(blendShape3, pct3);
    this.theBlends[3] = new WeightedBlendShape(blendShape4, pct4);
  }
}
