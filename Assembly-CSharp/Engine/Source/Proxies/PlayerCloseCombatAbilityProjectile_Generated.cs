using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerCloseCombatAbilityProjectile))]
  public class PlayerCloseCombatAbilityProjectile_Generated : 
    PlayerCloseCombatAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerCloseCombatAbilityProjectile_Generated instance = Activator.CreateInstance<PlayerCloseCombatAbilityProjectile_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      PlayerCloseCombatAbilityProjectile_Generated projectileGenerated = (PlayerCloseCombatAbilityProjectile_Generated) target2;
      projectileGenerated.radius = radius;
      projectileGenerated.angle = angle;
      projectileGenerated.maximumXOffset = maximumXOffset;
      projectileGenerated.aims = aims;
      projectileGenerated.blocked = blocked;
      projectileGenerated.orientation = orientation;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Radius", radius);
      DefaultDataWriteUtility.Write(writer, "Angle", angle);
      DefaultDataWriteUtility.Write(writer, "MaximumXOffset", maximumXOffset);
      DefaultDataWriteUtility.Write(writer, "Aims", aims);
      DefaultDataWriteUtility.WriteEnum(writer, "Blocked", blocked);
      DefaultDataWriteUtility.WriteEnum(writer, "Orientation", orientation);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      radius = DefaultDataReadUtility.Read(reader, "Radius", radius);
      angle = DefaultDataReadUtility.Read(reader, "Angle", angle);
      maximumXOffset = DefaultDataReadUtility.Read(reader, "MaximumXOffset", maximumXOffset);
      aims = DefaultDataReadUtility.Read(reader, "Aims", aims);
      blocked = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Blocked");
      orientation = DefaultDataReadUtility.ReadEnum<HitOrientationTypeEnum>(reader, "Orientation");
    }
  }
}
