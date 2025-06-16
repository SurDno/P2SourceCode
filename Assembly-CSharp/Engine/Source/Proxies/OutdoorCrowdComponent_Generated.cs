// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.OutdoorCrowdComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Crowds;
using Engine.Source.Components;
using Engine.Source.OutdoorCrowds;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdComponent))]
  public class OutdoorCrowdComponent_Generated : 
    OutdoorCrowdComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      OutdoorCrowdComponent_Generated instance = Activator.CreateInstance<OutdoorCrowdComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((OutdoorCrowdComponent) target2).data = this.data;

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<OutdoorCrowdData>(writer, "Data", this.data);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.data = UnityDataReadUtility.Read<OutdoorCrowdData>(reader, "Data", this.data);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<OutdoorCrowdLayoutEnum>(writer, "Layout", this.layout);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.layout = DefaultDataReadUtility.ReadEnum<OutdoorCrowdLayoutEnum>(reader, "Layout");
    }
  }
}
