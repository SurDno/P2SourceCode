using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[Serializable]
public class PhonemeVisemeMapping
{
  public string[] visNames = (string[]) null;
  public phn_string_array_t[] mapping = (phn_string_array_t[]) null;

  public PhonemeVisemeMapping()
  {
  }

  public PhonemeVisemeMapping(string[] names, string[][] phns)
  {
    this.visNames = names;
    this.mapping = new phn_string_array_t[phns.Length];
    for (int index = 0; index < phns.Length; ++index)
      this.mapping[index] = new phn_string_array_t(phns[index]);
  }

  public virtual int GetNumVisemes() => this.visNames.Length;

  public virtual string[] GetVisemeNames() => this.visNames;

  public string[] GetVisemePhonemes(string vis)
  {
    for (int index = 0; index < this.visNames.Length; ++index)
    {
      if (this.visNames[index] == vis)
        return this.mapping[index].phns;
    }
    return (string[]) null;
  }

  public phn_string_array_t GetPhonemes(string visLabel)
  {
    for (int index = 0; index < this.visNames.Length; ++index)
    {
      if (visLabel == this.visNames[index])
        return this.mapping[index];
    }
    return (phn_string_array_t) null;
  }

  public string PhonemeToViseme(string phn)
  {
    foreach (phn_string_array_t phnStringArrayT in this.mapping)
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
        stringListList.Add(this.Read_Phns(reader, "vis_phns"));
    }
    this.visNames = new string[stringListList.Count];
    this.mapping = new phn_string_array_t[stringListList.Count];
    for (int index = 0; index < stringListList.Count; ++index)
    {
      if (stringListList[index].Count != 0)
      {
        this.visNames[index] = stringListList[index][0];
        if (this.visNames[index] == "l")
          this.visNames[index] = "L";
      }
      else
      {
        Debug.Log((object) "Load Error: Bad vis_phns in bone config file");
        this.visNames[index] = "BAD";
      }
      this.mapping[index] = new phn_string_array_t(stringListList[index]);
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
