using Cofe.Meta;
using Engine.Source.Connections;
using Engine.Source.Services.Consoles;
using Scripts.Data;

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
