// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.StorableTooltipInfo_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableTooltipInfo))]
  public class StorableTooltipInfo_Generated : 
    StorableTooltipInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      StorableTooltipInfo_Generated instance = Activator.CreateInstance<StorableTooltipInfo_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StorableTooltipInfo_Generated tooltipInfoGenerated = (StorableTooltipInfo_Generated) target2;
      tooltipInfoGenerated.Name = this.Name;
      tooltipInfoGenerated.Value = this.Value;
      tooltipInfoGenerated.Color = this.Color;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<StorableTooltipNameEnum>(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Value", this.Value);
      UnityDataWriteUtility.Write(writer, "Color", this.Color);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.ReadEnum<StorableTooltipNameEnum>(reader, "Name");
      this.Value = DefaultDataReadUtility.Read(reader, "Value", this.Value);
      this.Color = UnityDataReadUtility.Read(reader, "Color", this.Color);
    }
  }
}
