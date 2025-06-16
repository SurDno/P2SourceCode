// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.Cell_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Inventory;
using System;

#nullable disable
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
