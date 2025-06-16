using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Assets.Internal.Reference;
using Engine.Common.Commons.Converters;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SceneObjectItem))]
  public class SceneObjectItem_Generated : SceneObjectItem, ISerializeDataWrite, ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", Id);
      DefaultDataWriteUtility.Write(writer, "PreserveName", PreserveName);
      UnityDataWriteUtility.Write(writer, "Origin", Origin);
      UnityDataWriteUtility.Write(writer, "Template", Template);
      DefaultDataWriteUtility.WriteListSerialize(writer, "Items", Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Id = DefaultDataReadUtility.Read(reader, "Id", Id);
      PreserveName = DefaultDataReadUtility.Read(reader, "PreserveName", PreserveName);
      Origin = UnityDataReadUtility.Read(reader, "Origin", Origin);
      Template = UnityDataReadUtility.Read(reader, "Template", Template);
      Items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", Items);
    }
  }
}
