using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(StorableTooltipSimple))]
public class StorableTooltipSimple_Generated :
	StorableTooltipSimple,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<StorableTooltipSimple_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var tooltipSimpleGenerated = (StorableTooltipSimple_Generated)target2;
		tooltipSimpleGenerated.isEnabled = isEnabled;
		tooltipSimpleGenerated.info = CloneableObjectUtility.Clone(info);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		DefaultDataWriteUtility.WriteSerialize(writer, "Info", info);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		info = DefaultDataReadUtility.ReadSerialize<StorableTooltipInfo>(reader, "Info");
	}
}