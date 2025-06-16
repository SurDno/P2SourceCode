using System.Collections.Generic;
using System.IO;
using Assets.Engine.Source.Services.Profiles;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services.Profiles;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services;

[RuntimeService(typeof(ProfilesService), typeof(IProfilesService))]
public class ProfilesService : IProfilesService, IInitialisable {
	[Inspected] private ProfilesData data;
	[FromLocator] private IFactory factory;
	[FromLocator] private ITimeService timeService;

	public string GetValue(string name) {
		if (Current == null)
			return "";
		var customProfileData = Current.Data.FirstOrDefaultNoAlloc(o => o.Name == name);
		if (customProfileData == null)
			return "";
		Debug.Log(ObjectInfoUtility.GetStream().Append(customProfileData.Name).Append(" : ")
			.Append(customProfileData.Value));
		return customProfileData.Value;
	}

	public void SetValue(string name, string value) {
		if (Current == null)
			return;
		var customProfileData = Current.Data.FirstOrDefaultNoAlloc(o => o.Name == name);
		if (customProfileData == null) {
			customProfileData = ServiceCache.Factory.Create<CustomProfileData>();
			customProfileData.Name = name;
			Current.Data.Add(customProfileData);
		}

		customProfileData.Value = value;
		SaveProfiles();
	}

	public int GetIntValue(string name) {
		return DefaultConverter.ParseInt(GetValue(name));
	}

	public void SetIntValue(string name, int value) {
		SetValue(name, DefaultConverter.ToString(value));
	}

	public bool GetBoolValue(string name) {
		return DefaultConverter.ParseBool(GetValue(name));
	}

	public void SetBoolValue(string name, bool value) {
		SetValue(name, DefaultConverter.ToString(value));
	}

	public float GetFloatValue(string name) {
		return DefaultConverter.ParseFloat(GetValue(name));
	}

	public void SetFloatValue(string name, float value) {
		SetValue(name, DefaultConverter.ToString(value));
	}

	public ProfileData Current {
		get {
			CheckData();
			return data.CurrentIndex < 0 || data.CurrentIndex >= data.Profiles.Count
				? null
				: data.Profiles[data.CurrentIndex];
		}
	}

	public IEnumerable<ProfileData> Profiles => data.Profiles;

	public void Initialise() {
		CheckData();
	}

	public void Terminate() { }

	private void CheckData() {
		if (data != null)
			return;
		var path = "{DataPath}/Saves/Profiles.xml".Replace("{DataPath}", Application.persistentDataPath);
		if (File.Exists(path))
			data = SerializeUtility.Deserialize<ProfilesData>(path);
		else {
			data = factory.Create<ProfilesData>();
			SaveProfiles();
		}
	}

	public void GenerateNewProfile(string gameName) {
		var profileData = factory.Create<ProfileData>();
		profileData.Name = GetProfileName(gameName);
		data.CurrentIndex = data.Profiles.Count;
		data.Profiles.Add(profileData);
		SaveProfiles();
	}

	private string GetProfileName(string gameName) {
		var num = 1;
		string profileName;
		while (true) {
			profileName = gameName + " " + num;
			var flag = false;
			foreach (var profile in Profiles)
				if (profile.Name == profileName) {
					flag = true;
					break;
				}

			if (flag)
				++num;
			else
				break;
		}

		return profileName;
	}

	public void SetCurrent(string name) {
		var profileData = data.Profiles.FirstOrDefaultNoAlloc(o => o.Name == name);
		if (profileData == null || profileData == Current)
			return;
		data.CurrentIndex = data.Profiles.IndexOf(profileData);
		SaveProfiles();
	}

	public void DeleteProfile(string name) {
		var profileData = data.Profiles.FirstOrDefaultNoAlloc(o => o.Name == name);
		if (profileData == null)
			return;
		var current = Current;
		if (profileData == current)
			return;
		data.Profiles.Remove(profileData);
		data.CurrentIndex = data.Profiles.IndexOf(current);
		SaveProfiles();
		var path = ProfilesUtility.ProfilePath(profileData.Name);
		if (!Directory.Exists(path))
			return;
		Directory.Delete(path, true);
	}

	public void GenerateSaveName() {
		Current.LastSave = ProfilesUtility.GenerateSaveName();
		SaveProfiles();
	}

	private void SaveProfiles() {
		var path = "{DataPath}/Saves/Profiles.xml".Replace("{DataPath}", Application.persistentDataPath);
		var directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
			Directory.CreateDirectory(directoryName);
		SerializeUtility.Serialize(path, data);
	}

	public string GetLastSaveName() {
		return Current == null || Current.LastSave == ""
			? ""
			: ProfilesUtility.SavePath(Current.Name, Current.LastSave);
	}

	public void DeleteSave(string name) {
		var current = Current;
		if (current == null)
			return;
		var path = ProfilesUtility.SavePath(current.Name, name);
		if (!Directory.Exists(path))
			return;
		Directory.Delete(path, true);
		var saveNames = ProfilesUtility.GetSaveNames(current.Name);
		current.LastSave = saveNames.Count <= 0 ? "" : saveNames[0];
		SaveProfiles();
	}
}