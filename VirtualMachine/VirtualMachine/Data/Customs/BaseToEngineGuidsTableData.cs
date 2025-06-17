using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Serializations.Converters;
using Engine.Common.Comparers;
using VirtualMachine.Common.Data;

namespace VirtualMachine.Data.Customs
{
  public class BaseToEngineGuidsTableData : IEditorDataReader
  {
    private Dictionary<ulong, Guid> idToGuid;
    private Dictionary<Guid, ulong> guidToId;

    public void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      int capacity = 0;
      if (!xml.IsEmptyElement)
      {
        xml.MoveToAttribute("count");
        capacity = DefaultConverter.ParseInt(xml.Value);
      }
      idToGuid = new Dictionary<ulong, Guid>(capacity, UlongComparer.Instance);
      guidToId = new Dictionary<Guid, ulong>(capacity, GuidComparer.Instance);
      if (xml.IsEmptyElement)
        return;
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          xml.MoveToAttribute("key");
          ulong key = DefaultConverter.ParseUlong(xml.Value);
          Guid guid = DefaultConverter.ParseGuid(XmlReaderUtility.ReadContent(xml));
          idToGuid.Add(key, guid);
          guidToId.Add(guid, key);
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public ulong GetBaseGuidByEngineTemplateGuid(Guid guid)
    {
      guidToId.TryGetValue(guid, out ulong engineTemplateGuid);
      return engineTemplateGuid;
    }

    public Guid GetEngineTemplateGuidByBaseGuid(ulong id)
    {
      idToGuid.TryGetValue(id, out Guid templateGuidByBaseGuid);
      return templateGuidByBaseGuid;
    }
  }
}
