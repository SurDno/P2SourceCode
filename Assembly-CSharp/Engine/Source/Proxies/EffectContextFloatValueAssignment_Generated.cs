using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextFloatValueAssignment))]
  public class EffectContextFloatValueAssignment_Generated : 
    EffectContextFloatValueAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextFloatValueAssignment_Generated instance = Activator.CreateInstance<EffectContextFloatValueAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextFloatValueAssignment_Generated assignmentGenerated = (EffectContextFloatValueAssignment_Generated) target2;
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
