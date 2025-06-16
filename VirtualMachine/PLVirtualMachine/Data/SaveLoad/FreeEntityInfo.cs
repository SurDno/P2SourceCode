// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Data.SaveLoad.FreeEntityInfo
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;
using System;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Data.SaveLoad
{
  public class FreeEntityInfo : ISerializeStateSave, IDynamicLoadSerializable
  {
    public FreeEntityInfo()
    {
    }

    public FreeEntityInfo(Guid templateId, Guid instanceId)
    {
      this.TemplateId = templateId;
      this.InstanceId = instanceId;
    }

    public Guid TemplateId { get; private set; }

    public Guid InstanceId { get; private set; }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "TemplateId", this.TemplateId);
      SaveManagerUtility.Save(writer, "InstanceId", this.InstanceId);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i].Name == "TemplateId")
          this.TemplateId = VMSaveLoadManager.ReadGuid(xmlNode.ChildNodes[i]);
        else if (xmlNode.ChildNodes[i].Name == "InstanceId")
          this.InstanceId = VMSaveLoadManager.ReadGuid(xmlNode.ChildNodes[i]);
      }
    }
  }
}
