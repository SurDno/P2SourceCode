using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Crowds;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (HerbRootsComponent))]
  public class HerbRootsComponent_Generated : 
    HerbRootsComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      HerbRootsComponent_Generated instance = Activator.CreateInstance<HerbRootsComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      HerbRootsComponent_Generated componentGenerated = (HerbRootsComponent_Generated) target2;
      CloneableObjectUtility.CopyListTo<HerbRootsTemplate>(componentGenerated.templates, this.templates);
      componentGenerated.herbsBudget = this.herbsBudget;
      componentGenerated.herbsCountMin = this.herbsCountMin;
      componentGenerated.herbsCountMax = this.herbsCountMax;
      componentGenerated.HerbsGrowTimeInMinutesMin = this.HerbsGrowTimeInMinutesMin;
      componentGenerated.herbsGrowTimeInMinutesMax = this.herbsGrowTimeInMinutesMax;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<HerbRootsTemplate>(writer, "Templates", this.templates);
      DefaultDataWriteUtility.Write(writer, "HerbsBudget", this.herbsBudget);
      DefaultDataWriteUtility.Write(writer, "HerbsCountMin", this.herbsCountMin);
      DefaultDataWriteUtility.Write(writer, "HerbsCountMax", this.herbsCountMax);
      DefaultDataWriteUtility.Write(writer, "HerbsGrowTimeInMinutesMin", this.HerbsGrowTimeInMinutesMin);
      DefaultDataWriteUtility.Write(writer, "HerbsGrowTimeInMinutesMax", this.herbsGrowTimeInMinutesMax);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.templates = DefaultDataReadUtility.ReadListSerialize<HerbRootsTemplate>(reader, "Templates", this.templates);
      this.herbsBudget = DefaultDataReadUtility.Read(reader, "HerbsBudget", this.herbsBudget);
      this.herbsCountMin = DefaultDataReadUtility.Read(reader, "HerbsCountMin", this.herbsCountMin);
      this.herbsCountMax = DefaultDataReadUtility.Read(reader, "HerbsCountMax", this.herbsCountMax);
      this.HerbsGrowTimeInMinutesMin = DefaultDataReadUtility.Read(reader, "HerbsGrowTimeInMinutesMin", this.HerbsGrowTimeInMinutesMin);
      this.herbsGrowTimeInMinutesMax = DefaultDataReadUtility.Read(reader, "HerbsGrowTimeInMinutesMax", this.herbsGrowTimeInMinutesMax);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<HerbRootsComponentStateEnum>(writer, "State", this.state);
      DefaultDataWriteUtility.WriteList(writer, "HerbGrowTimesLeftSorted", this.herbGrowTimesLeftSorted);
      DefaultDataWriteUtility.Write(writer, "GrownHerbsCount", this.grownHerbsCount);
      DefaultDataWriteUtility.Write(writer, "CurrentHerbsCount", this.currentHerbsCount);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.state = DefaultDataReadUtility.ReadEnum<HerbRootsComponentStateEnum>(reader, "State");
      this.herbGrowTimesLeftSorted = DefaultDataReadUtility.ReadList(reader, "HerbGrowTimesLeftSorted", this.herbGrowTimesLeftSorted);
      this.grownHerbsCount = DefaultDataReadUtility.Read(reader, "GrownHerbsCount", this.grownHerbsCount);
      this.currentHerbsCount = DefaultDataReadUtility.Read(reader, "CurrentHerbsCount", this.currentHerbsCount);
    }
  }
}
