using AssetDatabases;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Common.Weather;
using Engine.Impl.Weather;
using Engine.Proxy.Weather;
using Engine.Source.Services.Templates;
using Engine.Source.Services.Utilities;
using System;
using UnityEngine;

namespace Engine.Source.Commons
{
  public static class MainMenuSetup
  {
    public static void SetupMainMenuSettings()
    {
      if ((UnityEngine.Object) GameCamera.Instance == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "Game camera not found");
      }
      else
      {
        GameCamera.Instance.CameraTransform.SetPositionAndRotation(ScriptableObjectInstance<BuildSettings>.Instance.MainMenuCameraPosition, Quaternion.Euler(ScriptableObjectInstance<BuildSettings>.Instance.MainMenuCameraRotation));
        Guid id = ScriptableObjectInstance<BuildSettings>.Instance.MainMenuWeatherSnapshot.Id;
        string path = AssetDatabaseService.Instance.GetPath(id);
        if (path.IsNullOrEmpty())
        {
          Debug.LogError((object) ("Menu weather not found, id : " + (object) id));
        }
        else
        {
          WeatherSnapshot snapshot = (WeatherSnapshot) TemplateLoaderUtility.LoadObject(path);
          if (snapshot == null)
            return;
          WeatherSnapshotUtility.CopyFrom(snapshot);
          TOD_Sky instance = TOD_Sky.Instance;
          if ((UnityEngine.Object) instance == (UnityEngine.Object) null)
          {
            Debug.LogError((object) "Tod not found");
          }
          else
          {
            TimeSpan timeSpan = ScriptableObjectInstance<BuildSettings>.Instance.MainMenuSolarTime.Value;
            DateTime dateTime = ScriptableObjectInstance<GameSettingsData>.Instance.OffsetTime.Value;
            float mainMenuSkyRotation = ScriptableObjectInstance<BuildSettings>.Instance.MainMenuSkyRotation;
            instance.Cycle.DateTime = dateTime + timeSpan;
            instance.transform.localEulerAngles = new Vector3(0.0f, mainMenuSkyRotation, 0.0f);
            ITimeService service1 = ServiceLocator.GetService<ITimeService>();
            if (service1 != null)
              service1.SolarTime = timeSpan;
            IWeatherController service2 = ServiceLocator.GetService<IWeatherController>();
            if (service2 == null)
              return;
            WeatherUtility.SetDefaultWeather(service2, (IWeatherSnapshot) snapshot);
          }
        }
      }
    }
  }
}
