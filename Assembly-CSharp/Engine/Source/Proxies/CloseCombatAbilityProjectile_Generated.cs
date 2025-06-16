using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Projectiles;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CloseCombatAbilityProjectile))]
  public class CloseCombatAbilityProjectile_Generated : 
    CloseCombatAbilityProjectile,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CloseCombatAbilityProjectile_Generated instance = Activator.CreateInstance<CloseCombatAbilityProjectile_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloseCombatAbilityProjectile_Generated projectileGenerated = (CloseCombatAbilityProjectile_Generated) target2;
      projectileGenerated.blocked = this.blocked;
      projectileGenerated.orientation = this.orientation;
      projectileGenerated.radius = this.radius;
      projectileGenerated.angle = this.angle;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<BlockTypeEnum>(writer, "Blocked", this.blocked);
      DefaultDataWriteUtility.WriteEnum<HitOrientationTypeEnum>(writer, "Orientation", this.orientation);
      DefaultDataWriteUtility.Write(writer, "Radius", this.radius);
      DefaultDataWriteUtility.Write(writer, "Angle", this.angle);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.blocked = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Blocked");
      this.orientation = DefaultDataReadUtility.ReadEnum<HitOrientationTypeEnum>(reader, "Orientation");
      this.radius = DefaultDataReadUtility.Read(reader, "Radius", this.radius);
      this.angle = DefaultDataReadUtility.Read(reader, "Angle", this.angle);
    }
  }
}
