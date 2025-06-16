using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(LipSyncInfo))]
public class LipSyncInfo_Generated :
	LipSyncInfo,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<LipSyncInfo_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var syncInfoGenerated = (LipSyncInfo_Generated)target2;
		syncInfoGenerated.Clip = Clip;
		syncInfoGenerated.Data = Data;
		syncInfoGenerated.Tag = Tag;
	}

	public void DataWrite(IDataWriter writer) {
		UnityDataWriteUtility.Write(writer, "Clip", Clip);
		DefaultDataWriteUtility.Write(writer, "Data", Data);
		DefaultDataWriteUtility.Write(writer, "Tag", Tag);
	}

	public void DataRead(IDataReader reader, Type type) {
		Clip = UnityDataReadUtility.Read(reader, "Clip", Clip);
		Data = DefaultDataReadUtility.Read(reader, "Data", Data);
		Tag = DefaultDataReadUtility.Read(reader, "Tag", Tag);
	}
}