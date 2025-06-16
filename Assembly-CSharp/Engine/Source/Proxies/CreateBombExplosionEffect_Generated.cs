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
  [FactoryProxy(typeof (CreateBombExplosionEffect))]
  public class CreateBombExplosionEffect_Generated : 
    CreateBombExplosionEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CreateBombExplosionEffect_Generated instance = Activator.CreateInstance<CreateBombExplosionEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CreateBombExplosionEffect_Generated explosionEffectGenerated = (CreateBombExplosionEffect_Generated) target2;
      explosionEffectGenerated.queue = this.queue;
      explosionEffectGenerated.template = this.template;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.template);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.template);
    }
  }
}
