using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NpcPunchHitEffect))]
  public class NpcPunchHitEffect_Generated : 
    NpcPunchHitEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NpcPunchHitEffect_Generated instance = Activator.CreateInstance<NpcPunchHitEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      NpcPunchHitEffect_Generated hitEffectGenerated = (NpcPunchHitEffect_Generated) target2;
      hitEffectGenerated.queue = queue;
      hitEffectGenerated.weapon = weapon;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.WriteEnum(writer, "Weapon", weapon);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      weapon = DefaultDataReadUtility.ReadEnum<WeaponEnum>(reader, "Weapon");
    }
  }
}
