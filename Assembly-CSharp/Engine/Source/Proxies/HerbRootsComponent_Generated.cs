using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Components.Crowds;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(HerbRootsComponent))]
public class HerbRootsComponent_Generated :
	HerbRootsComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<HerbRootsComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var componentGenerated = (HerbRootsComponent_Generated)target2;
		CloneableObjectUtility.CopyListTo(componentGenerated.templates, templates);
		componentGenerated.herbsBudget = herbsBudget;
		componentGenerated.herbsCountMin = herbsCountMin;
		componentGenerated.herbsCountMax = herbsCountMax;
		componentGenerated.HerbsGrowTimeInMinutesMin = HerbsGrowTimeInMinutesMin;
		componentGenerated.herbsGrowTimeInMinutesMax = herbsGrowTimeInMinutesMax;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteListSerialize(writer, "Templates", templates);
		DefaultDataWriteUtility.Write(writer, "HerbsBudget", herbsBudget);
		DefaultDataWriteUtility.Write(writer, "HerbsCountMin", herbsCountMin);
		DefaultDataWriteUtility.Write(writer, "HerbsCountMax", herbsCountMax);
		DefaultDataWriteUtility.Write(writer, "HerbsGrowTimeInMinutesMin", HerbsGrowTimeInMinutesMin);
		DefaultDataWriteUtility.Write(writer, "HerbsGrowTimeInMinutesMax", herbsGrowTimeInMinutesMax);
	}

	public void DataRead(IDataReader reader, Type type) {
		templates = DefaultDataReadUtility.ReadListSerialize(reader, "Templates", templates);
		herbsBudget = DefaultDataReadUtility.Read(reader, "HerbsBudget", herbsBudget);
		herbsCountMin = DefaultDataReadUtility.Read(reader, "HerbsCountMin", herbsCountMin);
		herbsCountMax = DefaultDataReadUtility.Read(reader, "HerbsCountMax", herbsCountMax);
		HerbsGrowTimeInMinutesMin =
			DefaultDataReadUtility.Read(reader, "HerbsGrowTimeInMinutesMin", HerbsGrowTimeInMinutesMin);
		herbsGrowTimeInMinutesMax =
			DefaultDataReadUtility.Read(reader, "HerbsGrowTimeInMinutesMax", herbsGrowTimeInMinutesMax);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "State", state);
		DefaultDataWriteUtility.WriteList(writer, "HerbGrowTimesLeftSorted", herbGrowTimesLeftSorted);
		DefaultDataWriteUtility.Write(writer, "GrownHerbsCount", grownHerbsCount);
		DefaultDataWriteUtility.Write(writer, "CurrentHerbsCount", currentHerbsCount);
	}

	public void StateLoad(IDataReader reader, Type type) {
		state = DefaultDataReadUtility.ReadEnum<HerbRootsComponentStateEnum>(reader, "State");
		herbGrowTimesLeftSorted =
			DefaultDataReadUtility.ReadList(reader, "HerbGrowTimesLeftSorted", herbGrowTimesLeftSorted);
		grownHerbsCount = DefaultDataReadUtility.Read(reader, "GrownHerbsCount", grownHerbsCount);
		currentHerbsCount = DefaultDataReadUtility.Read(reader, "CurrentHerbsCount", currentHerbsCount);
	}
}