using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Fog))]
public class Fog_Generated : Fog, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<Fog_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var fogGenerated = (Fog_Generated)target2;
		fogGenerated.density = density;
		fogGenerated.height = height;
		fogGenerated.startDistance = startDistance;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Density", density);
		DefaultDataWriteUtility.Write(writer, "Height", height);
		DefaultDataWriteUtility.Write(writer, "StartDistance", startDistance);
	}

	public void DataRead(IDataReader reader, Type type) {
		density = DefaultDataReadUtility.Read(reader, "Density", density);
		height = DefaultDataReadUtility.Read(reader, "Height", height);
		startDistance = DefaultDataReadUtility.Read(reader, "StartDistance", startDistance);
	}
}