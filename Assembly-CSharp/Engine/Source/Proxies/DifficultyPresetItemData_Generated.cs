using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyPresetItemData))]
  public class DifficultyPresetItemData_Generated : 
    DifficultyPresetItemData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyPresetItemData_Generated instance = Activator.CreateInstance<DifficultyPresetItemData_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyPresetItemData_Generated itemDataGenerated = (DifficultyPresetItemData_Generated) target2;
      itemDataGenerated.Name = Name;
      itemDataGenerated.Value = Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", Name);
      DefaultDataWriteUtility.Write(writer, "Value", Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Name = DefaultDataReadUtility.Read(reader, "Name", Name);
      Value = DefaultDataReadUtility.Read(reader, "Value", Value);
    }
  }
}
