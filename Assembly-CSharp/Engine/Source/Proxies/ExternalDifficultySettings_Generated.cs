using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ExternalDifficultySettings))]
public class ExternalDifficultySettings_Generated :
	ExternalDifficultySettings,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ExternalDifficultySettings_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var settingsGenerated = (ExternalDifficultySettings_Generated)target2;
		settingsGenerated.Version = Version;
		CloneableObjectUtility.CopyListTo(settingsGenerated.Items, Items);
		CloneableObjectUtility.CopyListTo(settingsGenerated.Groups, Groups);
		CloneableObjectUtility.CopyListTo(settingsGenerated.Presets, Presets);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "Version", Version);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Items", Items);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Groups", Groups);
		DefaultDataWriteUtility.WriteListSerialize(writer, "Presets", Presets);
	}

	public void DataRead(IDataReader reader, Type type) {
		Version = DefaultDataReadUtility.Read(reader, "Version", Version);
		Items = DefaultDataReadUtility.ReadListSerialize(reader, "Items", Items);
		Groups = DefaultDataReadUtility.ReadListSerialize(reader, "Groups", Groups);
		Presets = DefaultDataReadUtility.ReadListSerialize(reader, "Presets", Presets);
	}
}