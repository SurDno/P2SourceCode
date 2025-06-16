// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.StorableTooltipItemDurability_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableTooltipItemDurability))]
  public class StorableTooltipItemDurability_Generated : 
    StorableTooltipItemDurability,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      StorableTooltipItemDurability_Generated instance = Activator.CreateInstance<StorableTooltipItemDurability_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StorableTooltipItemDurability_Generated durabilityGenerated = (StorableTooltipItemDurability_Generated) target2;
      durabilityGenerated.isEnabled = this.isEnabled;
      durabilityGenerated.name = this.name;
      durabilityGenerated.parameter = this.parameter;
      durabilityGenerated.isFood = this.isFood;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      DefaultDataWriteUtility.WriteEnum<StorableTooltipNameEnum>(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterNameEnum>(writer, "Parameter", this.parameter);
      DefaultDataWriteUtility.Write(writer, "IsFood", this.isFood);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.name = DefaultDataReadUtility.ReadEnum<StorableTooltipNameEnum>(reader, "Name");
      this.parameter = DefaultDataReadUtility.ReadEnum<ParameterNameEnum>(reader, "Parameter");
      this.isFood = DefaultDataReadUtility.Read(reader, "IsFood", this.isFood);
    }
  }
}
