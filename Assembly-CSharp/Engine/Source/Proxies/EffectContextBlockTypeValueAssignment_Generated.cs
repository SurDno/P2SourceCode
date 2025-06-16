using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextBlockTypeValueAssignment))]
  public class EffectContextBlockTypeValueAssignment_Generated : 
    EffectContextBlockTypeValueAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextBlockTypeValueAssignment_Generated instance = Activator.CreateInstance<EffectContextBlockTypeValueAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextBlockTypeValueAssignment_Generated assignmentGenerated = (EffectContextBlockTypeValueAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<BlockTypeEnum>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<BlockTypeEnum>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<BlockTypeEnum>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<BlockTypeEnum>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<BlockTypeEnum>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<BlockTypeEnum>>(reader, "Source");
    }
  }
}
