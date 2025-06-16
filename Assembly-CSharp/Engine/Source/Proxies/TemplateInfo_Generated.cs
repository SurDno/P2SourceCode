using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(TemplateInfo))]
public class TemplateInfo_Generated :
	TemplateInfo,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<TemplateInfo_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var templateInfoGenerated = (TemplateInfo_Generated)target2;
		templateInfoGenerated.Id = Id;
		templateInfoGenerated.InventoryTemplate = InventoryTemplate;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", Id);
		UnityDataWriteUtility.Write(writer, "InventoryTemplate", InventoryTemplate);
	}

	public void DataRead(IDataReader reader, Type type) {
		Id = DefaultDataReadUtility.Read(reader, "Id", Id);
		InventoryTemplate = UnityDataReadUtility.Read(reader, "InventoryTemplate", InventoryTemplate);
	}
}