﻿using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (GreaterFloatOperation))]
  public class GreaterFloatOperation_Generated : 
    GreaterFloatOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      GreaterFloatOperation_Generated instance = Activator.CreateInstance<GreaterFloatOperation_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      GreaterFloatOperation_Generated operationGenerated = (GreaterFloatOperation_Generated) target2;
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
      a = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Left");
      b = DefaultDataReadUtility.ReadSerialize<IValue<float>>(reader, "Right");
    }
  }
}
