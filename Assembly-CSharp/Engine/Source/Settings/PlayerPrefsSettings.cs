// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.PlayerPrefsSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Settings
{
  public class PlayerPrefsSettings : IPlayerSettings
  {
    public int GetInt(string name, int defaultValue) => PlayerPrefs.GetInt(name, defaultValue);

    public void SetInt(string name, int value) => PlayerPrefs.SetInt(name, value);

    public bool GetBool(string name, bool defaultValue)
    {
      return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) != 0;
    }

    public void SetBool(string name, bool value) => PlayerPrefs.SetInt(name, value ? 1 : 0);

    public float GetFloat(string name, float defaultValue)
    {
      return PlayerPrefs.GetFloat(name, defaultValue);
    }

    public void SetFloat(string name, float value) => PlayerPrefs.SetFloat(name, value);

    public string GetString(string name, string defaultValue)
    {
      return PlayerPrefs.GetString(name, defaultValue);
    }

    public void SetString(string name, string value) => PlayerPrefs.SetString(name, value);

    public T GetEnum<T>(string name, T defaultValue) where T : struct, IComparable, IFormattable, IConvertible
    {
      return (T) (ValueType) PlayerPrefs.GetInt(name, (int) (ValueType) defaultValue);
    }

    public void SetEnum<T>(string name, T value) where T : struct, IComparable, IFormattable, IConvertible
    {
      PlayerPrefs.SetInt(name, (int) (ValueType) value);
    }

    public void Save() => PlayerPrefs.Save();
  }
}
