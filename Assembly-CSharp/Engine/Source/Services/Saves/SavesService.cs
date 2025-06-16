using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Settings.External;
using Engine.Source.Unity;
using Scripts.Utility;
using Debug = UnityEngine.Debug;

namespace Engine.Source.Services.Saves
{
  [RuntimeService(typeof (SavesService))]
  public class SavesService : IErrorLoadingHandler
  {
    private List<ISavesController> serializables;
    private const string nodeName = "Document";
    private bool saveLoadState;

    public event Action UnloadEvent;

    public string ErrorLoading { get; private set; }

    public bool HasErrorLoading => ErrorLoading != null;

    public IEnumerator Load()
    {
      Debug.Log("Try default load");
      ErrorLoading = null;
      if (saveLoadState)
      {
        Debug.LogError("Save load already state");
      }
      else
      {
        saveLoadState = true;
        Stopwatch sw = new Stopwatch();
        serializables = ServiceLocatorUtility.GetServices<ISavesController, SaveDependAttribute>();
        for (int index = 0; index < serializables.Count; ++index)
        {
          ISavesController serializable = serializables[index];
          sw.Restart();
          yield return serializable.Load(this);
          sw.Stop();
          Debug.Log(ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Load)).Append(" , type : ").Append(TypeUtility.GetTypeName(serializable.GetType())).Append(" , elapsed : ").Append(sw.Elapsed));
          if (!HasErrorLoading)
            serializable = null;
          else
            break;
        }
        saveLoadState = false;
      }
    }

    public IEnumerator Load(string saveName)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Try load ").Append(saveName));
      ErrorLoading = null;
      if (saveLoadState)
      {
        Debug.LogError("Save load already state");
      }
      else
      {
        saveLoadState = true;
        Stopwatch sw = new Stopwatch();
        serializables = ServiceLocatorUtility.GetServices<ISavesController, SaveDependAttribute>();
        for (int index = 0; index < serializables.Count; ++index)
        {
          ISavesController serializable = serializables[index];
          string typeName = TypeUtility.GetTypeName(serializable.GetType());
          string fileName = saveName + "/" + typeName + ".xml";
          sw.Restart();
          XmlDocument doc = null;
          try
          {
            doc = SavesServiceUtility.LoadDocument(fileName);
          }
          catch (Exception ex)
          {
            LogException(ex);
            break;
          }
          if (doc == null)
          {
            LogError("File not found : " + fileName);
            break;
          }
          yield return serializable.Load(doc.DocumentElement, fileName, this);
          sw.Stop();
          Debug.Log(ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Load)).Append(" , file name : ").Append(fileName).Append(" , type : ").Append(typeName).Append(" , elapsed : ").Append(sw.Elapsed));
          if (!HasErrorLoading)
          {
            serializable = null;
            typeName = null;
            fileName = null;
            doc = null;
          }
          else
            break;
        }
        if (!HasErrorLoading)
          ServiceLocator.GetService<LogicEventService>().FireCommonEvent("GameLoadComplete");
        saveLoadState = false;
      }
    }

    public void Unload()
    {
      Debug.Log("Try unload");
      if (saveLoadState)
      {
        Debug.LogError("Save load already state");
      }
      else
      {
        saveLoadState = true;
        Stopwatch stopwatch = new Stopwatch();
        for (int index = 0; index < serializables.Count; ++index)
        {
          ISavesController serializable = serializables[index];
          try
          {
            stopwatch.Restart();
            serializable.Unload();
            stopwatch.Stop();
            Debug.Log(ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Unload)).Append(" , type : ").Append(TypeUtility.GetTypeName(serializable.GetType())).Append(" , elapsed : ").Append(stopwatch.Elapsed));
          }
          catch (Exception ex)
          {
            Debug.LogException(ex);
          }
        }
        serializables = null;
        saveLoadState = false;
        Action unloadEvent = UnloadEvent;
        if (unloadEvent == null)
          return;
        unloadEvent();
      }
    }

    public void Save(string saveName)
    {
      Debug.Log(ObjectInfoUtility.GetStream().Append("Try save ").Append(saveName));
      if (saveLoadState)
      {
        Debug.LogError("Save load already state");
      }
      else
      {
        saveLoadState = true;
        FileUtility.CleanFolder(saveName);
        Stopwatch stopwatch = new Stopwatch();
        for (int index = 0; index < serializables.Count; ++index)
        {
          ISavesController serializable = serializables[index];
          string typeName = TypeUtility.GetTypeName(serializable.GetType());
          string fileName = saveName + "/" + typeName + ".xml";
          stopwatch.Restart();
          SavesServiceUtility.SaveToFile("Document", serializable, fileName, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SaveCompress);
          stopwatch.Stop();
          Debug.Log(ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Save)).Append(" , type : ").Append(typeName).Append(" , elapsed : ").Append(stopwatch.Elapsed));
        }
        saveLoadState = false;
      }
    }

    public void LogError(string text)
    {
      ErrorLoading = text;
      Debug.LogError(text);
    }

    public void LogException(Exception e)
    {
      ErrorLoading = e.ToString();
      Debug.LogException(e);
    }

    public struct SaveInfo
    {
      public string FileName;
      public bool Compress;
    }
  }
}
