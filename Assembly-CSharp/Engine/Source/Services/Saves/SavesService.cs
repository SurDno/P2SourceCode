// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Saves.SavesService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Settings.External;
using Engine.Source.Unity;
using Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

#nullable disable
namespace Engine.Source.Services.Saves
{
  [RuntimeService(new System.Type[] {typeof (SavesService)})]
  public class SavesService : IErrorLoadingHandler
  {
    private List<ISavesController> serializables;
    private const string nodeName = "Document";
    private bool saveLoadState;

    public event Action UnloadEvent;

    public string ErrorLoading { get; private set; }

    public bool HasErrorLoading => this.ErrorLoading != null;

    public IEnumerator Load()
    {
      UnityEngine.Debug.Log((object) "Try default load");
      this.ErrorLoading = (string) null;
      if (this.saveLoadState)
      {
        UnityEngine.Debug.LogError((object) "Save load already state");
      }
      else
      {
        this.saveLoadState = true;
        Stopwatch sw = new Stopwatch();
        this.serializables = ServiceLocatorUtility.GetServices<ISavesController, SaveDependAttribute>();
        for (int index = 0; index < this.serializables.Count; ++index)
        {
          ISavesController serializable = this.serializables[index];
          sw.Restart();
          yield return (object) serializable.Load((IErrorLoadingHandler) this);
          sw.Stop();
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Load)).Append(" , type : ").Append(TypeUtility.GetTypeName(serializable.GetType())).Append(" , elapsed : ").Append((object) sw.Elapsed));
          if (!this.HasErrorLoading)
            serializable = (ISavesController) null;
          else
            break;
        }
        this.saveLoadState = false;
      }
    }

    public IEnumerator Load(string saveName)
    {
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Try load ").Append(saveName));
      this.ErrorLoading = (string) null;
      if (this.saveLoadState)
      {
        UnityEngine.Debug.LogError((object) "Save load already state");
      }
      else
      {
        this.saveLoadState = true;
        Stopwatch sw = new Stopwatch();
        this.serializables = ServiceLocatorUtility.GetServices<ISavesController, SaveDependAttribute>();
        for (int index = 0; index < this.serializables.Count; ++index)
        {
          ISavesController serializable = this.serializables[index];
          string typeName = TypeUtility.GetTypeName(serializable.GetType());
          string fileName = saveName + "/" + typeName + ".xml";
          sw.Restart();
          XmlDocument doc = (XmlDocument) null;
          try
          {
            doc = SavesServiceUtility.LoadDocument(fileName);
          }
          catch (Exception ex)
          {
            this.LogException(ex);
            break;
          }
          if (doc == null)
          {
            this.LogError("File not found : " + fileName);
            break;
          }
          yield return (object) serializable.Load(doc.DocumentElement, fileName, (IErrorLoadingHandler) this);
          sw.Stop();
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Load)).Append(" , file name : ").Append(fileName).Append(" , type : ").Append(typeName).Append(" , elapsed : ").Append((object) sw.Elapsed));
          if (!this.HasErrorLoading)
          {
            serializable = (ISavesController) null;
            typeName = (string) null;
            fileName = (string) null;
            doc = (XmlDocument) null;
          }
          else
            break;
        }
        if (!this.HasErrorLoading)
          ServiceLocator.GetService<LogicEventService>().FireCommonEvent("GameLoadComplete");
        this.saveLoadState = false;
      }
    }

    public void Unload()
    {
      UnityEngine.Debug.Log((object) "Try unload");
      if (this.saveLoadState)
      {
        UnityEngine.Debug.LogError((object) "Save load already state");
      }
      else
      {
        this.saveLoadState = true;
        Stopwatch stopwatch = new Stopwatch();
        for (int index = 0; index < this.serializables.Count; ++index)
        {
          ISavesController serializable = this.serializables[index];
          try
          {
            stopwatch.Restart();
            serializable.Unload();
            stopwatch.Stop();
            UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Unload)).Append(" , type : ").Append(TypeUtility.GetTypeName(serializable.GetType())).Append(" , elapsed : ").Append((object) stopwatch.Elapsed));
          }
          catch (Exception ex)
          {
            UnityEngine.Debug.LogException(ex);
          }
        }
        this.serializables = (List<ISavesController>) null;
        this.saveLoadState = false;
        Action unloadEvent = this.UnloadEvent;
        if (unloadEvent == null)
          return;
        unloadEvent();
      }
    }

    public void Save(string saveName)
    {
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("Try save ").Append(saveName));
      if (this.saveLoadState)
      {
        UnityEngine.Debug.LogError((object) "Save load already state");
      }
      else
      {
        this.saveLoadState = true;
        FileUtility.CleanFolder(saveName);
        Stopwatch stopwatch = new Stopwatch();
        for (int index = 0; index < this.serializables.Count; ++index)
        {
          ISavesController serializable = this.serializables[index];
          string typeName = TypeUtility.GetTypeName(serializable.GetType());
          string fileName = saveName + "/" + typeName + ".xml";
          stopwatch.Restart();
          SavesServiceUtility.SaveToFile("Document", serializable, fileName, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SaveCompress);
          stopwatch.Stop();
          UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (SavesService)).Append(" : ").Append(nameof (Save)).Append(" , type : ").Append(typeName).Append(" , elapsed : ").Append((object) stopwatch.Elapsed));
        }
        this.saveLoadState = false;
      }
    }

    public void LogError(string text)
    {
      this.ErrorLoading = text;
      UnityEngine.Debug.LogError((object) text);
    }

    public void LogException(Exception e)
    {
      this.ErrorLoading = e.ToString();
      UnityEngine.Debug.LogException(e);
    }

    public struct SaveInfo
    {
      public string FileName;
      public bool Compress;
    }
  }
}
