using Engine.Common;
using Engine.Source.Otimizations;
using Engine.Source.Settings.External;
using InputServices;
using Inspectors;
using SRDebugger.Services;
using SRF.Service;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

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
      get => this.isPaused;
      set
      {
        this.DontStopLipSyncInPause = false;
        if (this.isPaused == value)
          return;
        this.isPaused = value;
        Action onPauseEvent = this.OnPauseEvent;
        if (onPauseEvent == null)
          return;
        onPauseEvent();
      }
    }

    [Inspected(Mutable = true)]
    public bool ViewEnabled
    {
      get => this.viewEnabled;
      set
      {
        if (this.viewEnabled == value)
          return;
        this.viewEnabled = value;
        Action<bool> viewEnabledEvent = this.OnViewEnabledEvent;
        if (viewEnabledEvent == null)
          return;
        viewEnabledEvent(this.viewEnabled);
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
      Action<bool> applicationFocusEvent = this.OnApplicationFocusEvent;
      if (applicationFocusEvent == null)
        return;
      applicationFocusEvent(focus);
    }

    public IEnumerator Initialize()
    {
      this.IsInitialized = !this.IsInitialized ? true : throw new Exception();
      EngineApplication.MainThread = Thread.CurrentThread;
      yield return (object) MemoryStrategy.Instance.Compute(MemoryStrategyContextEnum.ApplicationStart);
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
        systemInformationService.AddInfo(typeof (BuildData).Name, (ISystemInfo) SRDebugger.Services.Info.Create("Version", (object) Application.version));
        systemInformationService.AddInfo(typeof (BuildData).Name, (ISystemInfo) SRDebugger.Services.Info.Create("Branch", (object) ScriptableObjectInstance<BuildData>.Instance.Branch));
        systemInformationService.AddInfo(typeof (BuildData).Name, (ISystemInfo) SRDebugger.Services.Info.Create("Time", (object) ScriptableObjectInstance<BuildData>.Instance.Time));
        systemInformationService.AddInfo(typeof (BuildData).Name, (ISystemInfo) SRDebugger.Services.Info.Create("Label", (Func<object>) (() => (object) InstanceByRequest<LabelService>.Instance.Label), false));
      }
      if (UnityEngine.Debug.isDebugBuild)
        SRDebug.Init();
      CursorService.Instance.Free = CursorService.Instance.Visible = true;
      MainMenuSetup.SetupMainMenuSettings();
      Stopwatch sw = new Stopwatch();
      sw.Restart();
      yield return (object) InitialiseServices.Initialise();
      sw.Stop();
      UnityEngine.Debug.Log((object) ObjectInfoUtility.GetStream().Append("[Engine]").Append("  ").Append("InitialiseServices").Append(" , elapsed : ").Append((object) sw.Elapsed));
      this.IsPaused = true;
      Action onInitialized = this.OnInitialized;
      if (onInitialized != null)
        onInitialized();
    }

    public void Terminate()
    {
      this.IsInitialized = this.IsInitialized ? false : throw new Exception();
      InitialiseServices.Terminate();
    }

    public void Exit()
    {
      Action onApplicationQuit = this.OnApplicationQuit;
      if (onApplicationQuit != null)
        onApplicationQuit();
      Application.Quit();
    }
  }
}
