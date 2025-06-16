using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyGroupData))]
  public class DifficultyGroupData_Generated : 
    DifficultyGroupData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyGroupData_Generated instance = Activator.CreateInstance<DifficultyGroupData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyGroupData_Generated groupDataGenerated = (DifficultyGroupData_Generated) target2;
      groupDataGenerated.Name = this.Name;
      CloneableObjectUtility.CopyListTo<DifficultyGroupItemData>(groupDataGenerated.Items, this.Items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteListSerialize<DifficultyGroupItemData>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Items = DefaultDataReadUtility.ReadListSerialize<DifficultyGroupItemData>(reader, "Items", this.Items);
    }
  }
}
