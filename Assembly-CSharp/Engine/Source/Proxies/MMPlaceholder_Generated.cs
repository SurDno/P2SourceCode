using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.MindMap;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MMPlaceholder))]
public class MMPlaceholder_Generated :
	MMPlaceholder,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var placeholderGenerated = (MMPlaceholder_Generated)target2;
		placeholderGenerated.name = name;
		placeholderGenerated.image = image;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Id", id);
		UnityDataWriteUtility.Write(writer, "Image", image);
	}

	public void DataRead(IDataReader reader, Type type) {
		id = DefaultDataReadUtility.Read(reader, "Id", id);
		image = UnityDataReadUtility.Read(reader, "Image", image);
	}
}