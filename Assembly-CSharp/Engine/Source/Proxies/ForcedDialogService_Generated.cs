using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

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
      DefaultStateSaveUtility.SaveListSerialize<ForcedDialogCharacterInfo>(writer, "Characters", this.characters);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.characters = DefaultStateLoadUtility.ReadListSerialize<ForcedDialogCharacterInfo>(reader, "Characters", this.characters);
    }
  }
}
