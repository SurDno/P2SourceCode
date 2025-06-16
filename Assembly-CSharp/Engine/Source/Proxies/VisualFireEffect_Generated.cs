using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (VisualFireEffect))]
  public class VisualFireEffect_Generated : 
    VisualFireEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      VisualFireEffect_Generated instance = Activator.CreateInstance<VisualFireEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      VisualFireEffect_Generated fireEffectGenerated = (VisualFireEffect_Generated) target2;
      fireEffectGenerated.queue = queue;
      fireEffectGenerated.single = single;
      fireEffectGenerated.realTime = realTime;
      fireEffectGenerated.duration = duration;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Single", single);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      single = DefaultDataReadUtility.Read(reader, "Single", single);
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
    }
  }
}
