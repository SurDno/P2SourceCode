using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Engine.Source.Effects.Engine;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ExpressionEffect_Generated expressionEffectGenerated = (ExpressionEffect_Generated) target2;
      expressionEffectGenerated.name = this.name;
      expressionEffectGenerated.queue = this.queue;
      expressionEffectGenerated.enable = this.enable;
      expressionEffectGenerated.single = this.single;
      expressionEffectGenerated.durationType = this.durationType;
      expressionEffectGenerated.realTime = this.realTime;
      expressionEffectGenerated.duration = this.duration;
      expressionEffectGenerated.interval = this.interval;
      expressionEffectGenerated.expression = CloneableObjectUtility.RuntimeOnlyCopy<IEffectValueSetter>(this.expression);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Enable", this.enable);
      DefaultDataWriteUtility.Write(writer, "Single", this.single);
      DefaultDataWriteUtility.WriteEnum<DurationTypeEnum>(writer, "DurationType", this.durationType);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.realTime);
      DefaultDataWriteUtility.Write(writer, "Duration", this.duration);
      DefaultDataWriteUtility.Write(writer, "Interval", this.interval);
      DefaultDataWriteUtility.WriteSerialize<IEffectValueSetter>(writer, "Expression", this.expression);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.enable = DefaultDataReadUtility.Read(reader, "Enable", this.enable);
      this.single = DefaultDataReadUtility.Read(reader, "Single", this.single);
      this.durationType = DefaultDataReadUtility.ReadEnum<DurationTypeEnum>(reader, "DurationType");
      this.realTime = DefaultDataReadUtility.Read(reader, "RealTime", this.realTime);
      this.duration = DefaultDataReadUtility.Read(reader, "Duration", this.duration);
      this.interval = DefaultDataReadUtility.Read(reader, "Interval", this.interval);
      this.expression = DefaultDataReadUtility.ReadSerialize<IEffectValueSetter>(reader, "Expression");
    }
  }
}
