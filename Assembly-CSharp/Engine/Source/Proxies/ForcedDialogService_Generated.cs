using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ForcedDialogService))]
  public class ForcedDialogService_Generated : 
    ForcedDialogService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveListSerialize(writer, "Characters", characters);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      characters = DefaultStateLoadUtility.ReadListSerialize(reader, "Characters", characters);
    }
  }
}
