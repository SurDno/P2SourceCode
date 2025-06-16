using System.Collections.Generic;
using UnityEngine;

public class AnnoBlendDeformer : IAnnoDeformer
{
  private SkinnedMeshRenderer skinnedMesh;
  private Dictionary<int, float> BlendIdxToWeight;
  private int[] activeBlendIndices;
  private Dictionary<string, VisemeBlendDefine> _P2V;

  public AnnoBlendDeformer(
    VisemeBlendDefine[] _blendPoses,
    Dictionary<int, float> blendIndices,
    SkinnedMeshRenderer _skinnedMesh)
  {
    this.skinnedMesh = _skinnedMesh;
    this.BlendIdxToWeight = blendIndices;
    this.activeBlendIndices = new int[blendIndices.Keys.Count];
    int index = 0;
    foreach (int key in blendIndices.Keys)
    {
      this.activeBlendIndices[index] = key;
      ++index;
    }
    this._P2V = new Dictionary<string, VisemeBlendDefine>();
    foreach (VisemeBlendDefine blendPose in _blendPoses)
    {
      foreach (string thePhone in blendPose.thePhones)
        this._P2V.Add(thePhone, blendPose);
    }
  }

  public void Start()
  {
    foreach (int activeBlendIndex in this.activeBlendIndices)
      this.BlendIdxToWeight[activeBlendIndex] = 0.0f;
  }

  public void Blend(string sLabel, float weight)
  {
    VisemeBlendDefine visemeBlendDefine;
    if (!this._P2V.TryGetValue(sLabel, out visemeBlendDefine))
      return;
    foreach (WeightedBlendShape theBlend in visemeBlendDefine.theBlends)
      this.BlendIdxToWeight[theBlend.blendIdx] += weight * theBlend.weight;
  }

  public void End()
  {
    foreach (KeyValuePair<int, float> keyValuePair in this.BlendIdxToWeight)
      this.skinnedMesh.SetBlendShapeWeight(keyValuePair.Key, keyValuePair.Value);
  }
}
