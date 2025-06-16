using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyGroupItemData))]
  public class DifficultyGroupItemData_Generated : 
    DifficultyGroupItemData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyGroupItemData_Generated instance = Activator.CreateInstance<DifficultyGroupItemData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((DifficultyGroupItemData) target2).Name = this.Name;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
    }
  }
}
