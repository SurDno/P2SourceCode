using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyItemData))]
  public class DifficultyItemData_Generated : 
    DifficultyItemData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyItemData_Generated instance = Activator.CreateInstance<DifficultyItemData_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyItemData_Generated itemDataGenerated = (DifficultyItemData_Generated) target2;
      itemDataGenerated.Name = Name;
      itemDataGenerated.Min = Min;
      itemDataGenerated.Max = Max;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.Write(writer, "Min", Min);
      DefaultDataWriteUtility.Write(writer, "Max", Max);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.Read(reader, "Name", Name);
      Min = DefaultDataReadUtility.Read(reader, "Min", Min);
      Max = DefaultDataReadUtility.Read(reader, "Max", Max);
    }
  }
}
