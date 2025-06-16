using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Engine.Source.Effects.Engine;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextDetectTypeValueAssignment))]
  public class EffectContextDetectTypeValueAssignment_Generated : 
    EffectContextDetectTypeValueAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextDetectTypeValueAssignment_Generated instance = Activator.CreateInstance<EffectContextDetectTypeValueAssignment_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextDetectTypeValueAssignment_Generated assignmentGenerated = (EffectContextDetectTypeValueAssignment_Generated) target2;
      assignmentGenerated.a = CloneableObjectUtility.Clone<IValueSetter<DetectType>>(this.a);
      assignmentGenerated.b = CloneableObjectUtility.Clone<IValue<DetectType>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValueSetter<DetectType>>(writer, "A", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<DetectType>>(writer, "Source", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValueSetter<DetectType>>(reader, "A");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Source");
    }
  }
}
