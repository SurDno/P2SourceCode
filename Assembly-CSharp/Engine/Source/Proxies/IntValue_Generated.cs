using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (IntValue))]
  public class IntValue_Generated : 
    IntValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      IntValue_Generated instance = Activator.CreateInstance<IntValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ConstValue<int>) target2).value = this.value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.Read(reader, "Value", this.value);
    }
  }
}
