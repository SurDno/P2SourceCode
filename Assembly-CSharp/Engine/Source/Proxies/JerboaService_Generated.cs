// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.JerboaService_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Impl.Services;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (JerboaService))]
  public class JerboaService_Generated : JerboaService, ISerializeStateSave, ISerializeStateLoad
  {
    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Amount", this.Amount);
      DefaultDataWriteUtility.WriteEnum<JerboaColorEnum>(writer, "Color", this.Color);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Amount = DefaultDataReadUtility.Read(reader, "Amount", this.Amount);
      this.Color = DefaultDataReadUtility.ReadEnum<JerboaColorEnum>(reader, "Color");
    }
  }
}
