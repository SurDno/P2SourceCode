// Decompiled with JetBrains decompiler
// Type: Scripts.Data.GameDataInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Connections;
using System;

#nullable disable
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
