using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StammKindValue))]
  public class StammKindValue_Generated : 
    StammKindValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      StammKindValue_Generated instance = Activator.CreateInstance<StammKindValue_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((StammKindValue_Generated) target2).value = value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Value", value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      value = DefaultDataReadUtility.ReadEnum<StammKind>(reader, "Value");
    }
  }
}
