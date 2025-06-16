using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      MoveProjectileEffect_Generated projectileEffectGenerated = (MoveProjectileEffect_Generated) target2;
      projectileEffectGenerated.queue = queue;
      projectileEffectGenerated.throwPower = throwPower;
      projectileEffectGenerated.projectilePrefab = projectilePrefab;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Power", throwPower);
      UnityDataWriteUtility.Write(writer, "ProjectilePrefab", projectilePrefab);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      throwPower = DefaultDataReadUtility.Read(reader, "Power", throwPower);
      projectilePrefab = UnityDataReadUtility.Read(reader, "ProjectilePrefab", projectilePrefab);
    }
  }
}
