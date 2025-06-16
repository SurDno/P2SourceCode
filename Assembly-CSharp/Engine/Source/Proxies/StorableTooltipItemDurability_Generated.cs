using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using System;

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
