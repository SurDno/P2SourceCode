// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.XmlDeserializationCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Xml;

#nullable disable
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
