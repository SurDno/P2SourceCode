using SRF;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRDebugger.Services.Implementation
{
  [SRF.Service.Service(typeof (ISystemInformationService))]
  public class StandardSystemInformationService : ISystemInformationService
  {
    private readonly Dictionary<string, List<ISystemInfo>> _info = new Dictionary<string, List<ISystemInfo>>();

    public StandardSystemInformationService() => this.CreateDefaultSet();

    public IEnumerable<string> GetCategories() => (IEnumerable<string>) this._info.Keys;

    public IList<ISystemInfo> GetInfo(string category)
    {
      List<ISystemInfo> info;
      if (this._info.TryGetValue(category, out info))
        return (IList<ISystemInfo>) info;
      Debug.LogError((object) "[SystemInformationService] Category not found: {0}".Fmt((object) category));
      return (IList<ISystemInfo>) new ISystemInfo[0];
    }

    public void AddInfo(string category, ISystemInfo info)
    {
      List<ISystemInfo> systemInfoList;
      if (!this._info.TryGetValue(category, out systemInfoList))
      {
        systemInfoList = new List<ISystemInfo>();
        this._info.Add(category, systemInfoList);
      }
      systemInfoList.Add(info);
    }

    public Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false)
    {
      Dictionary<string, Dictionary<string, object>> report = new Dictionary<string, Dictionary<string, object>>();
      foreach (KeyValuePair<string, List<ISystemInfo>> keyValuePair in this._info)
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        foreach (ISystemInfo systemInfo in keyValuePair.Value)
        {
          if (!systemInfo.IsPrivate || includePrivate)
            dictionary.Add(systemInfo.Title, systemInfo.Value);
        }
        report.Add(keyValuePair.Key, dictionary);
      }
      return report;
    }

    private void CreateDefaultSet()
    {
      this._info.Add("System", new List<ISystemInfo>()
      {
        (ISystemInfo) SRDebugger.Services.Info.Create("Operating System", (object) SystemInfo.operatingSystem),
        (ISystemInfo) SRDebugger.Services.Info.Create("Device Name", (object) SystemInfo.deviceName, true),
        (ISystemInfo) SRDebugger.Services.Info.Create("Device Type", (object) SystemInfo.deviceType),
        (ISystemInfo) SRDebugger.Services.Info.Create("Device Model", (object) SystemInfo.deviceModel),
        (ISystemInfo) SRDebugger.Services.Info.Create("CPU Type", (object) SystemInfo.processorType),
        (ISystemInfo) SRDebugger.Services.Info.Create("CPU Count", (object) SystemInfo.processorCount),
        (ISystemInfo) SRDebugger.Services.Info.Create("System Memory", (object) SRFileUtil.GetBytesReadable((long) SystemInfo.systemMemorySize * 1024L * 1024L))
      });
      this._info.Add("Unity", new List<ISystemInfo>()
      {
        (ISystemInfo) SRDebugger.Services.Info.Create("Version", (object) Application.unityVersion),
        (ISystemInfo) SRDebugger.Services.Info.Create("Debug", (object) Debug.isDebugBuild),
        (ISystemInfo) SRDebugger.Services.Info.Create("Logging", (object) Debug.unityLogger.filterLogType),
        (ISystemInfo) SRDebugger.Services.Info.Create("Unity Pro", (object) Application.HasProLicense()),
        (ISystemInfo) SRDebugger.Services.Info.Create("Genuine", (object) "{0} ({1})".Fmt(Application.genuine ? (object) "Yes" : (object) "No", Application.genuineCheckAvailable ? (object) "Trusted" : (object) "Untrusted")),
        (ISystemInfo) SRDebugger.Services.Info.Create("System Language", (object) Application.systemLanguage),
        (ISystemInfo) SRDebugger.Services.Info.Create("Platform", (object) Application.platform),
        (ISystemInfo) SRDebugger.Services.Info.Create("IL2CPP", (object) "No"),
        (ISystemInfo) SRDebugger.Services.Info.Create("Persistent Data Path", (object) Application.persistentDataPath)
      });
      this._info.Add("Display", new List<ISystemInfo>()
      {
        (ISystemInfo) SRDebugger.Services.Info.Create("Resolution", (Func<object>) (() => (object) (Screen.width.ToString() + "x" + (object) Screen.height)), false),
        (ISystemInfo) SRDebugger.Services.Info.Create("DPI", (Func<object>) (() => (object) Screen.dpi), false),
        (ISystemInfo) SRDebugger.Services.Info.Create("Fullscreen", (Func<object>) (() => (object) Screen.fullScreen), false),
        (ISystemInfo) SRDebugger.Services.Info.Create("Orientation", (Func<object>) (() => (object) Screen.orientation), false)
      });
      this._info.Add("Runtime", new List<ISystemInfo>()
      {
        (ISystemInfo) SRDebugger.Services.Info.Create("Play Time", (Func<object>) (() => (object) Time.unscaledTime), false),
        (ISystemInfo) SRDebugger.Services.Info.Create("Level Play Time", (Func<object>) (() => (object) Time.timeSinceLevelLoad), false),
        (ISystemInfo) SRDebugger.Services.Info.Create("Current Level", (Func<object>) (() =>
        {
          Scene activeScene = SceneManager.GetActiveScene();
          return (object) "{0} (Index: {1})".Fmt((object) activeScene.name, (object) activeScene.buildIndex);
        }), false),
        (ISystemInfo) SRDebugger.Services.Info.Create("Quality Level", (Func<object>) (() => (object) (QualitySettings.names[QualitySettings.GetQualityLevel()] + " (" + (object) QualitySettings.GetQualityLevel() + ")")), false)
      });
      this._info.Add("Features", new List<ISystemInfo>()
      {
        (ISystemInfo) SRDebugger.Services.Info.Create("Location", (object) SystemInfo.supportsLocationService),
        (ISystemInfo) SRDebugger.Services.Info.Create("Accelerometer", (object) SystemInfo.supportsAccelerometer),
        (ISystemInfo) SRDebugger.Services.Info.Create("Gyroscope", (object) SystemInfo.supportsGyroscope),
        (ISystemInfo) SRDebugger.Services.Info.Create("Vibration", (object) SystemInfo.supportsVibration)
      });
      this._info.Add("Graphics", new List<ISystemInfo>()
      {
        (ISystemInfo) SRDebugger.Services.Info.Create("Device Name", (object) SystemInfo.graphicsDeviceName),
        (ISystemInfo) SRDebugger.Services.Info.Create("Device Vendor", (object) SystemInfo.graphicsDeviceVendor),
        (ISystemInfo) SRDebugger.Services.Info.Create("Device Version", (object) SystemInfo.graphicsDeviceVersion),
        (ISystemInfo) SRDebugger.Services.Info.Create("Max Tex Size", (object) SystemInfo.maxTextureSize),
        (ISystemInfo) SRDebugger.Services.Info.Create("NPOT Support", (object) SystemInfo.npotSupport),
        (ISystemInfo) SRDebugger.Services.Info.Create("Render Textures", (object) "{0} ({1})".Fmt(SystemInfo.supportsRenderTextures ? (object) "Yes" : (object) "No", (object) SystemInfo.supportedRenderTargetCount)),
        (ISystemInfo) SRDebugger.Services.Info.Create("3D Textures", (object) SystemInfo.supports3DTextures),
        (ISystemInfo) SRDebugger.Services.Info.Create("Compute Shaders", (object) SystemInfo.supportsComputeShaders),
        (ISystemInfo) SRDebugger.Services.Info.Create("Image Effects", (object) SystemInfo.supportsImageEffects),
        (ISystemInfo) SRDebugger.Services.Info.Create("Cubemaps", (object) SystemInfo.supportsRenderToCubemap),
        (ISystemInfo) SRDebugger.Services.Info.Create("Shadows", (object) SystemInfo.supportsShadows),
        (ISystemInfo) SRDebugger.Services.Info.Create("Stencil", (object) SystemInfo.supportsStencil),
        (ISystemInfo) SRDebugger.Services.Info.Create("Sparse Textures", (object) SystemInfo.supportsSparseTextures)
      });
    }

    private static string GetCloudManifestPrettyName(string name)
    {
      switch (name)
      {
        case "scmCommitId":
          return "Commit";
        case "scmBranch":
          return "Branch";
        case "cloudBuildTargetName":
          return "Build Target";
        case "buildStartTime":
          return "Build Date";
        default:
          return name.Substring(0, 1).ToUpper() + name.Substring(1);
      }
    }
  }
}
