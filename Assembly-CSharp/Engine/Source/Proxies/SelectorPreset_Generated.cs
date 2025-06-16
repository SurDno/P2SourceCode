using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Selectors;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(SelectorPreset))]
public class SelectorPreset_Generated :
	SelectorPreset,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<SelectorPreset_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		CloneableObjectUtility.CopyListTo(((SelectorPreset)target2).Objects, Objects);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteListSerialize(writer, "Objects", Objects);
	}

	public void DataRead(IDataReader reader, Type type) {
		Objects = DefaultDataReadUtility.ReadListSerialize(reader, "Objects", Objects);
	}
}