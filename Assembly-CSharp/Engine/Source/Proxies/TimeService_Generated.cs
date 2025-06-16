// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.TimeService_Generated
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
  [FactoryProxy(typeof (TimeService))]
  public class TimeService_Generated : TimeService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "AbsoluteGameTime", this.AbsoluteGameTime);
      DefaultDataWriteUtility.Write(writer, "RealTime", this.RealTime);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.AbsoluteGameTime = DefaultDataReadUtility.Read(reader, "AbsoluteGameTime", this.AbsoluteGameTime);
      this.RealTime = DefaultDataReadUtility.Read(reader, "RealTime", this.RealTime);
    }
  }
}
