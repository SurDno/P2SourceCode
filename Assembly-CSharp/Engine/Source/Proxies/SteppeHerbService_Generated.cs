// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SteppeHerbService_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Impl.Services;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SteppeHerbService))]
  public class SteppeHerbService_Generated : 
    SteppeHerbService,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "BrownTwyreAmount", this.BrownTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "BloodTwyreAmount", this.BloodTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "BlackTwyreAmount", this.BlackTwyreAmount);
      DefaultDataWriteUtility.Write(writer, "SweveryAmount", this.SweveryAmount);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.BrownTwyreAmount = DefaultDataReadUtility.Read(reader, "BrownTwyreAmount", this.BrownTwyreAmount);
      this.BloodTwyreAmount = DefaultDataReadUtility.Read(reader, "BloodTwyreAmount", this.BloodTwyreAmount);
      this.BlackTwyreAmount = DefaultDataReadUtility.Read(reader, "BlackTwyreAmount", this.BlackTwyreAmount);
      this.SweveryAmount = DefaultDataReadUtility.Read(reader, "SweveryAmount", this.SweveryAmount);
    }
  }
}
