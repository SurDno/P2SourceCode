using System;
using System.Collections.Generic;
using System.Xml;

[Serializable]
public class PhonemeVisemeMapping
{
  public string[] visNames = null;
  public phn_string_array_t[] mapping = null;

  public PhonemeVisemeMapping()
  {
  }

  public PhonemeVisemeMapping(string[] names, string[][] phns)
  {
    visNames = names;
    mapping = new phn_string_array_t[phns.Length];
    for (int index = 0; index < phns.Length; ++index)
      mapping[index] = new phn_string_array_t(phns[index]);
  }

  public virtual int GetNumVisemes() => visNames.Length;

  public virtual string[] GetVisemeNames() => visNames;

  public string[] GetVisemePhonemes(string vis)
  {
    for (int index = 0; index < visNames.Length; ++index)
    {
      if (visNames[index] == vis)
        return mapping[index].phns;
    }
    return null;
  }

  public phn_string_array_t GetPhonemes(string visLabel)
  {
    for (int index = 0; index < visNames.Length; ++index)
    {
      if (visLabel == visNames[index])
        return mapping[index];
    }
    return null;
  }

  public string PhonemeToViseme(string phn)
  {
    foreach (phn_string_array_t phnStringArrayT in mapping)
    {
      foreach (string phn1 in phnStringArrayT.phns)
      {
        if (string.Compare(phn1, phn, true) == 0)
          return phnStringArrayT.phns[0] == "l" ? "L" : phnStringArrayT.phns[0];
      }
    }
    return "";
  }

  public void ReadMapping(XmlTextReader reader, string endTag)
  {
    bool flag = false;
    List<List<string>> stringListList = new List<List<string>>();
    while (!flag && reader.Read())
    {
      if (reader.Name == endTag && reader.NodeType == XmlNodeType.EndElement)
        flag = true;
      else if (reader.Name == "vis_phns")
        stringListList.Add(Read_Phns(reader, "vis_phns"));
    }
    visNames = new string[stringListList.Count];
    mapping = new phn_string_array_t[stringListList.Count];
    for (int index = 0; index < stringListList.Count; ++index)
    {
      if (stringListList[index].Count != 0)
      {
        visNames[index] = stringListList[index][0];
        if (visNames[index] == "l")
          visNames[index] = "L";
      }
      else
      {
        Debug.Log((object) "Load Error: Bad vis_phns in bone config file");
        visNames[index] = "BAD";
      }
      mapping[index] = new phn_string_array_t(stringListList[index]);
    }
  }

  private List<string> Read_Phns(XmlTextReader reader, string endTag)
  {
    bool flag = false;
    List<string> stringList = new List<string>();
    while (!flag && reader.Read())
    {
      if (reader.Name == endTag && reader.NodeType == XmlNodeType.EndElement)
        flag = true;
      else if (reader.Name == "phn")
      {
        string str = XMLUtils.ReadXMLInnerText(reader, reader.Name);
        if (str.Length != 0)
          stringList.Add(str);
      }
    }
    return stringList;
  }
}
