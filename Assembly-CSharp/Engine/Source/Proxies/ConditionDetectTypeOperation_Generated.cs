﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ConditionDetectTypeOperation_Generated operationGenerated = (ConditionDetectTypeOperation_Generated) target2;
      operationGenerated.condition = CloneableObjectUtility.Clone(condition);
      operationGenerated.trueResult = CloneableObjectUtility.Clone(trueResult);
      operationGenerated.falseResult = CloneableObjectUtility.Clone(falseResult);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "Condition", condition);
      DefaultDataWriteUtility.WriteSerialize(writer, "True", trueResult);
      DefaultDataWriteUtility.WriteSerialize(writer, "False", falseResult);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      condition = DefaultDataReadUtility.ReadSerialize<IValue<bool>>(reader, "Condition");
      trueResult = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "True");
      falseResult = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "False");
    }
  }
}
