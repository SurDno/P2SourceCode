using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (NotEqualStammKindOperations))]
  public class NotEqualStammKindOperations_Generated : 
    NotEqualStammKindOperations,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      NotEqualStammKindOperations_Generated instance = Activator.CreateInstance<NotEqualStammKindOperations_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      NotEqualStammKindOperations_Generated operationsGenerated = (NotEqualStammKindOperations_Generated) target2;
      operationsGenerated.a = CloneableObjectUtility.Clone<IValue<StammKind>>(this.a);
      operationsGenerated.b = CloneableObjectUtility.Clone<IValue<StammKind>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<StammKind>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<StammKind>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<StammKind>>(reader, "Right");
    }
  }
}
