using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(StorableData))]
public class StorableData_Generated : StorableData, ISerializeStateSave, ISerializeStateLoad {
	public void StateSave(IDataWriter writer) {
		CustomStateSaveUtility.SaveReference(writer, "Storage", Storage);
		DefaultDataWriteUtility.Write(writer, "TemplateId", TemplateId);
	}

	public void StateLoad(IDataReader reader, Type type) {
		Storage = CustomStateLoadUtility.LoadReference(reader, "Storage", Storage);
		TemplateId = DefaultDataReadUtility.Read(reader, "TemplateId", TemplateId);
	}
}