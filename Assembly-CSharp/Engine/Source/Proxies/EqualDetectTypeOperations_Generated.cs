using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EqualDetectTypeOperations))]
  public class EqualDetectTypeOperations_Generated : 
    EqualDetectTypeOperations,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EqualDetectTypeOperations_Generated instance = Activator.CreateInstance<EqualDetectTypeOperations_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      EqualDetectTypeOperations_Generated operationsGenerated = (EqualDetectTypeOperations_Generated) target2;
      operationsGenerated.a = CloneableObjectUtility.Clone(a);
      operationsGenerated.b = CloneableObjectUtility.Clone(b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "Left", a);
      DefaultDataWriteUtility.WriteSerialize(writer, "Right", b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      a = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Left");
      b = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Right");
    }
  }
}
