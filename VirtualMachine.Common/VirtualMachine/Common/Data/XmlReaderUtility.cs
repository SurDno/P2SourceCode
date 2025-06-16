using System.Xml;

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
