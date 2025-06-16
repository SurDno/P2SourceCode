using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyItemData_Generated itemDataGenerated = (DifficultyItemData_Generated) target2;
      itemDataGenerated.Name = this.Name;
      itemDataGenerated.Min = this.Min;
      itemDataGenerated.Max = this.Max;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Min", this.Min);
      DefaultDataWriteUtility.Write(writer, "Max", this.Max);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Min = DefaultDataReadUtility.Read(reader, "Min", this.Min);
      this.Max = DefaultDataReadUtility.Read(reader, "Max", this.Max);
    }
  }
}
