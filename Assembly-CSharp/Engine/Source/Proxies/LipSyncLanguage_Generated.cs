using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(LipSyncLanguage))]
public class LipSyncLanguage_Generated :
	LipSyncLanguage,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<LipSyncLanguage_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var languageGenerated = (LipSyncLanguage_Generated)target2;
		languageGenerated.Language = Language;
		CloneableObjectUtility.CopyListTo(languageGenerated.LipSyncs, LipSyncs);
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteEnum(writer, "Language", Language);
		DefaultDataWriteUtility.WriteListSerialize(writer, "LipSyncs", LipSyncs);
	}

	public void DataRead(IDataReader reader, Type type) {
		Language = DefaultDataReadUtility.ReadEnum<LanguageEnum>(reader, "Language");
		LipSyncs = DefaultDataReadUtility.ReadListSerialize(reader, "LipSyncs", LipSyncs);
	}
}