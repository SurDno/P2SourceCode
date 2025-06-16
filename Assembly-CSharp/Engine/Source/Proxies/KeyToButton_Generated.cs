using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using InputServices;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(KeyToButton))]
public class KeyToButton_Generated :
	KeyToButton,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<KeyToButton_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var toButtonGenerated = (KeyToButton_Generated)target2;
		toButtonGenerated.Name = Name;
		toButtonGenerated.KeyCode = KeyCode;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
		DefaultDataWriteUtility.WriteEnum(writer, "KeyCode", KeyCode);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
		KeyCode = DefaultDataReadUtility.ReadEnum<JoystickKeyCode>(reader, "KeyCode");
	}
}