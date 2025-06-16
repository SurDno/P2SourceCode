using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextStammKindValueAssignment))]
  public class EffectContextStammKindValueAssignment_Generated : 
    EffectContextStammKindValueAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextStammKindValueAssignment_Generated instance = Activator.CreateInstance<EffectContextStammKindValueAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextStammKindValueAssignment_Generated assignmentGenerated = (EffectContextStammKindValueAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<StammKind>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<StammKind>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<StammKind>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<StammKind>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<StammKind>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Source");
    }
  }
}
