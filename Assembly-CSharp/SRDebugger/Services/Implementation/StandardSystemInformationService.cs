using System;
using System.Collections.Generic;
using SRF;
using SRF.Service;

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
      Debug.LogError((object) "[SystemInformationService] Category not found: {0}".Fmt(category));
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
        Info.Create("Operating System", (object) SystemInfo.operatingSystem),
        Info.Create("Device Name", (object) SystemInfo.deviceName, true),
        Info.Create("Device Type", (object) SystemInfo.deviceType),
        Info.Create("Device Model", (object) SystemInfo.deviceModel),
        Info.Create("CPU Type", (object) SystemInfo.processorType),
        Info.Create("CPU Count", (object) SystemInfo.processorCount),
        Info.Create("System Memory", SRFileUtil.GetBytesReadable((long) SystemInfo.systemMemorySize * 1024L * 1024L))
      });
      _info.Add("Unity", new List<ISystemInfo> {
        Info.Create("Version", (object) Application.unityVersion),
        Info.Create("Debug", (object) Debug.isDebugBuild),
        Info.Create("Logging", (object) Debug.unityLogger.filterLogType),
        Info.Create("Unity Pro", (object) Application.HasProLicense()),
        Info.Create("Genuine", "{0} ({1})".Fmt(Application.genuine ? "Yes" : (object) "No", Application.genuineCheckAvailable ? "Trusted" : (object) "Untrusted")),
        Info.Create("System Language", (object) Application.systemLanguage),
        Info.Create("Platform", (object) Application.platform),
        Info.Create("IL2CPP", "No"),
        Info.Create("Persistent Data Path", (object) Application.persistentDataPath)
      });
      _info.Add("Display", new List<ISystemInfo> {
        Info.Create("Resolution", (Func<object>) (() => Screen.width.ToString() + "x" + (object) Screen.height)),
        Info.Create("DPI", (Func<object>) (() => (object) Screen.dpi)),
        Info.Create("Fullscreen", (Func<object>) (() => (object) Screen.fullScreen)),
        Info.Create("Orientation", (Func<object>) (() => (object) Screen.orientation))
      });
      _info.Add("Runtime", new List<ISystemInfo> {
        Info.Create("Play Time", (Func<object>) (() => (object) Time.unscaledTime)),
        Info.Create("Level Play Time", (Func<object>) (() => (object) Time.timeSinceLevelLoad)),
        Info.Create("Current Level", (Func<object>) (() =>
        {
          Scene activeScene = SceneManager.GetActiveScene();
          return "{0} (Index: {1})".Fmt((object) activeScene.name, (object) activeScene.buildIndex);
        })),
        Info.Create("Quality Level", (Func<object>) (() => QualitySettings.names[QualitySettings.GetQualityLevel()] + " (" + (object) QualitySettings.GetQualityLevel() + ")"))
      });
      _info.Add("Features", new List<ISystemInfo> {
        Info.Create("Location", (object) SystemInfo.supportsLocationService),
        Info.Create("Accelerometer", (object) SystemInfo.supportsAccelerometer),
        Info.Create("Gyroscope", (object) SystemInfo.supportsGyroscope),
        Info.Create("Vibration", (object) SystemInfo.supportsVibration)
      });
      _info.Add("Graphics", new List<ISystemInfo> {
        Info.Create("Device Name", (object) SystemInfo.graphicsDeviceName),
        Info.Create("Device Vendor", (object) SystemInfo.graphicsDeviceVendor),
        Info.Create("Device Version", (object) SystemInfo.graphicsDeviceVersion),
        Info.Create("Max Tex Size", (object) SystemInfo.maxTextureSize),
        Info.Create("NPOT Support", (object) SystemInfo.npotSupport),
        Info.Create("Render Textures", "{0} ({1})".Fmt(SystemInfo.supportsRenderTextures ? "Yes" : (object) "No", (object) SystemInfo.supportedRenderTargetCount)),
        Info.Create("3D Textures", (object) SystemInfo.supports3DTextures),
        Info.Create("Compute Shaders", (object) SystemInfo.supportsComputeShaders),
        Info.Create("Image Effects", (object) SystemInfo.supportsImageEffects),
        Info.Create("Cubemaps", (object) SystemInfo.supportsRenderToCubemap),
        Info.Create("Shadows", (object) SystemInfo.supportsShadows),
        Info.Create("Stencil", (object) SystemInfo.supportsStencil),
        Info.Create("Sparse Textures", (object) SystemInfo.supportsSparseTextures)
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
