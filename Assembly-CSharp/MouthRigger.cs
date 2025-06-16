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
  public VisemeBoneDefine[] VisemeBones = (VisemeBoneDefine[]) null;
  public PhonemeVisemeMapping phnMap = (PhonemeVisemeMapping) null;
  private bool bExpandedList = false;
  private bool bExpandedBoneList = false;
  private AnnoBoneDeformer visemeBoneDeformer;
  public BonePose[] BasePoses = (BonePose[]) null;
  public bool bImportFlipX = false;
  public float importScale = 0.01f;

  public IAnnoDeformer MouthDeformer
  {
    get
    {
      if (this.visemeBoneDeformer == null)
        this.MakeRuntimeDeformer();
      return (IAnnoDeformer) this.visemeBoneDeformer;
    }
  }

  private void MakeRuntimeDeformer()
  {
    if (this.visemeBoneDeformer != null || this.phnMap == null || this.VisemeBones == null)
      return;
    Dictionary<string, VisemeBoneDefine> _labelToBoneDict = new Dictionary<string, VisemeBoneDefine>();
    VisemeBoneDefine _basePose = (VisemeBoneDefine) null;
    foreach (VisemeBoneDefine visemeBone in this.VisemeBones)
    {
      phn_string_array_t phonemes = this.phnMap.GetPhonemes(visemeBone.m_visemeLabel);
      if (phonemes == null)
      {
        Debug.Log((object) ("Invalid Phoneme Map: can't find " + visemeBone.m_visemeLabel));
      }
      else
      {
        foreach (string phn in phonemes.phns)
          _labelToBoneDict.Add(phn, visemeBone);
      }
      if (visemeBone.m_visemeLabel == "x")
        _basePose = visemeBone;
    }
    this.visemeBoneDeformer = new AnnoBoneDeformer(_labelToBoneDict, _basePose);
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

  public void ImportBoneConfig()
  {
    bool bKeepLocalRotations = true;
    if (!((Object) this.BoneConfigFile != (Object) null))
      return;
    List<VisemeBoneDefine> visemeBoneDefineList = new List<VisemeBoneDefine>();
    Hashtable cache = new Hashtable();
    XmlTextReader reader = new XmlTextReader((TextReader) new StringReader(this.BoneConfigFile.text));
    while (reader.Read())
    {
      if (reader.Name == "mapping")
      {
        Debug.Log((object) "Read mapping");
        this.phnMap = new PhonemeVisemeMapping();
        this.phnMap.ReadMapping(reader, "mapping");
      }
      else if (reader.Name == "scale")
      {
        Debug.Log((object) "Read scale");
        this.importScale = XMLUtils.ReadXMLInnerTextFloat(reader, "scale");
      }
      else if (reader.Name == "flipx")
      {
        Debug.Log((object) "Read flip");
        this.bImportFlipX = true;
      }
      else if (reader.Name == "ignore_local_rotations")
        bKeepLocalRotations = false;
      else if (reader.Name == "viseme")
      {
        Debug.Log((object) "Read viseme");
        VisemeBoneDefine visemeBoneDefine = new VisemeBoneDefine();
        visemeBoneDefine.ReadViseme(reader, "viseme", this.bImportFlipX, this.importScale);
        visemeBoneDefine.LoadUnityBones(this.transform, cache, bKeepLocalRotations);
        string viseme = this.phnMap.PhonemeToViseme(visemeBoneDefine.m_visemeLabel);
        if (viseme.Length != 0)
          visemeBoneDefine.m_visemeLabel = viseme;
        else
          Debug.LogError((object) ("Can't find phoneme to viseme mapping for <viseme><label>" + visemeBoneDefine.m_visemeLabel + "</label> in BoneConfigFile"));
        visemeBoneDefineList.Add(visemeBoneDefine);
      }
    }
    this.VisemeBones = new VisemeBoneDefine[visemeBoneDefineList.Count];
    for (int index = 0; index < visemeBoneDefineList.Count; ++index)
      this.VisemeBones[index] = visemeBoneDefineList[index];
    this.MouthBonesList = new Transform[cache.Count];
    this.BasePoses = new BonePose[cache.Count];
    int index1 = 0;
    foreach (DictionaryEntry dictionaryEntry in cache)
    {
      this.MouthBonesList[index1] = dictionaryEntry.Value as Transform;
      this.BasePoses[index1] = new BonePose();
      this.BasePoses[index1].InitializeBone(this.MouthBonesList[index1]);
      ++index1;
    }
  }

  public void ResetToBasePose()
  {
    foreach (VisemeBoneDefine visemeBone in this.VisemeBones)
      visemeBone.ResetBonePose(this.BasePoses);
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

  public bool HasBonedViseme(string which)
  {
    if (this.VisemeBones == null)
      return false;
    foreach (VisemeBoneDefine visemeBone in this.VisemeBones)
    {
      if (visemeBone != null && visemeBone.m_visemeLabel == which && visemeBone.HasPose)
        return true;
    }
    return false;
  }

  public VisemeBoneDefine GetViseme(string which)
  {
    if (this.VisemeBones == null)
      this.VisemeBones = new VisemeBoneDefine[0];
    foreach (VisemeBoneDefine visemeBone in this.VisemeBones)
    {
      if (visemeBone != null && visemeBone.m_visemeLabel == which)
        return visemeBone;
    }
    int length = this.VisemeBones.Length;
    VisemeBoneDefine[] visemeBoneDefineArray = new VisemeBoneDefine[length + 1];
    for (int index = 0; index < length; ++index)
      visemeBoneDefineArray[index] = this.VisemeBones[index];
    visemeBoneDefineArray[length] = new VisemeBoneDefine(which);
    this.VisemeBones = visemeBoneDefineArray;
    return visemeBoneDefineArray[length];
  }

  public void CommitBonesForCurrentViseme()
  {
    this.GetViseme(this.CurrentViseme).RecordBonePositions(this.MouthBonesList);
  }

  public void ShowViseme(string which) => this.GetViseme(which).ResetToThisPose();
}
