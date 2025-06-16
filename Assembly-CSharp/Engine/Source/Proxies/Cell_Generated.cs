using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Inventory;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (Cell))]
  public class Cell_Generated : 
    Cell,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      Cell_Generated instance = Activator.CreateInstance<Cell_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      Cell_Generated cellGenerated = (Cell_Generated) target2;
      cellGenerated.Column = this.Column;
      cellGenerated.Row = this.Row;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Column", this.Column);
      DefaultDataWriteUtility.Write(writer, "Row", this.Row);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Column = DefaultDataReadUtility.Read(reader, "Column", this.Column);
      this.Row = DefaultDataReadUtility.Read(reader, "Row", this.Row);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Column", this.Column);
      DefaultDataWriteUtility.Write(writer, "Row", this.Row);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Column = DefaultDataReadUtility.Read(reader, "Column", this.Column);
      this.Row = DefaultDataReadUtility.Read(reader, "Row", this.Row);
    }
  }
}
