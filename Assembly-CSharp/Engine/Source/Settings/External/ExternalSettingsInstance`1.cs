using AssetDatabases;
using Cofe.Meta;
using Engine.Common.Generator;
using Inspectors;
using System;
using System.IO;
using UnityEngine;

namespace Engine.Source.Settings.External
{
  public class ExternalSettingsInstance<T> where T : ExternalSettingsInstance<T>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    public int Version;
    private static T instance;

    public static T Instance
    {
      get
      {
        if ((object) ExternalSettingsInstance<T>.instance == null)
        {
          MetaService.Initialise("[Engine]");
          bool flag = !Application.isEditor;
          ExternalSettingsInstance<T>.instance = ExternalSettingsInstance<T>.LoadFromResources("Assets/Data/Settings/Resources/" + typeof (T).Name + ".xml");
          if (flag)
          {
            string str = "{DataPath}/Settings/".Replace("{DataPath}", Application.persistentDataPath) + typeof (T).Name + ".xml";
            T obj = default (T);
            if (File.Exists(str))
            {
              try
              {
                obj = ExternalSettingsInstance<T>.LoadFromFile(str);
              }
              catch (Exception ex)
              {
                Debug.LogError((object) ("Error load settings : " + str + " , ex : " + (object) ex));
              }
            }
            if ((object) obj == null || obj.Version != ExternalSettingsInstance<T>.instance.Version)
            {
              FileInfo fileInfo = new FileInfo(str);
              if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
              SerializeUtility.Serialize<T>(str, ExternalSettingsInstance<T>.instance);
            }
            else
              ExternalSettingsInstance<T>.instance = obj;
          }
        }
        return ExternalSettingsInstance<T>.instance;
      }
    }

    private static T LoadFromFile(string path) => SerializeUtility.Deserialize<T>(path);

    private static T LoadFromResources(string path)
    {
      TextAsset textAsset = AssetDatabaseService.Instance.Load<TextAsset>(path);
      if (!((UnityEngine.Object) textAsset != (UnityEngine.Object) null) || textAsset.bytes == null)
        return default (T);
      using (MemoryStream memoryStream = new MemoryStream(textAsset.bytes))
        return SerializeUtility.Deserialize<T>((Stream) memoryStream, path);
    }
  }
}
