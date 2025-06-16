using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LessDetectTypeOperation))]
  public class LessDetectTypeOperation_Generated : 
    LessDetectTypeOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LessDetectTypeOperation_Generated instance = Activator.CreateInstance<LessDetectTypeOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LessDetectTypeOperation_Generated operationGenerated = (LessDetectTypeOperation_Generated) target2;
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
