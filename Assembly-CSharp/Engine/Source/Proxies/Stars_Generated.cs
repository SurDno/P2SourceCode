using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Stars))]
public class Stars_Generated :
	Stars,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<Stars_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var starsGenerated = (Stars_Generated)target2;
		starsGenerated.brightness = brightness;
		starsGenerated.size = size;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Brightness", brightness);
		DefaultDataWriteUtility.Write(writer, "Size", size);
	}

	public void DataRead(IDataReader reader, Type type) {
		brightness = DefaultDataReadUtility.Read(reader, "Brightness", brightness);
		size = DefaultDataReadUtility.Read(reader, "Size", size);
	}
}