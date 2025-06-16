using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(WaterSupplyControllerComponent))]
public class WaterSupplyControllerComponent_Generated :
	WaterSupplyControllerComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<WaterSupplyControllerComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) { }

	public void DataWrite(IDataWriter writer) { }

	public void DataRead(IDataReader reader, Type type) { }
}