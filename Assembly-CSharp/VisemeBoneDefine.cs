using System;
using System.Collections;
using System.Xml;

[Serializable]
public class VisemeBoneDefine
{
  public string m_visemeLabel = "";
  public BonePose[] m_BonePoses;

  public VisemeBoneDefine()
  {
  }

  public VisemeBoneDefine(string vis) => m_visemeLabel = vis;

  public string Name
  {
    get => m_visemeLabel;
    set => m_visemeLabel = value;
  }

  public bool HasPose => m_BonePoses != null && m_BonePoses.Length != 0;

  public void RecordBonePositions(Transform[] boneList)
  {
    m_BonePoses = new BonePose[boneList.Length];
    for (int index = 0; index < boneList.Length; ++index)
    {
      m_BonePoses[index] = new BonePose();
      m_BonePoses[index].InitializeBone(boneList[index]);
    }
  }

  public void ResetBonePose(BonePose[] basePoses)
  {
    foreach (BonePose bonePose in m_BonePoses)
    {
      foreach (BonePose basePose in basePoses)
      {
        if ((UnityEngine.Object) basePose.m_bone == (UnityEngine.Object) bonePose.m_bone)
          bonePose.ResetToThisPose(basePose);
      }
    }
  }

  public void ReadViseme(XmlTextReader reader, string endTag, bool flipX, float scale)
  {
    bool flag = false;
    int num = 0;
    ArrayList arrayList = new ArrayList();
    while (!flag && reader.Read())
    {
      if (reader.Name == endTag && reader.NodeType == XmlNodeType.EndElement)
        flag = true;
      else if (reader.Name == "label")
        m_visemeLabel = XMLUtils.ReadXMLInnerText(reader, "label");
      else if (reader.Name == "bone")
      {
        ++num;
        BonePose bonePose = new BonePose();
        bonePose.ReadBonePose(reader, flipX, scale);
        arrayList.Add(bonePose);
      }
    }
    m_BonePoses = new BonePose[arrayList.Count];
    int index = 0;
    foreach (BonePose bonePose in arrayList)
    {
      m_BonePoses[index] = bonePose;
      ++index;
    }
  }

  public void LoadUnityBones(Transform gObject, Hashtable cache, bool bKeepLocalRotations)
  {
    foreach (BonePose bonePose in m_BonePoses)
      bonePose.LoadUnityBone(gObject, cache, bKeepLocalRotations);
  }

  public void ResetToThisPose()
  {
    foreach (BonePose bonePose in m_BonePoses)
      bonePose.ResetToThisTransform();
  }

  public void Print()
  {
    foreach (BonePose bonePose in m_BonePoses)
      bonePose.Print();
  }

  public void ConvertPosesToOffsetsFromBase(VisemeBoneDefine defaultModel)
  {
    if (this == defaultModel)
      return;
    foreach (BonePose bonePose1 in m_BonePoses)
    {
      BonePose bonePose2 = defaultModel.GetBonePose(bonePose1.boneName);
      if (bonePose2 != null)
        bonePose1.ConvertToOffsets(bonePose2);
      else
        Debug.Log((object) ("VisemeBoneDefine::ConvertPosesToOffsetsFromBase - can't find bone in base pose " + bonePose1.boneName));
    }
  }

  private BonePose GetBonePose(string boneName)
  {
    foreach (BonePose bonePose in m_BonePoses)
    {
      if (bonePose.boneName == boneName)
        return bonePose;
    }
    return null;
  }

  public void Deform(float weight, BoneDeformMemento memento)
  {
    foreach (BonePose bonePose in m_BonePoses)
      bonePose.Deform(weight, memento);
  }
}
