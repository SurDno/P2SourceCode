// Decompiled with JetBrains decompiler
// Type: VirtualMachine.Data.Customs.BaseToEngineGuidsTableData
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Serializations.Converters;
using Engine.Common.Comparers;
using System;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common.Data;

#nullable disable
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
      this.idToGuid = new Dictionary<ulong, Guid>(capacity, (IEqualityComparer<ulong>) UlongComparer.Instance);
      this.guidToId = new Dictionary<Guid, ulong>(capacity, (IEqualityComparer<Guid>) GuidComparer.Instance);
      if (xml.IsEmptyElement)
        return;
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          xml.MoveToAttribute("key");
          ulong key = DefaultConverter.ParseUlong(xml.Value);
          Guid guid = DefaultConverter.ParseGuid(XmlReaderUtility.ReadContent(xml));
          this.idToGuid.Add(key, guid);
          this.guidToId.Add(guid, key);
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public ulong GetBaseGuidByEngineTemplateGuid(Guid guid)
    {
      ulong engineTemplateGuid = 0;
      this.guidToId.TryGetValue(guid, out engineTemplateGuid);
      return engineTemplateGuid;
    }

    public Guid GetEngineTemplateGuidByBaseGuid(ulong id)
    {
      Guid templateGuidByBaseGuid;
      this.idToGuid.TryGetValue(id, out templateGuidByBaseGuid);
      return templateGuidByBaseGuid;
    }
  }
}
