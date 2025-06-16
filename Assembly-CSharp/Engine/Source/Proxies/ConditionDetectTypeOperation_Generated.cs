using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ConditionDetectTypeOperation))]
  public class ConditionDetectTypeOperation_Generated : 
    ConditionDetectTypeOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ConditionDetectTypeOperation_Generated instance = Activator.CreateInstance<ConditionDetectTypeOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ConditionDetectTypeOperation_Generated operationGenerated = (ConditionDetectTypeOperation_Generated) target2;
      operationGenerated.condition = CloneableObjectUtility.Clone<IValue<bool>>(this.condition);
      operationGenerated.trueResult = CloneableObjectUtility.Clone<IValue<DetectType>>(this.trueResult);
      operationGenerated.falseResult = CloneableObjectUtility.Clone<IValue<DetectType>>(this.falseResult);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<bool>>(writer, "Condition", this.condition);
      DefaultDataWriteUtility.WriteSerialize<IValue<DetectType>>(writer, "True", this.trueResult);
      DefaultDataWriteUtility.WriteSerialize<IValue<DetectType>>(writer, "False", this.falseResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
      this.trueResult = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "True");
      this.falseResult = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "False");
    }
  }
}
