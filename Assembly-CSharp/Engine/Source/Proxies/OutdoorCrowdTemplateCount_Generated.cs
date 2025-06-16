// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.OutdoorCrowdTemplateCount_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdTemplateCount))]
  public class OutdoorCrowdTemplateCount_Generated : 
    OutdoorCrowdTemplateCount,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdTemplateCount_Generated instance = Activator.CreateInstance<OutdoorCrowdTemplateCount_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdTemplateCount_Generated templateCountGenerated = (OutdoorCrowdTemplateCount_Generated) target2;
      templateCountGenerated.Min = this.Min;
      templateCountGenerated.Max = this.Max;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Min", this.Min);
      DefaultDataWriteUtility.Write(writer, "Max", this.Max);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Min = DefaultDataReadUtility.Read(reader, "Min", this.Min);
      this.Max = DefaultDataReadUtility.Read(reader, "Max", this.Max);
    }
  }
}
