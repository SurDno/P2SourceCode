using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TeleportData))]
  public class TeleportData_Generated : TeleportData, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveReference(writer, "Location", Location);
      CustomStateSaveUtility.SaveReference(writer, "Target", Target);
      UnityDataWriteUtility.Write(writer, "Position", Position);
      UnityDataWriteUtility.Write(writer, "Rotation", Rotation);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      Location = CustomStateLoadUtility.LoadReference(reader, "Location", Location);
      Target = CustomStateLoadUtility.LoadReference(reader, "Target", Target);
      Position = UnityDataReadUtility.Read(reader, "Position", Position);
      Rotation = UnityDataReadUtility.Read(reader, "Rotation", Rotation);
    }
  }
}
