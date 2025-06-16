using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Storable;
using Engine.Source.Commons.Abilities.Controllers;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ItemMountAbilityController))]
public class ItemMountAbilityController_Generated :
	ItemMountAbilityController,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ItemMountAbilityController_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((ItemMountAbilityController_Generated)target2).group = group;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Group", group);
	}

	public void DataRead(IDataReader reader, Type type) {
		group = DefaultDataReadUtility.ReadEnum<InventoryGroup>(reader, "Group");
	}
}