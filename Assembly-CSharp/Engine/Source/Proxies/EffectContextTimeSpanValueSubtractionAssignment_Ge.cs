using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextTimeSpanValueSubtractionAssignment))]
  public class EffectContextTimeSpanValueSubtractionAssignment_Generated : 
    EffectContextTimeSpanValueSubtractionAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextTimeSpanValueSubtractionAssignment_Generated instance = Activator.CreateInstance<EffectContextTimeSpanValueSubtractionAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextTimeSpanValueSubtractionAssignment_Generated assignmentGenerated = (EffectContextTimeSpanValueSubtractionAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<TimeSpan>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<TimeSpan>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<TimeSpan>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<TimeSpan>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<TimeSpan>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "Source");
    }
  }
}
