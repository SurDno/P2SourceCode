using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MoveProjectileEffect))]
  public class MoveProjectileEffect_Generated : 
    MoveProjectileEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      MoveProjectileEffect_Generated instance = Activator.CreateInstance<MoveProjectileEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      MoveProjectileEffect_Generated projectileEffectGenerated = (MoveProjectileEffect_Generated) target2;
      projectileEffectGenerated.queue = this.queue;
      projectileEffectGenerated.throwPower = this.throwPower;
      projectileEffectGenerated.projectilePrefab = this.projectilePrefab;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Power", this.throwPower);
      UnityDataWriteUtility.Write<GameObject>(writer, "ProjectilePrefab", this.projectilePrefab);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.throwPower = DefaultDataReadUtility.Read(reader, "Power", this.throwPower);
      this.projectilePrefab = UnityDataReadUtility.Read<GameObject>(reader, "ProjectilePrefab", this.projectilePrefab);
    }
  }
}
