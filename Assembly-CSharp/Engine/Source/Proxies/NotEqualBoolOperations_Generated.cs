using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NotEqualBoolOperations))]
  public class NotEqualBoolOperations_Generated : 
    NotEqualBoolOperations,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NotEqualBoolOperations_Generated instance = Activator.CreateInstance<NotEqualBoolOperations_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      NotEqualBoolOperations_Generated operationsGenerated = (NotEqualBoolOperations_Generated) target2;
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
      a = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Left");
      b = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Right");
    }
  }
}
