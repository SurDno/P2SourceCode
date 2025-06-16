using System.Collections.Generic;
using SRF;
using SRF.Service;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRDebugger.Services.Implementation
{
  [Service(typeof (ISystemInformationService))]
  public class StandardSystemInformationService : ISystemInformationService
  {
    private readonly Dictionary<string, List<ISystemInfo>> _info = new Dictionary<string, List<ISystemInfo>>();

    public StandardSystemInformationService() => CreateDefaultSet();

    public IEnumerable<string> GetCategories() => _info.Keys;

    public IList<ISystemInfo> GetInfo(string category)
    {
      List<ISystemInfo> info;
      if (_info.TryGetValue(category, out info))
        return info;
      Debug.LogError("[SystemInformationService] Category not found: {0}".Fmt(category));
      return new ISystemInfo[0];
    }

    public void AddInfo(string category, ISystemInfo info)
    {
      List<ISystemInfo> systemInfoList;
      if (!_info.TryGetValue(category, out systemInfoList))
      {
        systemInfoList = new List<ISystemInfo>();
        _info.Add(category, systemInfoList);
      }
      systemInfoList.Add(info);
    }

    public Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false)
    {
      Dictionary<string, Dictionary<string, object>> report = new Dictionary<string, Dictionary<string, object>>();
      foreach (KeyValuePair<string, List<ISystemInfo>> keyValuePair in _info)
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
      _info.Add("System", new List<ISystemInfo> {
        Info.Create("Operating System", SystemInfo.operatingSystem),
        Info.Create("Device Name", SystemInfo.deviceName, true),
        Info.Create("Device Type", SystemInfo.deviceType),
        Info.Create("Device Model", SystemInfo.deviceModel),
        Info.Create("CPU Type", SystemInfo.processorType),
        Info.Create("CPU Count", SystemInfo.processorCount),
        Info.Create("System Memory", SRFileUtil.GetBytesReadable(SystemInfo.systemMemorySize * 1024L * 1024L))
      });
      _info.Add("Unity", new List<ISystemInfo> {
        Info.Create("Version", Application.unityVersion),
        Info.Create("Debug", Debug.isDebugBuild),
        Info.Create("Logging", Debug.unityLogger.filterLogType),
        Info.Create("Unity Pro", Application.HasProLicense()),
        Info.Create("Genuine", "{0} ({1})".Fmt(Application.genuine ? "Yes" : (object) "No", Application.genuineCheckAvailable ? "Trusted" : (object) "Untrusted")),
        Info.Create("System Language", Application.systemLanguage),
        Info.Create("Platform", Application.platform),
        Info.Create("IL2CPP", "No"),
        Info.Create("Persistent Data Path", Application.persistentDataPath)
      });
      _info.Add("Display", new List<ISystemInfo> {
        Info.Create("Resolution", () => Screen.width + "x" + Screen.height),
        Info.Create("DPI", () => Screen.dpi),
        Info.Create("Fullscreen", () => Screen.fullScreen),
        Info.Create("Orientation", () => Screen.orientation)
      });
      _info.Add("Runtime", new List<ISystemInfo> {
        Info.Create("Play Time", () => Time.unscaledTime),
        Info.Create("Level Play Time", () => Time.timeSinceLevelLoad),
        Info.Create("Current Level", () =>
        {
          Scene activeScene = SceneManager.GetActiveScene();
          return "{0} (Index: {1})".Fmt(activeScene.name, activeScene.buildIndex);
        }),
        Info.Create("Quality Level", () => QualitySettings.names[QualitySettings.GetQualityLevel()] + " (" + QualitySettings.GetQualityLevel() + ")")
      });
      _info.Add("Features", new List<ISystemInfo> {
        Info.Create("Location", SystemInfo.supportsLocationService),
        Info.Create("Accelerometer", SystemInfo.supportsAccelerometer),
        Info.Create("Gyroscope", SystemInfo.supportsGyroscope),
        Info.Create("Vibration", SystemInfo.supportsVibration)
      });
      _info.Add("Graphics", new List<ISystemInfo> {
        Info.Create("Device Name", SystemInfo.graphicsDeviceName),
        Info.Create("Device Vendor", SystemInfo.graphicsDeviceVendor),
        Info.Create("Device Version", SystemInfo.graphicsDeviceVersion),
        Info.Create("Max Tex Size", SystemInfo.maxTextureSize),
        Info.Create("NPOT Support", SystemInfo.npotSupport),
        Info.Create("Render Textures", "{0} ({1})".Fmt(SystemInfo.supportsRenderTextures ? "Yes" : (object) "No", SystemInfo.supportedRenderTargetCount)),
        Info.Create("3D Textures", SystemInfo.supports3DTextures),
        Info.Create("Compute Shaders", SystemInfo.supportsComputeShaders),
        Info.Create("Image Effects", SystemInfo.supportsImageEffects),
        Info.Create("Cubemaps", SystemInfo.supportsRenderToCubemap),
        Info.Create("Shadows", SystemInfo.supportsShadows),
        Info.Create("Stencil", SystemInfo.supportsStencil),
        Info.Create("Sparse Textures", SystemInfo.supportsSparseTextures)
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
