using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (EqualBoolOperations))]
  public class EqualBoolOperations_Generated : 
    EqualBoolOperations,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      EqualBoolOperations_Generated instance = Activator.CreateInstance<EqualBoolOperations_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      EqualBoolOperations_Generated operationsGenerated = (EqualBoolOperations_Generated) target2;
      operationsGenerated.a = CloneableObjectUtility.Clone<IValue<bool>>(this.a);
      operationsGenerated.b = CloneableObjectUtility.Clone<IValue<bool>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Right");
    }
  }
}
