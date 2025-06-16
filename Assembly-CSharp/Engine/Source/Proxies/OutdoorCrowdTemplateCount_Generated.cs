using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(OutdoorCrowdTemplateCount))]
public class OutdoorCrowdTemplateCount_Generated :
	OutdoorCrowdTemplateCount,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<OutdoorCrowdTemplateCount_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var templateCountGenerated = (OutdoorCrowdTemplateCount_Generated)target2;
		templateCountGenerated.Min = Min;
		templateCountGenerated.Max = Max;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Min", Min);
		DefaultDataWriteUtility.Write(writer, "Max", Max);
	}

	public void DataRead(IDataReader reader, Type type) {
		Min = DefaultDataReadUtility.Read(reader, "Min", Min);
		Max = DefaultDataReadUtility.Read(reader, "Max", Max);
	}
}