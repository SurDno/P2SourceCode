using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextIntValueSubtractionAssignment))]
  public class EffectContextIntValueSubtractionAssignment_Generated : 
    EffectContextIntValueSubtractionAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextIntValueSubtractionAssignment_Generated instance = Activator.CreateInstance<EffectContextIntValueSubtractionAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextIntValueSubtractionAssignment_Generated assignmentGenerated = (EffectContextIntValueSubtractionAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<int>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<int>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<int>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<int>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<int>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "Source");
    }
  }
}
