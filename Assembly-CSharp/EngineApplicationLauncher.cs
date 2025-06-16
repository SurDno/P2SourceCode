using System;
using System.Collections;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using UnityEngine;
using Logger = Cofe.Loggers.Logger;

public class EngineApplicationLauncher : MonoBehaviour
{
  private void Awake()
  {
    Debug.unityLogger.filterLogType = LogType.Log;
    UnityLogger.Initialize();
    Debug.Log("Date : " + DateTime.Now.ToString("dd:MM:yyyy"));
    Logger.SetLogger(UnityLogger.Instance);
    EngineRuntime.IsRuntime = true;
    AffinityUtility.ComputeAffinity();
    MetaService.Initialise("[Engine]");
    ServiceLocator.Clear();
    RuntimeServiceAttribute.CreateServices();
  }

  private IEnumerator Start()
  {
    yield return new WaitForEndOfFrame();
    yield return InstanceByRequest<EngineApplication>.Instance.Initialize();
    ServiceLocator.GetService<GameLauncher>().StartGame();
  }

  private void OnApplicationQuit()
  {
  }
}
