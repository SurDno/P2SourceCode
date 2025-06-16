using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ConditionIntOperation))]
  public class ConditionIntOperation_Generated : 
    ConditionIntOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ConditionIntOperation_Generated instance = Activator.CreateInstance<ConditionIntOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ConditionIntOperation_Generated operationGenerated = (ConditionIntOperation_Generated) target2;
      operationGenerated.condition = CloneableObjectUtility.Clone<IValue<bool>>(this.condition);
      operationGenerated.trueResult = CloneableObjectUtility.Clone<IValue<int>>(this.trueResult);
      operationGenerated.falseResult = CloneableObjectUtility.Clone<IValue<int>>(this.falseResult);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Condition", this.condition);
      DefaultDataWriteUtility.WriteSerialize<IValue<int>>(writer, "True", this.trueResult);
      DefaultDataWriteUtility.WriteSerialize<IValue<int>>(writer, "False", this.falseResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
      this.trueResult = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "True");
      this.falseResult = DefaultDataReadUtility.ReadSerialize<IValue<int>>(reader, "False");
    }
  }
}
