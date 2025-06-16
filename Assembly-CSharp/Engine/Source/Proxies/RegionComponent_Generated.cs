using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Regions;
using Engine.Source.Components;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(RegionComponent))]
public class RegionComponent_Generated :
	RegionComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<RegionComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (RegionComponent_Generated)target2;
		componentGenerated.region = region;
		componentGenerated.regionBehaviour = regionBehaviour;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Region", region);
		DefaultDataWriteUtility.WriteEnum(writer, "RegionBehavior", regionBehaviour);
	}

	public void DataRead(IDataReader reader, Type type) {
		region = DefaultDataReadUtility.ReadEnum<RegionEnum>(reader, "Region");
		regionBehaviour = DefaultDataReadUtility.ReadEnum<RegionBehaviourEnum>(reader, "RegionBehavior");
	}
}