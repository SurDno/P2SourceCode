using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LessOrEqualIntOperation))]
  public class LessOrEqualIntOperation_Generated : 
    LessOrEqualIntOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LessOrEqualIntOperation_Generated instance = Activator.CreateInstance<LessOrEqualIntOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LessOrEqualIntOperation_Generated operationGenerated = (LessOrEqualIntOperation_Generated) target2;
      operationGenerated.a = CloneableObjectUtility.Clone<IValue<int>>(this.a);
      operationGenerated.b = CloneableObjectUtility.Clone<IValue<int>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<int>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<int>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "Right");
    }
  }
}
