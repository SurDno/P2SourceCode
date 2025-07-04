﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (GreaterOrEqualTimeSpanOperation))]
  public class GreaterOrEqualTimeSpanOperation_Generated : 
    GreaterOrEqualTimeSpanOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      GreaterOrEqualTimeSpanOperation_Generated instance = Activator.CreateInstance<GreaterOrEqualTimeSpanOperation_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      GreaterOrEqualTimeSpanOperation_Generated operationGenerated = (GreaterOrEqualTimeSpanOperation_Generated) target2;
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
      a = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "Left");
      b = DefaultDataReadUtility.ReadSerialize<IValue<TimeSpan>>(reader, "Right");
    }
  }
}
