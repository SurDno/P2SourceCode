// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.OutdoorCrowdState_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdState))]
  public class OutdoorCrowdState_Generated : 
    OutdoorCrowdState,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdState_Generated instance = Activator.CreateInstance<OutdoorCrowdState_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdState_Generated crowdStateGenerated = (OutdoorCrowdState_Generated) target2;
      crowdStateGenerated.State = this.State;
      CloneableObjectUtility.CopyListTo<OutdoorCrowdTemplateLink>(crowdStateGenerated.TemplateLinks, this.TemplateLinks);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<DiseasedStateEnum>(writer, "State", this.State);
      DefaultDataWriteUtility.WriteListSerialize<OutdoorCrowdTemplateLink>(writer, "TemplateLinks", this.TemplateLinks);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.State = DefaultDataReadUtility.ReadEnum<DiseasedStateEnum>(reader, "State");
      this.TemplateLinks = DefaultDataReadUtility.ReadListSerialize<OutdoorCrowdTemplateLink>(reader, "TemplateLinks", this.TemplateLinks);
    }
  }
}
