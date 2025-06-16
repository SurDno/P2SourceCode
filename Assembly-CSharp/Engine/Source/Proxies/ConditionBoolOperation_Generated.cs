using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ConditionBoolOperation))]
  public class ConditionBoolOperation_Generated : 
    ConditionBoolOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ConditionBoolOperation_Generated instance = Activator.CreateInstance<ConditionBoolOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ConditionBoolOperation_Generated operationGenerated = (ConditionBoolOperation_Generated) target2;
      operationGenerated.condition = CloneableObjectUtility.Clone<IValue<bool>>(this.condition);
      operationGenerated.trueResult = CloneableObjectUtility.Clone<IValue<bool>>(this.trueResult);
      operationGenerated.falseResult = CloneableObjectUtility.Clone<IValue<bool>>(this.falseResult);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Condition", this.condition);
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "True", this.trueResult);
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "False", this.falseResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
      this.trueResult = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "True");
      this.falseResult = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "False");
    }
  }
}
