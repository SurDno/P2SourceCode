using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class MouthRigger : MonoBehaviour
{
  public Transform[] MouthBonesList;
  public Visemes_Count_t VisemeSet = Visemes_Count_t._Please_Set_;
  public TextAsset BoneConfigFile;
  private string VisemeSelector;
  public VisemeBoneDefine[] VisemeBones;
  public PhonemeVisemeMapping phnMap;
  private bool bExpandedList;
  private bool bExpandedBoneList;
  private AnnoBoneDeformer visemeBoneDeformer;
  public BonePose[] BasePoses;
  public bool bImportFlipX;
  public float importScale = 0.01f;

  public IAnnoDeformer MouthDeformer
  {
    get
    {
      if (visemeBoneDeformer == null)
        MakeRuntimeDeformer();
      return visemeBoneDeformer;
    }
  }

  private void MakeRuntimeDeformer()
  {
    if (visemeBoneDeformer != null || phnMap == null || VisemeBones == null)
      return;
    Dictionary<string, VisemeBoneDefine> _labelToBoneDict = new Dictionary<string, VisemeBoneDefine>();
    VisemeBoneDefine _basePose = null;
    foreach (VisemeBoneDefine visemeBone in VisemeBones)
    {
      phn_string_array_t phonemes = phnMap.GetPhonemes(visemeBone.m_visemeLabel);
      if (phonemes == null)
      {
        Debug.Log("Invalid Phoneme Map: can't find " + visemeBone.m_visemeLabel);
      }
      else
      {
        foreach (string phn in phonemes.phns)
          _labelToBoneDict.Add(phn, visemeBone);
      }
      if (visemeBone.m_visemeLabel == "x")
        _basePose = visemeBone;
    }
    visemeBoneDeformer = new AnnoBoneDeformer(_labelToBoneDict, _basePose);
  }

  private void Start() => MakeRuntimeDeformer();

  public bool VisemesExpanded
  {
    get => bExpandedList;
    set => bExpandedList = value;
  }

  public bool BoneListExpanded
  {
    get => bExpandedBoneList;
    set => bExpandedBoneList = value;
  }

  private void Update()
  {
  }

  public Visemes_Count_t VisemeConfig
  {
    get => VisemeSet;
    set
    {
      if (value == VisemeSet)
        return;
      VisemeSet = value;
      InstantiatePhnToVis();
    }
  }

  public void ImportBoneConfig()
  {
    bool bKeepLocalRotations = true;
    if (!(BoneConfigFile != null))
      return;
    List<VisemeBoneDefine> visemeBoneDefineList = [];
    Hashtable cache = new Hashtable();
    XmlTextReader reader = new XmlTextReader(new StringReader(BoneConfigFile.text));
    while (reader.Read())
    {
      if (reader.Name == "mapping")
      {
        Debug.Log("Read mapping");
        phnMap = new PhonemeVisemeMapping();
        phnMap.ReadMapping(reader, "mapping");
      }
      else if (reader.Name == "scale")
      {
        Debug.Log("Read scale");
        importScale = XMLUtils.ReadXMLInnerTextFloat(reader, "scale");
      }
      else if (reader.Name == "flipx")
      {
        Debug.Log("Read flip");
        bImportFlipX = true;
      }
      else if (reader.Name == "ignore_local_rotations")
        bKeepLocalRotations = false;
      else if (reader.Name == "viseme")
      {
        Debug.Log("Read viseme");
        VisemeBoneDefine visemeBoneDefine = new VisemeBoneDefine();
        visemeBoneDefine.ReadViseme(reader, "viseme", bImportFlipX, importScale);
        visemeBoneDefine.LoadUnityBones(transform, cache, bKeepLocalRotations);
        string viseme = phnMap.PhonemeToViseme(visemeBoneDefine.m_visemeLabel);
        if (viseme.Length != 0)
          visemeBoneDefine.m_visemeLabel = viseme;
        else
          Debug.LogError("Can't find phoneme to viseme mapping for <viseme><label>" + visemeBoneDefine.m_visemeLabel + "</label> in BoneConfigFile");
        visemeBoneDefineList.Add(visemeBoneDefine);
      }
    }
    VisemeBones = new VisemeBoneDefine[visemeBoneDefineList.Count];
    for (int index = 0; index < visemeBoneDefineList.Count; ++index)
      VisemeBones[index] = visemeBoneDefineList[index];
    MouthBonesList = new Transform[cache.Count];
    BasePoses = new BonePose[cache.Count];
    int index1 = 0;
    foreach (DictionaryEntry dictionaryEntry in cache)
    {
      MouthBonesList[index1] = dictionaryEntry.Value as Transform;
      BasePoses[index1] = new BonePose();
      BasePoses[index1].InitializeBone(MouthBonesList[index1]);
      ++index1;
    }
  }

  public void ResetToBasePose()
  {
    foreach (VisemeBoneDefine visemeBone in VisemeBones)
      visemeBone.ResetBonePose(BasePoses);
  }

  public void InstantiatePhnToVis()
  {
    switch (VisemeSet)
    {
      case Visemes_Count_t._Please_Set_:
        phnMap = null;
        break;
      case Visemes_Count_t._9_Visemes:
        Visemes9 visemes9 = new Visemes9();
        phnMap = new PhonemeVisemeMapping(visemes9.visNames, visemes9.mapping);
        break;
      case Visemes_Count_t._12_Visemes:
        Visemes12 visemes12 = new Visemes12();
        phnMap = new PhonemeVisemeMapping(visemes12.visNames, visemes12.mapping);
        break;
      case Visemes_Count_t._17_Visemes:
        Visemes17 visemes17 = new Visemes17();
        phnMap = new PhonemeVisemeMapping(visemes17.visNames, visemes17.mapping);
        break;
    }
  }

  public string CurrentViseme
  {
    get => VisemeSelector;
    set => VisemeSelector = value;
  }

  public int GetPopupInfo(out string[] list)
  {
    list = null;
    if (phnMap == null)
      InstantiatePhnToVis();
    if (phnMap == null)
      return -1;
    list = phnMap.GetVisemeNames();
    if (list == null)
      return -1;
    for (int popupInfo = 0; popupInfo < list.Length; ++popupInfo)
    {
      if (list[popupInfo] == VisemeSelector)
        return popupInfo;
    }
    return 0;
  }

  public bool HasBonedViseme(string which)
  {
    if (VisemeBones == null)
      return false;
    foreach (VisemeBoneDefine visemeBone in VisemeBones)
    {
      if (visemeBone != null && visemeBone.m_visemeLabel == which && visemeBone.HasPose)
        return true;
    }
    return false;
  }

  public VisemeBoneDefine GetViseme(string which)
  {
    if (VisemeBones == null)
      VisemeBones = [];
    foreach (VisemeBoneDefine visemeBone in VisemeBones)
    {
      if (visemeBone != null && visemeBone.m_visemeLabel == which)
        return visemeBone;
    }
    int length = VisemeBones.Length;
    VisemeBoneDefine[] visemeBoneDefineArray = new VisemeBoneDefine[length + 1];
    for (int index = 0; index < length; ++index)
      visemeBoneDefineArray[index] = VisemeBones[index];
    visemeBoneDefineArray[length] = new VisemeBoneDefine(which);
    VisemeBones = visemeBoneDefineArray;
    return visemeBoneDefineArray[length];
  }

  public void CommitBonesForCurrentViseme()
  {
    GetViseme(CurrentViseme).RecordBonePositions(MouthBonesList);
  }

  public void ShowViseme(string which) => GetViseme(which).ResetToThisPose();
}
