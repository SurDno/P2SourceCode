// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.AssetDatabaseMapItemData_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.AssetDatabaseService;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AssetDatabaseMapItemData))]
  public class AssetDatabaseMapItemData_Generated : 
    AssetDatabaseMapItemData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AssetDatabaseMapItemData_Generated instance = Activator.CreateInstance<AssetDatabaseMapItemData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AssetDatabaseMapItemData_Generated itemDataGenerated = (AssetDatabaseMapItemData_Generated) target2;
      itemDataGenerated.Id = this.Id;
      itemDataGenerated.Name = this.Name;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.Id);
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Id = DefaultDataReadUtility.Read(reader, "Id", this.Id);
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
    }
  }
}
