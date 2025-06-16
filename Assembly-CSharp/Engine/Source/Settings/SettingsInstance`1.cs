// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.SettingsInstance`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using System;
using System.IO;
using UnityEngine;

#nullable disable
namespace Engine.Source.Settings
{
  public abstract class SettingsInstance<T> : ISettingsInstance<T> where T : class, ISettingsInstance<T>
  {
    private static readonly string folderName = "{DataPath}/Settings/".Replace("{DataPath}", Application.persistentDataPath);
    private static readonly string fileName = SettingsInstance<T>.folderName + typeof (T).Name + ".xml";
    private static T instance;

    public static T Instance
    {
      get
      {
        if ((object) SettingsInstance<T>.instance == null)
        {
          if (File.Exists(SettingsInstance<T>.fileName))
          {
            SettingsInstance<T>.instance = SerializeUtility.Deserialize<T>(SettingsInstance<T>.fileName);
          }
          else
          {
            SettingsInstance<T>.instance = ServiceLocator.GetService<IFactory>().Create<T>();
            SettingsInstance<T>.instance.Apply();
          }
        }
        return SettingsInstance<T>.instance;
      }
    }

    public event Action OnApply;

    public void Apply()
    {
      string directoryName = Path.GetDirectoryName(SettingsInstance<T>.fileName);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      SerializeUtility.Serialize<SettingsInstance<T>>(SettingsInstance<T>.fileName, this);
      Action onApply = this.OnApply;
      if (onApply == null)
        return;
      onApply();
    }
  }
}
