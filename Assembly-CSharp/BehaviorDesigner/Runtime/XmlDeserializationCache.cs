using System.Collections.Generic;
using System.Xml;

namespace BehaviorDesigner.Runtime
{
  public static class XmlDeserializationCache
  {
    private static Dictionary<int, XmlDocument> serializationCache = new();

    public static XmlDocument GetOrCreateData(string xml)
    {
      int hashCode = xml.GetHashCode();
      if (!serializationCache.TryGetValue(hashCode, out XmlDocument data))
      {
        data = new XmlDocument();
        data.LoadXml(xml);
        serializationCache.Add(hashCode, data);
      }
      return data;
    }
  }
}
