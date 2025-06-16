using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Threading;
using Engine.Common;
using Engine.Source.Otimizations;
using Engine.Source.Settings.External;
using InputServices;
using Inspectors;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine.Analytics;

namespace Engine.Source.Commons
{
  public class EngineApplication : InstanceByRequest<EngineApplication>
  {
    private bool isPaused = true;
    private bool viewEnabled;

    public bool IsInitialized { get; private set; }

    [Inspected]
    public bool IsDebug => SRDebug.Instance.IsDebugging;

    [Inspected]
    public bool IsPaused
    {
      get => isPaused;
      set
      {
        DontStopLipSyncInPause = false;
        if (isPaused == value)
          return;
        isPaused = value;
        Action onPauseEvent = OnPauseEvent;
        if (onPauseEvent == null)
          return;
        onPauseEvent();
      }
    }

    [Inspected(Mutable = true)]
    public bool ViewEnabled
    {
      get => viewEnabled;
      set
      {
        if (viewEnabled == value)
          return;
        viewEnabled = value;
        Action<bool> viewEnabledEvent = OnViewEnabledEvent;
        if (viewEnabledEvent == null)
          return;
        viewEnabledEvent(viewEnabled);
      }
    }

    [Inspected]
    public bool DontStopLipSyncInPause { get; set; }

    [Inspected]
    public static bool Sleep { get; set; }

    [Inspected]
    public static Vector3 PlayerPosition { get; set; }

    [Inspected]
    public static int FrameCount { get; set; }

    [Inspected]
    public static Thread MainThread { get; private set; }

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    public static bool IsRuntime => EngineRuntime.IsRuntime;

    public event Action OnPauseEvent;

    public event Action<bool> OnApplicationFocusEvent;

    public event Action OnApplicationQuit;

    public event Action<bool> OnViewEnabledEvent;

    public event Action OnInitialized;

    public void FireApplicationFocusEvent(bool focus)
    {
      Action<bool> applicationFocusEvent = OnApplicationFocusEvent;
      if (applicationFocusEvent == null)
        return;
      applicationFocusEvent(focus);
    }

    public IEnumerator Initialize()
    {
      IsInitialized = !IsInitialized ? true : throw new Exception();
      MainThread = Thread.CurrentThread;
      yield return MemoryStrategy.Instance.Compute(MemoryStrategyContextEnum.ApplicationStart);
      UnityEngine.Debug.Log((object) ("Build Version : " + Application.version));
      UnityEngine.Debug.Log((object) ("Build Time : " + ScriptableObjectInstance<BuildData>.Instance.Time));
      UnityEngine.Debug.Log((object) ("Build Label : " + ScriptableObjectInstance<BuildData>.Instance.Label));
      UnityEngine.Debug.Log((object) ("Unity Version : " + Application.unityVersion));
      UnityEngine.Debug.Log((object) ("User Id : " + AnalyticsSessionInfo.userId));
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" ").Append(nameof (EngineApplication)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name).Append(" , scene : ").Append(SceneManager.GetActiveScene().path));
      Application.targetFrameRate = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.TargetFrameRate;
      GCSettings.LatencyMode = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.LatencyMode;
      NavMesh.pathfindingIterationsPerFrame = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.PathfindingIterationsPerFrame;
      Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append(" ").Append(nameof (EngineApplication)).Append(" : ").Append(MethodBase.GetCurrentMethod().Name).Append(" , current folder : '").Append(Environment.CurrentDirectory));
      SRDebugger.Settings.Instance.IsEnabled = false;
      ISystemInformationService systemInformationService = SRServiceManager.GetService<ISystemInformationService>();
      if (systemInformationService != null)
      {
        systemInformationService.AddInfo(typeof (BuildData).Name, Info.Create("Version", (object) Application.version));
        systemInformationService.AddInfo(typeof (BuildData).Name, Info.Create("Branch", ScriptableObjectInstance<BuildData>.Instance.Branch));
        systemInformationService.AddInfo(typeof (BuildData).Name, Info.Create("Time", ScriptableObjectInstance<BuildData>.Instance.Time));
        systemInformationService.AddInfo(typeof (BuildData).Name, Info.Create("Label", () => InstanceByRequest<LabelService>.Instance.Label));
      }
      if (UnityEngine.Debug.isDebugBuild)
        SRDebug.Init();
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      MainMenuSetup.SetupMainMenuSettings();
      Stopwatch sw = new Stopwatch();
      sw.Restart();
      yield return InitialiseServices.Initialise();
      sw.Stop();
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append("  ").Append("InitialiseServices").Append(" , elapsed : ").Append(sw.Elapsed));
      IsPaused = true;
      Action onInitialized = OnInitialized;
      if (onInitialized != null)
        onInitialized();
    }

    public void Terminate()
    {
      IsInitialized = IsInitialized ? false : throw new Exception();
      InitialiseServices.Terminate();
    }

    public void Exit()
    {
      Action onApplicationQuit = OnApplicationQuit;
      if (onApplicationQuit != null)
        onApplicationQuit();
      Application.Quit();
    }
  }
}
