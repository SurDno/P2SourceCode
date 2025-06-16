using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Utility;
using System;
using System.Collections;
using UnityEngine;

public class EngineApplicationLauncher : MonoBehaviour
{
  private void Awake()
  {
    Debug.unityLogger.filterLogType = LogType.Log;
    UnityLogger.Initialize();
    Debug.Log((object) ("Date : " + DateTime.Now.ToString("dd:MM:yyyy")));
    Cofe.Loggers.Logger.SetLogger(UnityLogger.Instance);
    EngineRuntime.IsRuntime = true;
    AffinityUtility.ComputeAffinity();
    MetaService.Initialise("[Engine]");
    ServiceLocator.Clear();
    RuntimeServiceAttribute.CreateServices();
  }

  private IEnumerator Start()
  {
    yield return (object) new WaitForEndOfFrame();
    yield return (object) InstanceByRequest<EngineApplication>.Instance.Initialize();
    ServiceLocator.GetService<GameLauncher>().StartGame();
  }

  private void OnApplicationQuit()
  {
  }
}
