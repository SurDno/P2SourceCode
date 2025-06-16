using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextFloatValueSubtractionAssignment))]
  public class EffectContextFloatValueSubtractionAssignment_Generated : 
    EffectContextFloatValueSubtractionAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextFloatValueSubtractionAssignment_Generated instance = Activator.CreateInstance<EffectContextFloatValueSubtractionAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextFloatValueSubtractionAssignment_Generated assignmentGenerated = (EffectContextFloatValueSubtractionAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<float>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<float>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<float>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<float>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<float>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Source");
    }
  }
}
