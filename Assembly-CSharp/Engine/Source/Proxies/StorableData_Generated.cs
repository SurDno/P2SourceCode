using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableData))]
  public class StorableData_Generated : StorableData, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveReference(writer, "Storage", (object) this.Storage);
      DefaultDataWriteUtility.Write(writer, "TemplateId", this.TemplateId);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Storage = CustomStateLoadUtility.LoadReference<IStorageComponent>(reader, "Storage", this.Storage);
      this.TemplateId = DefaultDataReadUtility.Read(reader, "TemplateId", this.TemplateId);
    }
  }
}
