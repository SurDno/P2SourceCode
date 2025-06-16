using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (GreaterDetectTypeOperation))]
  public class GreaterDetectTypeOperation_Generated : 
    GreaterDetectTypeOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      GreaterDetectTypeOperation_Generated instance = Activator.CreateInstance<GreaterDetectTypeOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      GreaterDetectTypeOperation_Generated operationGenerated = (GreaterDetectTypeOperation_Generated) target2;
      operationGenerated.a = CloneableObjectUtility.Clone<IValue<DetectType>>(this.a);
      operationGenerated.b = CloneableObjectUtility.Clone<IValue<DetectType>>(this.b);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<IValue<DetectType>>(writer, "Left", this.a);
      DefaultDataWriteUtility.WriteSerialize<IValue<DetectType>>(writer, "Right", this.b);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.a = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Left");
      this.b = DefaultDataReadUtility.ReadSerialize<IValue<DetectType>>(reader, "Right");
    }
  }
}
