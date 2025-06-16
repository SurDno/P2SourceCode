using Scripts.Data;

public class LocalGameSettingsData : ScriptableObjectInstance<LocalGameSettingsData>
{
  [Header("Build")]
  public string FirstDataName;
  [Space]
  public GameDataInfo[] Data;
  [Space]
  [Header("Common")]
  public bool LongDistance;
  public bool AutoPlay;
  public bool FastMove;
  public bool SkipCutscenes;
  public bool VirtualCursor;
  public bool FullLogger;
  public bool SteamAchievement;
  public bool ShowUnityProgress;
  public bool DisableGameObjectGroups;
  public bool AdditionalLogs;
  public bool UseCompressedTemplates;
  public bool ExternalSettings;
  public bool Demo;
  public bool UnityAsBuild;
  public bool DisableSteps;
  public bool EnableVirtualMachineDebug;
  public bool ChangeLocationLoadingWindow;
  public bool UseArrow;
}
