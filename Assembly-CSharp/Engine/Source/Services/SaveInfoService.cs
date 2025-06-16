// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.SaveInfoService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using System;
using System.Collections;
using System.Xml;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new System.Type[] {})]
  public class SaveInfoService : ISavesController
  {
    private const string nodeName = "Info";
    [FromLocator]
    private ITimeService timeService;
    [FromLocator]
    private GameLauncher gameLauncher;
    private static readonly string version = Application.version;
    private static readonly string unityVersion = Application.unityVersion;

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public void Save(IDataWriter writer, string context)
    {
      writer.Begin("Info", (System.Type) null, true);
      DefaultDataWriteUtility.Write(writer, "SaveTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
      DefaultDataWriteUtility.Write(writer, "GameTime", this.timeService.GameTime.ToString("d\\.hh\\:mm\\:ss"));
      DefaultDataWriteUtility.Write(writer, "BuildVersion", SaveInfoService.version);
      DefaultDataWriteUtility.Write(writer, "BuildTime", ScriptableObjectInstance<BuildData>.Instance.Time);
      DefaultDataWriteUtility.Write(writer, "BuildLabel", InstanceByRequest<LabelService>.Instance.Label);
      DefaultDataWriteUtility.Write(writer, "UnityVersion", SaveInfoService.unityVersion);
      DefaultDataWriteUtility.Write(writer, "MachineName", Environment.MachineName);
      writer.End("Info", true);
    }

    public void Unload()
    {
    }
  }
}
