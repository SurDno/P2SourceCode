using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(AxisToButton))]
public class AxisToButton_Generated :
	AxisToButton,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<AxisToButton_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var toButtonGenerated = (AxisToButton_Generated)target2;
		toButtonGenerated.Name = Name;
		toButtonGenerated.Axis = Axis;
		toButtonGenerated.Min = Min;
		toButtonGenerated.Max = Max;
		toButtonGenerated.Inverse = Inverse;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
		DefaultDataWriteUtility.Write(writer, "Axis", Axis);
		DefaultDataWriteUtility.Write(writer, "Min", Min);
		DefaultDataWriteUtility.Write(writer, "Max", Max);
		DefaultDataWriteUtility.Write(writer, "Inverse", Inverse);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
		Axis = DefaultDataReadUtility.Read(reader, "Axis", Axis);
		Min = DefaultDataReadUtility.Read(reader, "Min", Min);
		Max = DefaultDataReadUtility.Read(reader, "Max", Max);
		Inverse = DefaultDataReadUtility.Read(reader, "Inverse", Inverse);
	}
}