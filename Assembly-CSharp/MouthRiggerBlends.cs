// Decompiled with JetBrains decompiler
// Type: MouthRiggerBlends
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MouthRiggerBlends : MonoBehaviour
{
  public SkinnedMeshRenderer skinnedMesh;
  public Visemes_Count_t VisemeSet = Visemes_Count_t._Please_Set_;
  private string VisemeSelector;
  public VisemeBlendDefine[] VisemeBlends = (VisemeBlendDefine[]) null;
  public PhonemeVisemeMapping phnMap = (PhonemeVisemeMapping) null;
  private bool bExpandedList = false;
  private bool bExpandedBoneList = false;
  private AnnoBlendDeformer visemeBlendDeformer;
  public BonePose[] BasePoses = (BonePose[]) null;

  public IAnnoDeformer MouthDeformer
  {
    get
    {
      if (this.visemeBlendDeformer == null)
        this.MakeRuntimeDeformer();
      return (IAnnoDeformer) this.visemeBlendDeformer;
    }
  }

  private void MakeRuntimeDeformer()
  {
    if (this.visemeBlendDeformer != null || this.phnMap == null || this.VisemeBlends == null)
      return;
    if ((UnityEngine.Object) this.skinnedMesh == (UnityEngine.Object) null)
      this.skinnedMesh = this.GetComponent<SkinnedMeshRenderer>();
    MouthRiggerBlends.UpdateBlendIndices(this.skinnedMesh, this.VisemeBlends);
    this.visemeBlendDeformer = new AnnoBlendDeformer(this.VisemeBlends, MouthRiggerBlends.GetActiveBlendIndices(this.skinnedMesh, this.VisemeBlends), this.skinnedMesh);
  }

  private void Start() => this.MakeRuntimeDeformer();

  public bool VisemesExpanded
  {
    get => this.bExpandedList;
    set => this.bExpandedList = value;
  }

  public bool BoneListExpanded
  {
    get => this.bExpandedBoneList;
    set => this.bExpandedBoneList = value;
  }

  private void Update()
  {
  }

  public Visemes_Count_t VisemeConfig
  {
    get => this.VisemeSet;
    set
    {
      if (value == this.VisemeSet)
        return;
      this.VisemeSet = value;
      this.InstantiatePhnToVis();
    }
  }

  public void InstantiatePhnToVis()
  {
    switch (this.VisemeSet)
    {
      case Visemes_Count_t._Please_Set_:
        this.phnMap = (PhonemeVisemeMapping) null;
        break;
      case Visemes_Count_t._9_Visemes:
        Visemes9 visemes9 = new Visemes9();
        this.phnMap = new PhonemeVisemeMapping(visemes9.visNames, visemes9.mapping);
        break;
      case Visemes_Count_t._12_Visemes:
        Visemes12 visemes12 = new Visemes12();
        this.phnMap = new PhonemeVisemeMapping(visemes12.visNames, visemes12.mapping);
        break;
      case Visemes_Count_t._17_Visemes:
        Visemes17 visemes17 = new Visemes17();
        this.phnMap = new PhonemeVisemeMapping(visemes17.visNames, visemes17.mapping);
        break;
    }
  }

  public string CurrentViseme
  {
    get => this.VisemeSelector;
    set => this.VisemeSelector = value;
  }

  public int GetPopupInfo(out string[] list)
  {
    list = (string[]) null;
    if (this.phnMap == null)
      this.InstantiatePhnToVis();
    if (this.phnMap == null)
      return -1;
    list = this.phnMap.GetVisemeNames();
    if (list == null)
      return -1;
    for (int popupInfo = 0; popupInfo < list.Length; ++popupInfo)
    {
      if (list[popupInfo] == this.VisemeSelector)
        return popupInfo;
    }
    return 0;
  }

  public VisemeBlendDefine GetViseme(string which)
  {
    if (this.VisemeBlends == null)
      this.VisemeBlends = new VisemeBlendDefine[0];
    foreach (VisemeBlendDefine visemeBlend in this.VisemeBlends)
    {
      foreach (string thePhone in visemeBlend.thePhones)
      {
        if (string.Compare(thePhone, which, StringComparison.OrdinalIgnoreCase) == 0)
          return visemeBlend;
      }
    }
    int length = this.VisemeBlends.Length;
    VisemeBlendDefine[] visemeBlendDefineArray = new VisemeBlendDefine[length + 1];
    for (int index = 0; index < length; ++index)
      visemeBlendDefineArray[index] = this.VisemeBlends[index];
    phn_string_array_t phonemes = this.phnMap.GetPhonemes(which);
    visemeBlendDefineArray[length] = new VisemeBlendDefine(phonemes.phns);
    this.VisemeBlends = visemeBlendDefineArray;
    return visemeBlendDefineArray[length];
  }

  public static void SaveGUIToBlendDefine(SkinnedMeshRenderer obj, VisemeBlendDefine bn)
  {
    List<WeightedBlendShape> weightedBlendShapeList = new List<WeightedBlendShape>();
    int blendShapeCount = obj.sharedMesh.blendShapeCount;
    for (int index = 0; index < blendShapeCount; ++index)
    {
      float blendShapeWeight = obj.GetBlendShapeWeight(index);
      if ((double) blendShapeWeight > 0.001)
      {
        string blendShapeName = obj.sharedMesh.GetBlendShapeName(index);
        if (blendShapeName != null)
          weightedBlendShapeList.Add(new WeightedBlendShape(blendShapeName, blendShapeWeight, index));
      }
    }
    bn.theBlends = new WeightedBlendShape[weightedBlendShapeList.Count];
    for (int index = 0; index < weightedBlendShapeList.Count; ++index)
      bn.theBlends[index] = weightedBlendShapeList[index];
  }

  public void CommitBonesForCurrentViseme()
  {
    MouthRiggerBlends.SaveGUIToBlendDefine(this.skinnedMesh, this.GetViseme(this.CurrentViseme));
  }

  public void DebugPrint()
  {
    string message = "";
    if (this.VisemeBlends == null)
      return;
    foreach (VisemeBlendDefine visemeBlend in this.VisemeBlends)
    {
      foreach (string thePhone in visemeBlend.thePhones)
        message = message + thePhone + " ";
      foreach (WeightedBlendShape theBlend in visemeBlend.theBlends)
        message = message + theBlend.blendName + "," + (object) theBlend.weight;
      message += "\n";
    }
    Debug.Log((object) message);
  }

  private static int BlendNameToBlendIndex(SkinnedMeshRenderer obj, string blendName)
  {
    int blendShapeCount = obj.sharedMesh.blendShapeCount;
    for (int shapeIndex = 0; shapeIndex < blendShapeCount; ++shapeIndex)
    {
      if (obj.sharedMesh.GetBlendShapeName(shapeIndex) == blendName)
        return shapeIndex;
    }
    Debug.Log((object) ("Missing Blend Shape for blend " + blendName));
    return -1;
  }

  public static void UpdateBlendIndices(SkinnedMeshRenderer obj, VisemeBlendDefine[] blends)
  {
    foreach (VisemeBlendDefine blend in blends)
    {
      foreach (WeightedBlendShape theBlend in blend.theBlends)
      {
        int blendIndex = MouthRiggerBlends.BlendNameToBlendIndex(obj, theBlend.blendName);
        theBlend.blendIdx = blendIndex;
      }
    }
  }

  public static Dictionary<int, float> GetActiveBlendIndices(
    SkinnedMeshRenderer obj,
    VisemeBlendDefine[] blends)
  {
    MouthRiggerBlends.UpdateBlendIndices(obj, blends);
    Dictionary<int, float> activeBlendIndices = new Dictionary<int, float>();
    foreach (VisemeBlendDefine blend in blends)
    {
      foreach (WeightedBlendShape theBlend in blend.theBlends)
      {
        if (!activeBlendIndices.ContainsKey(theBlend.blendIdx))
          activeBlendIndices.Add(theBlend.blendIdx, 0.0f);
      }
    }
    return activeBlendIndices;
  }

  public void ShowViseme(string which)
  {
    VisemeBlendDefine viseme = this.GetViseme(which);
    Dictionary<int, float> activeBlendIndices = MouthRiggerBlends.GetActiveBlendIndices(this.skinnedMesh, this.VisemeBlends);
    foreach (WeightedBlendShape theBlend in viseme.theBlends)
      activeBlendIndices[theBlend.blendIdx] += theBlend.weight;
    foreach (KeyValuePair<int, float> keyValuePair in activeBlendIndices)
      this.skinnedMesh.SetBlendShapeWeight(keyValuePair.Key, keyValuePair.Value);
  }
}
