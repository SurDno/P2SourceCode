using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.AssetDatabaseService;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo(((AssetDatabaseMapData) target2).Items, Items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize(writer, "Items", Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", Items);
    }
  }
}
