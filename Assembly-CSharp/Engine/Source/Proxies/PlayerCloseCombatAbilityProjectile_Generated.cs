using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      PlayerCloseCombatAbilityProjectile_Generated projectileGenerated = (PlayerCloseCombatAbilityProjectile_Generated) target2;
      projectileGenerated.radius = this.radius;
      projectileGenerated.angle = this.angle;
      projectileGenerated.maximumXOffset = this.maximumXOffset;
      projectileGenerated.aims = this.aims;
      projectileGenerated.blocked = this.blocked;
      projectileGenerated.orientation = this.orientation;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Radius", this.radius);
      DefaultDataWriteUtility.Write(writer, "Angle", this.angle);
      DefaultDataWriteUtility.Write(writer, "MaximumXOffset", this.maximumXOffset);
      DefaultDataWriteUtility.Write(writer, "Aims", this.aims);
      DefaultDataWriteUtility.WriteEnum<BlockTypeEnum>(writer, "Blocked", this.blocked);
      DefaultDataWriteUtility.WriteEnum<HitOrientationTypeEnum>(writer, "Orientation", this.orientation);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.radius = DefaultDataReadUtility.Read(reader, "Radius", this.radius);
      this.angle = DefaultDataReadUtility.Read(reader, "Angle", this.angle);
      this.maximumXOffset = DefaultDataReadUtility.Read(reader, "MaximumXOffset", this.maximumXOffset);
      this.aims = DefaultDataReadUtility.Read(reader, "Aims", this.aims);
      this.blocked = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Blocked");
      this.orientation = DefaultDataReadUtility.ReadEnum<HitOrientationTypeEnum>(reader, "Orientation");
    }
  }
}
