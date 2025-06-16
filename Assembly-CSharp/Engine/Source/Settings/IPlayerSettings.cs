// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.IPlayerSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Engine.Source.Settings
{
  public interface IPlayerSettings
  {
    int GetInt(string name, int defaultValue = 0);

    void SetInt(string name, int value);

    bool GetBool(string name, bool defaultValue = false);

    void SetBool(string name, bool value);

    float GetFloat(string name, float defaultValue = 0.0f);

    void SetFloat(string name, float value);

    string GetString(string name, string defaultValue = "");

    void SetString(string name, string value);

    T GetEnum<T>(string name, T defaultValue = default (T)) where T : struct, IComparable, IFormattable, IConvertible;

    void SetEnum<T>(string name, T value) where T : struct, IComparable, IFormattable, IConvertible;

    void Save();
  }
}
