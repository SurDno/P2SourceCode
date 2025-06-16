using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Storable;
using Engine.Source.Components;
using Engine.Source.Components.Saves;
using Engine.Source.Inventory;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorableComponent))]
  public class StorableComponent_Generated : 
    StorableComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      StorableComponent_Generated instance = Activator.CreateInstance<StorableComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StorableComponent_Generated componentGenerated = (StorableComponent_Generated) target2;
      componentGenerated.isEnabled = this.isEnabled;
      componentGenerated.placeholder = this.placeholder;
      CloneableObjectUtility.FillListTo<StorableGroup>(componentGenerated.groups, this.groups);
      CloneableObjectUtility.CopyListTo<IStorableTooltipComponent>(componentGenerated.tooltips, this.tooltips);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<IInventoryPlaceholder>(writer, "Placeholder", this.placeholder);
      DefaultDataWriteUtility.WriteListEnum<StorableGroup>(writer, "Groups", this.groups);
      DefaultDataWriteUtility.WriteListSerialize<IStorableTooltipComponent>(writer, "Tooltips", this.tooltips);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.placeholder = UnityDataReadUtility.Read<IInventoryPlaceholder>(reader, "Placeholder", this.placeholder);
      this.groups = DefaultDataReadUtility.ReadListEnum<StorableGroup>(reader, "Groups", this.groups);
      this.tooltips = DefaultDataReadUtility.ReadListSerialize<IStorableTooltipComponent>(reader, "Tooltips", this.tooltips);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultStateSaveUtility.SaveSerialize<Cell>(writer, "Cell", this.Cell);
      EngineDataWriteUtility.Write(writer, "Title", this.Title);
      EngineDataWriteUtility.Write(writer, "Tooltip", this.Tooltip);
      EngineDataWriteUtility.Write(writer, "Description", this.Description);
      EngineDataWriteUtility.Write(writer, "SpecialDescription", this.SpecialDescription);
      DefaultStateSaveUtility.SaveSerialize<StorableData>(writer, "StorableData", this.StorableData);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      DefaultDataWriteUtility.Write(writer, "Count", this.count);
      DefaultDataWriteUtility.Write(writer, "Max", this.max);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.Cell = DefaultStateLoadUtility.ReadSerialize<Cell>(reader, "Cell");
      this.Title = EngineDataReadUtility.Read(reader, "Title", this.Title);
      this.Tooltip = EngineDataReadUtility.Read(reader, "Tooltip", this.Tooltip);
      this.Description = EngineDataReadUtility.Read(reader, "Description", this.Description);
      this.SpecialDescription = EngineDataReadUtility.Read(reader, "SpecialDescription", this.SpecialDescription);
      this.StorableData = DefaultStateLoadUtility.ReadSerialize<StorableData>(reader, "StorableData");
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.count = DefaultDataReadUtility.Read(reader, "Count", this.count);
      this.max = DefaultDataReadUtility.Read(reader, "Max", this.max);
    }
  }
}
