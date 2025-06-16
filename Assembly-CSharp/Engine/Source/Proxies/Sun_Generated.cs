using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Sun))]
public class Sun_Generated : Sun, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<Sun_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var sunGenerated = (Sun_Generated)target2;
		sunGenerated.brightness = brightness;
		sunGenerated.contrast = contrast;
		sunGenerated.size = size;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Brightness", brightness);
		DefaultDataWriteUtility.Write(writer, "Contrast", contrast);
		DefaultDataWriteUtility.Write(writer, "Size", size);
	}

	public void DataRead(IDataReader reader, Type type) {
		brightness = DefaultDataReadUtility.Read(reader, "Brightness", brightness);
		contrast = DefaultDataReadUtility.Read(reader, "Contrast", contrast);
		size = DefaultDataReadUtility.Read(reader, "Size", size);
	}
}