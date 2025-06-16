using System;
using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;

namespace Engine.Source.Services
{
  [GameService]
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
      writer.Begin("Info", null, true);
      DefaultDataWriteUtility.Write(writer, "SaveTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
      DefaultDataWriteUtility.Write(writer, "GameTime", timeService.GameTime.ToString("d\\.hh\\:mm\\:ss"));
      DefaultDataWriteUtility.Write(writer, "BuildVersion", version);
      DefaultDataWriteUtility.Write(writer, "BuildTime", ScriptableObjectInstance<BuildData>.Instance.Time);
      DefaultDataWriteUtility.Write(writer, "BuildLabel", InstanceByRequest<LabelService>.Instance.Label);
      DefaultDataWriteUtility.Write(writer, "UnityVersion", unityVersion);
      DefaultDataWriteUtility.Write(writer, "MachineName", Environment.MachineName);
      writer.End("Info", true);
    }

    public void Unload()
    {
    }
  }
}
