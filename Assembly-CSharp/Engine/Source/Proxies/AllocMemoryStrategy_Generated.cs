using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(AllocMemoryStrategy))]
public class AllocMemoryStrategy_Generated :
	AllocMemoryStrategy,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<AllocMemoryStrategy_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var strategyGenerated = (AllocMemoryStrategy_Generated)target2;
		strategyGenerated.maxMemory = maxMemory;
		strategyGenerated.minMemory = minMemory;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "MaxMemory", maxMemory);
		DefaultDataWriteUtility.Write(writer, "MinMemory", minMemory);
	}

	public void DataRead(IDataReader reader, Type type) {
		maxMemory = DefaultDataReadUtility.Read(reader, "MaxMemory", maxMemory);
		minMemory = DefaultDataReadUtility.Read(reader, "MinMemory", minMemory);
	}
}