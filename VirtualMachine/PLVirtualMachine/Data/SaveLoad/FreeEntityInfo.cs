using System;
using System.Xml;
using Cofe.Serializations.Data;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Data.SaveLoad;

public class FreeEntityInfo : ISerializeStateSave, IDynamicLoadSerializable {
	public FreeEntityInfo() { }

	public FreeEntityInfo(Guid templateId, Guid instanceId) {
		TemplateId = templateId;
		InstanceId = instanceId;
	}

	public Guid TemplateId { get; private set; }

	public Guid InstanceId { get; private set; }

	public void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "TemplateId", TemplateId);
		SaveManagerUtility.Save(writer, "InstanceId", InstanceId);
	}

	public void LoadFromXML(XmlElement xmlNode) {
		for (var i = 0; i < xmlNode.ChildNodes.Count; ++i)
			if (xmlNode.ChildNodes[i].Name == "TemplateId")
				TemplateId = VMSaveLoadManager.ReadGuid(xmlNode.ChildNodes[i]);
			else if (xmlNode.ChildNodes[i].Name == "InstanceId")
				InstanceId = VMSaveLoadManager.ReadGuid(xmlNode.ChildNodes[i]);
	}
}