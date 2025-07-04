﻿using System;
using AssetDatabases;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Weather;
using Engine.Proxy.Weather;
using Engine.Source.Services.Templates;
using Engine.Source.Services.Utilities;
using UnityEngine;

namespace Engine.Source.Commons
{
  public static class MainMenuSetup
  {
    public static void SetupMainMenuSettings()
    {
      if (GameCamera.Instance == null)
      {
        Debug.LogError("Game camera not found");
      }
      else
      {
        GameCamera.Instance.CameraTransform.SetPositionAndRotation(ScriptableObjectInstance<BuildSettings>.Instance.MainMenuCameraPosition, Quaternion.Euler(ScriptableObjectInstance<BuildSettings>.Instance.MainMenuCameraRotation));
        Guid id = ScriptableObjectInstance<BuildSettings>.Instance.MainMenuWeatherSnapshot.Id;
        string path = AssetDatabaseService.Instance.GetPath(id);
        if (path.IsNullOrEmpty())
        {
          Debug.LogError("Menu weather not found, id : " + id);
        }
        else
        {
          WeatherSnapshot snapshot = (WeatherSnapshot) TemplateLoaderUtility.LoadObject(path);
          if (snapshot == null)
            return;
          WeatherSnapshotUtility.CopyFrom(snapshot);
          TOD_Sky instance = TOD_Sky.Instance;
          if (instance == null)
          {
            Debug.LogError("Tod not found");
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
            WeatherUtility.SetDefaultWeather(service2, snapshot);
          }
        }
      }
    }
  }
}
