// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.TeleportData_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components.Saves;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
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
