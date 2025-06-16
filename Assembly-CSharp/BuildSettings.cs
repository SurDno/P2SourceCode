// Decompiled with JetBrains decompiler
// Type: BuildSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Source.Connections;
using Engine.Source.Services.Consoles;
using Scripts.Data;
using UnityEngine;

#nullable disable
[Initialisable]
public class BuildSettings : ScriptableObjectInstance<BuildSettings>
{
  [GetSetConsoleCommand("first_data_name")]
  public string FirstDataName;
  [Space]
  public GameDataInfo[] Data;
  [Space]
  public IWeatherSnapshotSerializable MainMenuWeatherSnapshot;
  public TimeSpanField MainMenuSolarTime;
  public Vector3 MainMenuCameraPosition;
  public Vector3 MainMenuCameraRotation;
  public float MainMenuSkyRotation = 145f;
  [Space]
  [Space]
  public string[] TempLinkPaths;
}
