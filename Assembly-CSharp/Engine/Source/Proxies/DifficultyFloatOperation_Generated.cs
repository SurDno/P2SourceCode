using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Expressions;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyFloatOperation))]
  public class DifficultyFloatOperation_Generated : 
    DifficultyFloatOperation,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyFloatOperation_Generated instance = Activator.CreateInstance<DifficultyFloatOperation_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2) => ((DifficultyFloatOperation) target2).name = name;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", name);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      name = DefaultDataReadUtility.Read(reader, "Name", name);
    }
  }
}
