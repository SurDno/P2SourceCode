using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Detectors;
using Expressions;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((ConstValue<DetectType>) target2).value = value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "Value");
    }
  }
}
