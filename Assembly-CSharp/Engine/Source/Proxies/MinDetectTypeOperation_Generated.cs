using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      MinDetectTypeOperation_Generated operationGenerated = (MinDetectTypeOperation_Generated) target2;
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
