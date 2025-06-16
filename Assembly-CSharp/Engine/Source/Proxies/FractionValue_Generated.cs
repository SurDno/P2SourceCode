using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (FractionValue))]
  public class FractionValue_Generated : 
    FractionValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      FractionValue_Generated instance = Activator.CreateInstance<FractionValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ConstValue<FractionEnum>) target2).value = this.value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<FractionEnum>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<FractionEnum>(reader, "Value");
    }
  }
}
