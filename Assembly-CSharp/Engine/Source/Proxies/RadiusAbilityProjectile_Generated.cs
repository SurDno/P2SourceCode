using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(RadiusAbilityProjectile))]
public class RadiusAbilityProjectile_Generated :
	RadiusAbilityProjectile,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<RadiusAbilityProjectile_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var projectileGenerated = (RadiusAbilityProjectile_Generated)target2;
		projectileGenerated.radius = radius;
		projectileGenerated.ignoreSelf = ignoreSelf;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Radius", radius);
		DefaultDataWriteUtility.Write(writer, "IgnoreSelf", ignoreSelf);
	}

	public void DataRead(IDataReader reader, Type type) {
		radius = DefaultDataReadUtility.Read(reader, "Radius", radius);
		ignoreSelf = DefaultDataReadUtility.Read(reader, "IgnoreSelf", ignoreSelf);
	}
}