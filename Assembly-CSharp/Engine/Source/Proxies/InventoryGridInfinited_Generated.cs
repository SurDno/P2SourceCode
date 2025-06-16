// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.InventoryGridInfinited_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Inventory;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InventoryGridInfinited))]
  public class InventoryGridInfinited_Generated : 
    InventoryGridInfinited,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<InventoryGridInfinited_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      InventoryGridInfinited_Generated infinitedGenerated = (InventoryGridInfinited_Generated) target2;
      infinitedGenerated.name = this.name;
      infinitedGenerated.columns = this.columns;
      infinitedGenerated.rows = this.rows;
      infinitedGenerated.direction = this.direction;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "Columns", this.columns);
      DefaultDataWriteUtility.Write(writer, "Rows", this.rows);
      DefaultDataWriteUtility.WriteEnum<DirectionKind>(writer, "Direction", this.direction);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.columns = DefaultDataReadUtility.Read(reader, "Columns", this.columns);
      this.rows = DefaultDataReadUtility.Read(reader, "Rows", this.rows);
      this.direction = DefaultDataReadUtility.ReadEnum<DirectionKind>(reader, "Direction");
    }
  }
}
