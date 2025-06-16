using System.Collections.Generic;
using System.IO;
using Assets.Engine.Source.Services.Profiles;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services.Profiles;
using Inspectors;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (ProfilesService), typeof (IProfilesService))]
  public class ProfilesService : IProfilesService, IInitialisable
  {
    [Inspected]
    private ProfilesData data;
    [FromLocator]
    private IFactory factory;
    [FromLocator]
    private ITimeService timeService;

    public string GetValue(string name)
    {
      if (Current == null)
        return "";
      CustomProfileData customProfileData = Current.Data.FirstOrDefaultNoAlloc(o => o.Name == name);
      if (customProfileData == null)
        return "";
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(customProfileData.Name).Append(" : ").Append(customProfileData.Value));
      return customProfileData.Value;
    }

    public void SetValue(string name, string value)
    {
      if (Current == null)
        return;
      CustomProfileData customProfileData = Current.Data.FirstOrDefaultNoAlloc(o => o.Name == name);
      if (customProfileData == null)
      {
        customProfileData = ServiceCache.Factory.Create<CustomProfileData>();
        customProfileData.Name = name;
        Current.Data.Add(customProfileData);
      }
      customProfileData.Value = value;
      SaveProfiles();
    }

    public int GetIntValue(string name) => DefaultConverter.ParseInt(GetValue(name));

    public void SetIntValue(string name, int value)
    {
      SetValue(name, DefaultConverter.ToString(value));
    }

    public bool GetBoolValue(string name) => DefaultConverter.ParseBool(GetValue(name));

    public void SetBoolValue(string name, bool value)
    {
      SetValue(name, DefaultConverter.ToString(value));
    }

    public float GetFloatValue(string name) => DefaultConverter.ParseFloat(GetValue(name));

    public void SetFloatValue(string name, float value)
    {
      SetValue(name, DefaultConverter.ToString(value));
    }

    public ProfileData Current
    {
      get
      {
        CheckData();
        return data.CurrentIndex < 0 || data.CurrentIndex >= data.Profiles.Count ? null : data.Profiles[data.CurrentIndex];
      }
    }

    public IEnumerable<ProfileData> Profiles => data.Profiles;

    public void Initialise() => CheckData();

    public void Terminate()
    {
    }

    private void CheckData()
    {
      if (data != null)
        return;
      string path = "{DataPath}/Saves/Profiles.xml".Replace("{DataPath}", Application.persistentDataPath);
      if (File.Exists(path))
      {
        data = SerializeUtility.Deserialize<ProfilesData>(path);
      }
      else
      {
        data = factory.Create<ProfilesData>();
        SaveProfiles();
      }
    }

    public void GenerateNewProfile(string gameName)
    {
      ProfileData profileData = factory.Create<ProfileData>();
      profileData.Name = GetProfileName(gameName);
      data.CurrentIndex = data.Profiles.Count;
      data.Profiles.Add(profileData);
      SaveProfiles();
    }

    private string GetProfileName(string gameName)
    {
      int num = 1;
      string profileName;
      while (true)
      {
        profileName = gameName + " " + num;
        bool flag = false;
        foreach (ProfileData profile in Profiles)
        {
          if (profile.Name == profileName)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          ++num;
        else
          break;
      }
      return profileName;
    }

    public void SetCurrent(string name)
    {
      ProfileData profileData = data.Profiles.FirstOrDefaultNoAlloc(o => o.Name == name);
      if (profileData == null || profileData == Current)
        return;
      data.CurrentIndex = data.Profiles.IndexOf(profileData);
      SaveProfiles();
    }

    public void DeleteProfile(string name)
    {
      ProfileData profileData = data.Profiles.FirstOrDefaultNoAlloc(o => o.Name == name);
      if (profileData == null)
        return;
      ProfileData current = Current;
      if (profileData == current)
        return;
      data.Profiles.Remove(profileData);
      data.CurrentIndex = data.Profiles.IndexOf(current);
      SaveProfiles();
      string path = ProfilesUtility.ProfilePath(profileData.Name);
      if (!Directory.Exists(path))
        return;
      Directory.Delete(path, true);
    }

    public void GenerateSaveName()
    {
      Current.LastSave = ProfilesUtility.GenerateSaveName();
      SaveProfiles();
    }

    private void SaveProfiles()
    {
      string path = "{DataPath}/Saves/Profiles.xml".Replace("{DataPath}", Application.persistentDataPath);
      string directoryName = Path.GetDirectoryName(path);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      SerializeUtility.Serialize(path, data);
    }

    public string GetLastSaveName()
    {
      return Current == null || Current.LastSave == "" ? "" : ProfilesUtility.SavePath(Current.Name, Current.LastSave);
    }

    public void DeleteSave(string name)
    {
      ProfileData current = Current;
      if (current == null)
        return;
      string path = ProfilesUtility.SavePath(current.Name, name);
      if (!Directory.Exists(path))
        return;
      Directory.Delete(path, true);
      List<string> saveNames = ProfilesUtility.GetSaveNames(current.Name);
      current.LastSave = saveNames.Count <= 0 ? "" : saveNames[0];
      SaveProfiles();
    }
  }
}
