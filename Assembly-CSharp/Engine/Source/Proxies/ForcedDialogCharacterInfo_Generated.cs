using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using Scripts.Tools.Serializations.Converters;
using System;

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
      DefaultDataWriteUtility.Write(writer, "Distance", this.Distance);
      CustomStateSaveUtility.SaveReference(writer, "Character", (object) this.Character);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Distance = DefaultDataReadUtility.Read(reader, "Distance", this.Distance);
      this.Character = CustomStateLoadUtility.LoadReference<IEntity>(reader, "Character", this.Character);
    }
  }
}
