using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(MapTooltipResource))]
public class MapTooltipResource_Generated :
	MapTooltipResource,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		var resourceGenerated = (MapTooltipResource_Generated)target2;
		resourceGenerated.name = name;
		resourceGenerated.image = image;
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