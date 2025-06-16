using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyPresetData))]
  public class DifficultyPresetData_Generated : 
    DifficultyPresetData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyPresetData_Generated instance = Activator.CreateInstance<DifficultyPresetData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyPresetData_Generated presetDataGenerated = (DifficultyPresetData_Generated) target2;
      presetDataGenerated.Name = this.Name;
      CloneableObjectUtility.CopyListTo<DifficultyPresetItemData>(presetDataGenerated.Items, this.Items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteListSerialize<DifficultyPresetItemData>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Items = DefaultDataReadUtility.ReadListSerialize<DifficultyPresetItemData>(reader, "Items", this.Items);
    }
  }
}
