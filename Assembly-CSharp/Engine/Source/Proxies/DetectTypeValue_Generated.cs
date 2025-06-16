using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DetectTypeValue))]
  public class DetectTypeValue_Generated : 
    DetectTypeValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DetectTypeValue_Generated instance = Activator.CreateInstance<DetectTypeValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ConstValue<DetectType>) target2).value = this.value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "Value");
    }
  }
}
