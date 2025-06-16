using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(JoystickLayout))]
public class JoystickLayout_Generated :
	JoystickLayout,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<JoystickLayout_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var joystickLayoutGenerated = (JoystickLayout_Generated)target2;
		joystickLayoutGenerated.Name = Name;
		CloneableObjectUtility.CopyListTo(joystickLayoutGenerated.Axes, Axes);
		CloneableObjectUtility.CopyListTo(joystickLayoutGenerated.AxesToButtons, AxesToButtons);
		CloneableObjectUtility.CopyListTo(joystickLayoutGenerated.KeysToButtons, KeysToButtons);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Axes", Axes);
		DefaultDataWriteUtility.WriteListSerialize(writer, "AxesToButtons", AxesToButtons);
		DefaultDataWriteUtility.WriteListSerialize(writer, "KeysToButtons", KeysToButtons);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
		Axes = DefaultDataReadUtility.ReadListSerialize(reader, "Axes", Axes);
		AxesToButtons = DefaultDataReadUtility.ReadListSerialize(reader, "AxesToButtons", AxesToButtons);
		KeysToButtons = DefaultDataReadUtility.ReadListSerialize(reader, "KeysToButtons", KeysToButtons);
	}
}