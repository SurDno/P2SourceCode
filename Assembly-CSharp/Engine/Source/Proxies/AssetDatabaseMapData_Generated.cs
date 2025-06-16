// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.AssetDatabaseMapData_Generated
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
  [FactoryProxy(typeof (AssetDatabaseMapData))]
  public class AssetDatabaseMapData_Generated : 
    AssetDatabaseMapData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AssetDatabaseMapData_Generated instance = Activator.CreateInstance<AssetDatabaseMapData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<AssetDatabaseMapItemData>(((AssetDatabaseMapData) target2).Items, this.Items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<AssetDatabaseMapItemData>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Items = DefaultDataReadUtility.ReadListSerialize<AssetDatabaseMapItemData>(reader, "Items", this.Items);
    }
  }
}
