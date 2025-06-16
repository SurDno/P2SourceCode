using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(BehaviorTreeAbilityController))]
public class BehaviorTreeAbilityController_Generated :
	BehaviorTreeAbilityController,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<BehaviorTreeAbilityController_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((BehaviorTreeAbilityController_Generated)target2).name = name;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Name", name);
	}

	public void DataRead(IDataReader reader, Type type) {
		name = DefaultDataReadUtility.Read(reader, "Name", name);
	}
}