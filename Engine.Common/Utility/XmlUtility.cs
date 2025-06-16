using Cofe.Utility;
using System.IO;
using System.Xml;

namespace Engine.Common.Utility
{
  public static class XmlUtility
  {
    public static XmlDocument CreateDocument()
    {
      XmlDocument document = new XmlDocument();
      document.AppendChild((XmlNode) document.CreateXmlDeclaration("1.0", "UTF-8", (string) null));
      return document;
    }

    public static void SaveDocument(this XmlDocument doc, string path)
    {
      FileInfo fileInfo = new FileInfo(path);
      if (!fileInfo.Directory.Exists)
        fileInfo.Directory.Create();
      doc.Save(path);
    }

    public static void SaveDocument(this XmlElement node, string path)
    {
      node.OwnerDocument.SaveDocument(path);
    }

    public static XmlElement CreateNode(this XmlDocument doc, string name)
    {
      XmlElement element = doc.CreateElement(name);
      doc.AppendChild((XmlNode) element);
      return element;
    }

    public static XmlElement CreateNode(this XmlElement node, string name)
    {
      XmlElement element = node.OwnerDocument.CreateElement(name);
      node.AppendChild((XmlNode) element);
      return element;
    }

    public static XmlElement CreateNode(this XmlElement node, string name, string value)
    {
      XmlElement element = node.OwnerDocument.CreateElement(name);
      node.AppendChild((XmlNode) element);
      if (!value.IsNullOrEmpty())
        element.InnerText = value;
      return element;
    }

    public static XmlAttribute CreateAttribute(this XmlElement node, string name)
    {
      XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
      node.Attributes.Append(attribute);
      return attribute;
    }

    public static void CreateAttribute(this XmlElement node, string name, string value)
    {
      XmlAttribute attribute = node.OwnerDocument.CreateAttribute(name);
      node.Attributes.Append(attribute);
      attribute.Value = value;
    }
  }
}
