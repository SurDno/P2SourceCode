// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.ProfilesService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Assets.Engine.Source.Services.Profiles;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services.Profiles;
using Inspectors;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (ProfilesService), typeof (IProfilesService)})]
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
      if (this.Current == null)
        return "";
      CustomProfileData customProfileData = this.Current.Data.FirstOrDefaultNoAlloc<CustomProfileData>((Func<CustomProfileData, bool>) (o => o.Name == name));
      if (customProfileData == null)
        return "";
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(customProfileData.Name).Append(" : ").Append(customProfileData.Value));
      return customProfileData.Value;
    }

    public void SetValue(string name, string value)
    {
      if (this.Current == null)
        return;
      CustomProfileData customProfileData = this.Current.Data.FirstOrDefaultNoAlloc<CustomProfileData>((Func<CustomProfileData, bool>) (o => o.Name == name));
      if (customProfileData == null)
      {
        customProfileData = ServiceCache.Factory.Create<CustomProfileData>();
        customProfileData.Name = name;
        this.Current.Data.Add(customProfileData);
      }
      customProfileData.Value = value;
      this.SaveProfiles();
    }

    public int GetIntValue(string name) => DefaultConverter.ParseInt(this.GetValue(name));

    public void SetIntValue(string name, int value)
    {
      this.SetValue(name, DefaultConverter.ToString(value));
    }

    public bool GetBoolValue(string name) => DefaultConverter.ParseBool(this.GetValue(name));

    public void SetBoolValue(string name, bool value)
    {
      this.SetValue(name, DefaultConverter.ToString(value));
    }

    public float GetFloatValue(string name) => DefaultConverter.ParseFloat(this.GetValue(name));

    public void SetFloatValue(string name, float value)
    {
      this.SetValue(name, DefaultConverter.ToString(value));
    }

    public ProfileData Current
    {
      get
      {
        this.CheckData();
        return this.data.CurrentIndex < 0 || this.data.CurrentIndex >= this.data.Profiles.Count ? (ProfileData) null : this.data.Profiles[this.data.CurrentIndex];
      }
    }

    public IEnumerable<ProfileData> Profiles => (IEnumerable<ProfileData>) this.data.Profiles;

    public void Initialise() => this.CheckData();

    public void Terminate()
    {
    }

    private void CheckData()
    {
      if (this.data != null)
        return;
      string path = "{DataPath}/Saves/Profiles.xml".Replace("{DataPath}", Application.persistentDataPath);
      if (File.Exists(path))
      {
        this.data = SerializeUtility.Deserialize<ProfilesData>(path);
      }
      else
      {
        this.data = this.factory.Create<ProfilesData>();
        this.SaveProfiles();
      }
    }

    public void GenerateNewProfile(string gameName)
    {
      ProfileData profileData = this.factory.Create<ProfileData>();
      profileData.Name = this.GetProfileName(gameName);
      this.data.CurrentIndex = this.data.Profiles.Count;
      this.data.Profiles.Add(profileData);
      this.SaveProfiles();
    }

    private string GetProfileName(string gameName)
    {
      int num = 1;
      string profileName;
      while (true)
      {
        profileName = gameName + " " + (object) num;
        bool flag = false;
        foreach (ProfileData profile in this.Profiles)
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
      ProfileData profileData = this.data.Profiles.FirstOrDefaultNoAlloc<ProfileData>((Func<ProfileData, bool>) (o => o.Name == name));
      if (profileData == null || profileData == this.Current)
        return;
      this.data.CurrentIndex = this.data.Profiles.IndexOf(profileData);
      this.SaveProfiles();
    }

    public void DeleteProfile(string name)
    {
      ProfileData profileData = this.data.Profiles.FirstOrDefaultNoAlloc<ProfileData>((Func<ProfileData, bool>) (o => o.Name == name));
      if (profileData == null)
        return;
      ProfileData current = this.Current;
      if (profileData == current)
        return;
      this.data.Profiles.Remove(profileData);
      this.data.CurrentIndex = this.data.Profiles.IndexOf(current);
      this.SaveProfiles();
      string path = ProfilesUtility.ProfilePath(profileData.Name);
      if (!Directory.Exists(path))
        return;
      Directory.Delete(path, true);
    }

    public void GenerateSaveName()
    {
      this.Current.LastSave = ProfilesUtility.GenerateSaveName();
      this.SaveProfiles();
    }

    private void SaveProfiles()
    {
      string path = "{DataPath}/Saves/Profiles.xml".Replace("{DataPath}", Application.persistentDataPath);
      string directoryName = Path.GetDirectoryName(path);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      SerializeUtility.Serialize<ProfilesData>(path, this.data);
    }

    public string GetLastSaveName()
    {
      return this.Current == null || this.Current.LastSave == "" ? "" : ProfilesUtility.SavePath(this.Current.Name, this.Current.LastSave);
    }

    public void DeleteSave(string name)
    {
      ProfileData current = this.Current;
      if (current == null)
        return;
      string path = ProfilesUtility.SavePath(current.Name, name);
      if (!Directory.Exists(path))
        return;
      Directory.Delete(path, true);
      List<string> saveNames = ProfilesUtility.GetSaveNames(current.Name);
      current.LastSave = saveNames.Count <= 0 ? "" : saveNames[0];
      this.SaveProfiles();
    }
  }
}
