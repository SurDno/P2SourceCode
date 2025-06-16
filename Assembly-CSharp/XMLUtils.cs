// Decompiled with JetBrains decompiler
// Type: XMLUtils
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Xml;

#nullable disable
public class XMLUtils
{
  public static string ReadXMLInnerText(XmlTextReader reader, string endTag)
  {
    string str = "";
    while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == endTag)))
      str += reader.Value;
    return str;
  }

  public static float ReadXMLInnerTextFloat(XmlTextReader reader, string endTag)
  {
    return (float) Convert.ToDouble(XMLUtils.ReadXMLInnerText(reader, endTag));
  }
}
