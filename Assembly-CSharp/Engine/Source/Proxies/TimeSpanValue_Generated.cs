using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TimeSpanValue))]
  public class TimeSpanValue_Generated : 
    TimeSpanValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      TimeSpanValue_Generated instance = Activator.CreateInstance<TimeSpanValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ConstValue<TimeSpan>) target2).value = this.value;

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
