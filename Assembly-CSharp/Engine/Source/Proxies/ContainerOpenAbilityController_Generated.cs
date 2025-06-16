using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ContainerOpenAbilityController))]
public class ContainerOpenAbilityController_Generated :
	ContainerOpenAbilityController,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ContainerOpenAbilityController_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) { }

	public void DataWrite(IDataWriter writer) { }

	public void DataRead(IDataReader reader, Type type) { }
}