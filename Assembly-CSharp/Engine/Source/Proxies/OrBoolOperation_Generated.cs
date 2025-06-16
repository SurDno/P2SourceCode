using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OrBoolOperation))]
  public class OrBoolOperation_Generated : 
    OrBoolOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OrBoolOperation_Generated instance = Activator.CreateInstance<OrBoolOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OrBoolOperation_Generated operationGenerated = (OrBoolOperation_Generated) target2;
      operationGenerated.a = CloneableObjectUtility.Clone<IValue<bool>>(this.a);
      operationGenerated.b = CloneableObjectUtility.Clone<IValue<bool>>(this.b);
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
