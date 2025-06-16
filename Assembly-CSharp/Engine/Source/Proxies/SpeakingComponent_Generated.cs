using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(SpeakingComponent))]
public class SpeakingComponent_Generated :
	SpeakingComponent,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead,
	ISerializeStateSave,
	ISerializeStateLoad {
	public object Clone() {
		var instance = Activator.CreateInstance<SpeakingComponent_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		((SpeakingComponent_Generated)target2).isEnabled = isEnabled;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
	}

	public void DataRead(IDataReader reader, Type type) {
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
	}

	public void StateSave(IDataWriter writer) {
		DefaultDataWriteUtility.Write(writer, "SpeakAvailable", SpeakAvailable);
		DefaultDataWriteUtility.Write(writer, "IsEnabled", isEnabled);
		CustomStateSaveUtility.SaveListReferences(writer, "InitialPhrases", initialPhrases);
	}

	public void StateLoad(IDataReader reader, Type type) {
		SpeakAvailable = DefaultDataReadUtility.Read(reader, "SpeakAvailable", SpeakAvailable);
		isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", isEnabled);
		initialPhrases = CustomStateLoadUtility.LoadListReferences(reader, "InitialPhrases", initialPhrases);
	}
}