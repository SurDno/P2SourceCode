using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BlockTypeValue))]
  public class BlockTypeValue_Generated : 
    BlockTypeValue,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      BlockTypeValue_Generated instance = Activator.CreateInstance<BlockTypeValue_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ConstValue<BlockTypeEnum>) target2).value = this.value;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<BlockTypeEnum>(writer, "Value", this.value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.value = DefaultDataReadUtility.ReadEnum<BlockTypeEnum>(reader, "Value");
    }
  }
}
