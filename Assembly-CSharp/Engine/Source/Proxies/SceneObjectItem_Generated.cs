using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Assets.Internal.Reference;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SceneObjectItem))]
  public class SceneObjectItem_Generated : SceneObjectItem, ISerializeDataWrite, ISerializeDataRead
  {
    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.Id);
      DefaultDataWriteUtility.Write(writer, "PreserveName", this.PreserveName);
      UnityDataWriteUtility.Write<IEntity>(writer, "Origin", this.Origin);
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.Template);
      DefaultDataWriteUtility.WriteListSerialize<SceneObjectItem>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Id = DefaultDataReadUtility.Read(reader, "Id", this.Id);
      this.PreserveName = DefaultDataReadUtility.Read(reader, "PreserveName", this.PreserveName);
      this.Origin = UnityDataReadUtility.Read<IEntity>(reader, "Origin", this.Origin);
      this.Template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.Template);
      this.Items = DefaultDataReadUtility.ReadListSerialize<SceneObjectItem>(reader, "Items", this.Items);
    }
  }
}
