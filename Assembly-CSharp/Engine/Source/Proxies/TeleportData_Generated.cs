using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TeleportData))]
  public class TeleportData_Generated : TeleportData, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveReference(writer, "Location", (object) this.Location);
      CustomStateSaveUtility.SaveReference(writer, "Target", (object) this.Target);
      UnityDataWriteUtility.Write(writer, "Position", this.Position);
      UnityDataWriteUtility.Write(writer, "Rotation", this.Rotation);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Location = CustomStateLoadUtility.LoadReference<ILocationComponent>(reader, "Location", this.Location);
      this.Target = CustomStateLoadUtility.LoadReference<IEntity>(reader, "Target", this.Target);
      this.Position = UnityDataReadUtility.Read(reader, "Position", this.Position);
      this.Rotation = UnityDataReadUtility.Read(reader, "Rotation", this.Rotation);
    }
  }
}
