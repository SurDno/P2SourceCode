using System.Collections.Generic;
using System.Xml;

namespace BehaviorDesigner.Runtime
{
  public static class XmlDeserializationCache
  {
    private static Dictionary<int, XmlDocument> serializationCache = new Dictionary<int, XmlDocument>();

    public static XmlDocument GetOrCreateData(string xml)
    {
      int hashCode = xml.GetHashCode();
      XmlDocument data;
      if (!XmlDeserializationCache.serializationCache.TryGetValue(hashCode, out data))
      {
        data = new XmlDocument();
        data.LoadXml(xml);
        XmlDeserializationCache.serializationCache.Add(hashCode, data);
      }
      return data;
    }
  }
}
