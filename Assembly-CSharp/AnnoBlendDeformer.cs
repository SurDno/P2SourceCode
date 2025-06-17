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
    skinnedMesh = _skinnedMesh;
    BlendIdxToWeight = blendIndices;
    activeBlendIndices = new int[blendIndices.Keys.Count];
    int index = 0;
    foreach (int key in blendIndices.Keys)
    {
      activeBlendIndices[index] = key;
      ++index;
    }
    _P2V = new Dictionary<string, VisemeBlendDefine>();
    foreach (VisemeBlendDefine blendPose in _blendPoses)
    {
      foreach (string thePhone in blendPose.thePhones)
        _P2V.Add(thePhone, blendPose);
    }
  }

  public void Start()
  {
    foreach (int activeBlendIndex in activeBlendIndices)
      BlendIdxToWeight[activeBlendIndex] = 0.0f;
  }

  public void Blend(string sLabel, float weight)
  {
    if (!_P2V.TryGetValue(sLabel, out VisemeBlendDefine visemeBlendDefine))
      return;
    foreach (WeightedBlendShape theBlend in visemeBlendDefine.theBlends)
      BlendIdxToWeight[theBlend.blendIdx] += weight * theBlend.weight;
  }

  public void End()
  {
    foreach (KeyValuePair<int, float> keyValuePair in BlendIdxToWeight)
      skinnedMesh.SetBlendShapeWeight(keyValuePair.Key, keyValuePair.Value);
  }
}
