using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LessOrEqualDetectTypeOperation))]
  public class LessOrEqualDetectTypeOperation_Generated : 
    LessOrEqualDetectTypeOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LessOrEqualDetectTypeOperation_Generated instance = Activator.CreateInstance<LessOrEqualDetectTypeOperation_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LessOrEqualDetectTypeOperation_Generated operationGenerated = (LessOrEqualDetectTypeOperation_Generated) target2;
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
