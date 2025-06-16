using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Impl.Weather.Element;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(Wind))]
public class Wind_Generated : Wind, ICloneable, ICopyable, ISerializeDataWrite, ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<Wind_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var windGenerated = (Wind_Generated)target2;
		windGenerated.degrees = degrees;
		windGenerated.speed = speed;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Degrees", degrees);
		DefaultDataWriteUtility.Write(writer, "Speed", speed);
	}

	public void DataRead(IDataReader reader, Type type) {
		degrees = DefaultDataReadUtility.Read(reader, "Degrees", degrees);
		speed = DefaultDataReadUtility.Read(reader, "Speed", speed);
	}
}