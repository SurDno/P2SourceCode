using System;
using Engine.Source.Connections;

namespace Scripts.Data
{
  [Serializable]
  public class GameDataInfo
  {
    public string Name;
    public string GameName;
    public SceneAssetLink Scene;
    public IWeatherSnapshotSerializable WeatherSnapshot;
    public TimeSpanField SolarTime;
    public float SkyRotation = 145f;
    public int LoadingWindowGameDay;
    public bool HideLoadingWindow = true;
  }
}
