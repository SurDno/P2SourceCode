using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ForceCollectMemoryStrategy))]
public class ForceCollectMemoryStrategy_Generated :
	ForceCollectMemoryStrategy,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ForceCollectMemoryStrategy_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((ForceCollectMemoryStrategy_Generated)target2).disableGC = disableGC;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "DisableGC", disableGC);
	}

	public void DataRead(IDataReader reader, Type type) {
		disableGC = DefaultDataReadUtility.Read(reader, "DisableGC", disableGC);
	}
}