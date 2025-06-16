using System;
using System.Xml;

public class XMLUtils {
	public static string ReadXMLInnerText(XmlTextReader reader, string endTag) {
		var str = "";
		while (reader.Read() && (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == endTag)))
			str += reader.Value;
		return str;
	}

	public static float ReadXMLInnerTextFloat(XmlTextReader reader, string endTag) {
		return (float)Convert.ToDouble(ReadXMLInnerText(reader, endTag));
	}
}