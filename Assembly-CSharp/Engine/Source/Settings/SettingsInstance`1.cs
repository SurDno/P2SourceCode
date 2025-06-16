using System;
using System.IO;
using Engine.Common.Services;

namespace Engine.Source.Settings
{
  public abstract class SettingsInstance<T> : ISettingsInstance<T> where T : class, ISettingsInstance<T>
  {
    private static readonly string folderName = "{DataPath}/Settings/".Replace("{DataPath}", Application.persistentDataPath);
    private static readonly string fileName = folderName + typeof (T).Name + ".xml";
    private static T instance;

    public static T Instance
    {
      get
      {
        if (instance == null)
        {
          if (File.Exists(fileName))
          {
            instance = SerializeUtility.Deserialize<T>(fileName);
          }
          else
          {
            instance = ServiceLocator.GetService<IFactory>().Create<T>();
            instance.Apply();
          }
        }
        return instance;
      }
    }

    public event Action OnApply;

    public void Apply()
    {
      string directoryName = Path.GetDirectoryName(fileName);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      SerializeUtility.Serialize(fileName, this);
      Action onApply = OnApply;
      if (onApply == null)
        return;
      onApply();
    }
  }
}
