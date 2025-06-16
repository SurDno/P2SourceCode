using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CreateProjectileEntityEffect))]
  public class CreateProjectileEntityEffect_Generated : 
    CreateProjectileEntityEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CreateProjectileEntityEffect_Generated instance = Activator.CreateInstance<CreateProjectileEntityEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CreateProjectileEntityEffect_Generated entityEffectGenerated = (CreateProjectileEntityEffect_Generated) target2;
      entityEffectGenerated.queue = this.queue;
      entityEffectGenerated.template = this.template;
      entityEffectGenerated.spawnPlace = this.spawnPlace;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.template);
      DefaultDataWriteUtility.WriteEnum<ProjectileSpawnPlaceEnum>(writer, "SpawnPlace", this.spawnPlace);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.template);
      this.spawnPlace = DefaultDataReadUtility.ReadEnum<ProjectileSpawnPlaceEnum>(reader, "SpawnPlace");
    }
  }
}
