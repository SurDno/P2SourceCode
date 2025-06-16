// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.MapService_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Source.Services;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MapService))]
  public class MapService_Generated : MapService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "BullModeAvailable", this.BullModeAvailable);
      DefaultDataWriteUtility.Write(writer, "BullModeForced", this.BullModeForced);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.BullModeAvailable = DefaultDataReadUtility.Read(reader, "BullModeAvailable", this.BullModeAvailable);
      this.BullModeForced = DefaultDataReadUtility.Read(reader, "BullModeForced", this.BullModeForced);
    }
  }
}
