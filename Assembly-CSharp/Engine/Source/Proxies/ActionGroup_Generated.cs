using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ActionGroup))]
public class ActionGroup_Generated :
	ActionGroup,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ActionGroup_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var actionGroupGenerated = (ActionGroup_Generated)target2;
		actionGroupGenerated.Name = Name;
		actionGroupGenerated.Key = Key;
		actionGroupGenerated.Joystick = Joystick;
		actionGroupGenerated.JoystickHold = JoystickHold;
		actionGroupGenerated.IsChangeble = IsChangeble;
		actionGroupGenerated.Hide = Hide;
		actionGroupGenerated.Interact = Interact;
		actionGroupGenerated.Context = Context;
		CloneableObjectUtility.FillListTo(actionGroupGenerated.Actions, Actions);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", Name);
		DefaultDataWriteUtility.WriteEnum(writer, "Key", Key);
		DefaultDataWriteUtility.Write(writer, "Joystick", Joystick);
		DefaultDataWriteUtility.Write(writer, "JoystickHold", JoystickHold);
		DefaultDataWriteUtility.Write(writer, "IsChangeble", IsChangeble);
		DefaultDataWriteUtility.Write(writer, "Hide", Hide);
		DefaultDataWriteUtility.Write(writer, "Interact", Interact);
		DefaultDataWriteUtility.WriteEnum(writer, "Context", Context);
		DefaultDataWriteUtility.WriteListEnum(writer, "Actions", Actions);
	}

	public void DataRead(IDataReader reader, Type type) {
		Name = DefaultDataReadUtility.Read(reader, "Name", Name);
		Key = DefaultDataReadUtility.ReadEnum<KeyCode>(reader, "Key");
		Joystick = DefaultDataReadUtility.Read(reader, "Joystick", Joystick);
		JoystickHold = DefaultDataReadUtility.Read(reader, "JoystickHold", JoystickHold);
		IsChangeble = DefaultDataReadUtility.Read(reader, "IsChangeble", IsChangeble);
		Hide = DefaultDataReadUtility.Read(reader, "Hide", Hide);
		Interact = DefaultDataReadUtility.Read(reader, "Interact", Interact);
		Context = DefaultDataReadUtility.ReadEnum<ActionGroupContext>(reader, "Context");
		Actions = DefaultDataReadUtility.ReadListEnum(reader, "Actions", Actions);
	}
}