using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FloatValue))]
  public class FloatValue_Generated : 
    FloatValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FloatValue_Generated instance = Activator.CreateInstance<FloatValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((ConstValue<float>) target2).value = value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.Read(reader, "Value", value);
    }
  }
}
