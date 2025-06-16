using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyPresetItemData_Generated itemDataGenerated = (DifficultyPresetItemData_Generated) target2;
      itemDataGenerated.Name = this.Name;
      itemDataGenerated.Value = this.Value;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Value", this.Value);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Value = DefaultDataReadUtility.Read(reader, "Value", this.Value);
    }
  }
}
