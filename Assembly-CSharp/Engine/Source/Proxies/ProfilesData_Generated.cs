using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Profiles;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(ProfilesData))]
public class ProfilesData_Generated :
	ProfilesData,
	ICloneable,
	ICopyable,
	ISerializeDataWrite,
	ISerializeDataRead {
	public object Clone() {
		var instance = Activator.CreateInstance<ProfilesData_Generated>();
		CopyTo(instance);
		return instance;
	}

	public void CopyTo(object target2) {
		var profilesDataGenerated = (ProfilesData_Generated)target2;
		CloneableObjectUtility.CopyListTo(profilesDataGenerated.Profiles, Profiles);
		profilesDataGenerated.CurrentIndex = CurrentIndex;
	}

	public void DataWrite(IDataWriter writer) {
		DefaultDataWriteUtility.WriteListSerialize(writer, "Profiles", Profiles);
		DefaultDataWriteUtility.Write(writer, "CurrentIndex", CurrentIndex);
	}

	public void DataRead(IDataReader reader, Type type) {
		Profiles = DefaultDataReadUtility.ReadListSerialize(reader, "Profiles", Profiles);
		CurrentIndex = DefaultDataReadUtility.Read(reader, "CurrentIndex", CurrentIndex);
	}
}