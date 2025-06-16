using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ConditionFloatOperation))]
  public class ConditionFloatOperation_Generated : 
    ConditionFloatOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ConditionFloatOperation_Generated instance = Activator.CreateInstance<ConditionFloatOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ConditionFloatOperation_Generated operationGenerated = (ConditionFloatOperation_Generated) target2;
      operationGenerated.condition = CloneableObjectUtility.Clone<IValue<bool>>(this.condition);
      operationGenerated.trueResult = CloneableObjectUtility.Clone<IValue<float>>(this.trueResult);
      operationGenerated.falseResult = CloneableObjectUtility.Clone<IValue<float>>(this.falseResult);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Condition", this.condition);
      DefaultDataWriteUtility.WriteSerialize<IValue<float>>(writer, "True", this.trueResult);
      DefaultDataWriteUtility.WriteSerialize<IValue<float>>(writer, "False", this.falseResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
      this.trueResult = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "True");
      this.falseResult = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "False");
    }
  }
}
