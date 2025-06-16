using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.AssetDatabaseService;
using System;

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
