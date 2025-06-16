using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextTimeSpanValueSubtractionAssignment_Generated assignmentGenerated = (EffectContextTimeSpanValueSubtractionAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone(a);
      assignmentGenerated.b = CloneableObjectUtility.Clone(b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "A", a);
      DefaultDataWriteUtility.WriteSerialize(writer, "Source", b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      a = DefaultDataReadUtility.ReadSerialize<IValueSetter<TimeSpan>>(reader, "A");
      b = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "Source");
    }
  }
}
