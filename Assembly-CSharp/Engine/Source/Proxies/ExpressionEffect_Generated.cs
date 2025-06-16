using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Engine.Source.Effects.Engine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ExpressionEffect))]
  public class ExpressionEffect_Generated : 
    ExpressionEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ExpressionEffect_Generated instance = Activator.CreateInstance<ExpressionEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ExpressionEffect_Generated expressionEffectGenerated = (ExpressionEffect_Generated) target2;
      expressionEffectGenerated.name = name;
      expressionEffectGenerated.queue = queue;
      expressionEffectGenerated.enable = enable;
      expressionEffectGenerated.single = single;
      expressionEffectGenerated.durationType = durationType;
      expressionEffectGenerated.realTime = realTime;
      expressionEffectGenerated.duration = duration;
      expressionEffectGenerated.interval = interval;
      expressionEffectGenerated.expression = CloneableObjectUtility.RuntimeOnlyCopy(expression);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", name);
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Enable", enable);
      DefaultDataWriteUtility.Write(writer, "Single", single);
      DefaultDataWriteUtility.WriteEnum(writer, "DurationType", durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", duration);
      DefaultDataWriteUtility.Write(writer, "Interval", interval);
      DefaultDataWriteUtility.WriteSerialize(writer, "Expression", expression);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.Read(reader, "Name", name);
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      enable = DefaultDataReadUtility.Read(reader, "Enable", enable);
      single = DefaultDataReadUtility.Read(reader, "Single", single);
      durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      realTime = DefaultDataReadUtility.Read(reader, "RealTime", realTime);
      duration = DefaultDataReadUtility.Read(reader, "Duration", duration);
      interval = DefaultDataReadUtility.Read(reader, "Interval", interval);
      expression = DefaultDataReadUtility.ReadSerialize<IEffectValueSetter>(reader, "Expression");
    }
  }
}
