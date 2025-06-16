using System.Collections.Generic;
using System.Xml;

namespace BehaviorDesigner.Runtime;

public static class XmlDeserializationCache {
	private static Dictionary<int, XmlDocument> serializationCache = new();

	public static XmlDocument GetOrCreateData(string xml) {
		var hashCode = xml.GetHashCode();
		XmlDocument data;
		if (!serializationCache.TryGetValue(hashCode, out data)) {
			data = new XmlDocument();
			data.LoadXml(xml);
			serializationCache.Add(hashCode, data);
		}

		return data;
	}
}