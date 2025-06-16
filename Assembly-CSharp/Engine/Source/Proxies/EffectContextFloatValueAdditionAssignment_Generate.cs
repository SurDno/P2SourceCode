using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Effects.Engine;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EffectContextFloatValueAdditionAssignment))]
  public class EffectContextFloatValueAdditionAssignment_Generated : 
    EffectContextFloatValueAdditionAssignment,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EffectContextFloatValueAdditionAssignment_Generated instance = Activator.CreateInstance<EffectContextFloatValueAdditionAssignment_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EffectContextFloatValueAdditionAssignment_Generated assignmentGenerated = (EffectContextFloatValueAdditionAssignment_Generated) target2;
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
      a = DefaultDataReadUtility.ReadSerialize<IValueSetter<float>>(reader, "A");
      b = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Source");
    }
  }
}
