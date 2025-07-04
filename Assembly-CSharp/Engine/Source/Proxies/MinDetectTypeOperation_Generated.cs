﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MinDetectTypeOperation))]
  public class MinDetectTypeOperation_Generated : 
    MinDetectTypeOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      MinDetectTypeOperation_Generated instance = Activator.CreateInstance<MinDetectTypeOperation_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      MinDetectTypeOperation_Generated operationGenerated = (MinDetectTypeOperation_Generated) target2;
      operationGenerated.a = CloneableObjectUtility.Clone(a);
      operationGenerated.b = CloneableObjectUtility.Clone(b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "Left", a);
      DefaultDataWriteUtility.WriteSerialize(writer, "Right", b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      a = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Left");
      b = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Right");
    }
  }
}
