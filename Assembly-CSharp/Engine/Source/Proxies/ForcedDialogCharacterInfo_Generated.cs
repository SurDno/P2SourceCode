using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ForcedDialogCharacterInfo))]
  public class ForcedDialogCharacterInfo_Generated : 
    ForcedDialogCharacterInfo,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Distance", Distance);
      CustomStateSaveUtility.SaveReference(writer, "Character", Character);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      Distance = DefaultDataReadUtility.Read(reader, "Distance", Distance);
      Character = CustomStateLoadUtility.LoadReference(reader, "Character", Character);
    }
  }
}
