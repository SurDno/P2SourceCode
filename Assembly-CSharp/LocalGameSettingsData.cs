// Decompiled with JetBrains decompiler
// Type: LocalGameSettingsData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Scripts.Data;
using UnityEngine;

#nullable disable
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
