using System.IO;
using System.Xml;
using Cofe.Utility;

namespace Engine.Common.Utility;

public static class XmlUtility {
	public static XmlDocument CreateDocument() {
		var document = new XmlDocument();
		document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", null));
		return document;
	}

	public static void SaveDocument(this XmlDocument doc, string path) {
		var fileInfo = new FileInfo(path);
		if (!fileInfo.Directory.Exists)
			fileInfo.Directory.Create();
		doc.Save(path);
	}

	public static void SaveDocument(this XmlElement node, string path) {
		node.OwnerDocument.SaveDocument(path);
	}

	public static XmlElement CreateNode(this XmlDocument doc, string name) {
		var element = doc.CreateElement(name);
		doc.AppendChild(element);
		return element;
	}

	public static XmlElement CreateNode(this XmlElement node, string name) {
		var element = node.OwnerDocument.CreateElement(name);
		node.AppendChild(element);
		return element;
	}

	public static XmlElement CreateNode(this XmlElement node, string name, string value) {
		var element = node.OwnerDocument.CreateElement(name);
		node.AppendChild(element);
		if (!value.IsNullOrEmpty())
			element.InnerText = value;
		return element;
	}

	public static XmlAttribute CreateAttribute(this XmlElement node, string name) {
		var attribute = node.OwnerDocument.CreateAttribute(name);
		node.Attributes.Append(attribute);
		return attribute;
	}

	public static void CreateAttribute(this XmlElement node, string name, string value) {
		var attribute = node.OwnerDocument.CreateAttribute(name);
		node.Attributes.Append(attribute);
		attribute.Value = value;
	}
}