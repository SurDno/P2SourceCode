// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Common.Data.XmlReaderUtility
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Xml;

#nullable disable
namespace VirtualMachine.Common.Data
{
  public static class XmlReaderUtility
  {
    public static void SkipNode(XmlReader xml) => xml.Skip();

    public static string ReadContent(XmlReader xml)
    {
      string str = "";
      if (!xml.IsEmptyElement)
      {
        while (xml.Read())
        {
          if (xml.NodeType == XmlNodeType.Text)
            str = xml.Value;
          else if (xml.NodeType == XmlNodeType.EndElement)
            break;
        }
      }
      return str;
    }
  }
}
