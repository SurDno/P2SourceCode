using Cofe.Meta;
using Engine.Common.Components.Regions;
using Engine.Source.Connections;
using Engine.Source.Services.Consoles;

[Initialisable]
public class GameSettingsData : ScriptableObjectInstance<GameSettingsData>
{
  [Header("Common")]
  public LayerMask FlamethrowerLayer;
  public LayerMask DynamicLayer;
  public LayerMask RagdollLayer;
  public LayerMask NpcHitCollidersLayer;
  public LayerMask DialogLayer;
  public LayerMask TriggerInteractLayer;
  public RegionEnum DefaultRegionName;
  public Color InteractableNormalTextColor;
  [FormerlySerializedAs("interactableDebugTextColor")]
  public Color InteractableDebugTextColor;
  [FormerlySerializedAs("interactableCrimeTextColor")]
  public Color InteractableCrimeTextColor;
  [FormerlySerializedAs("interactableDisabledTextColor")]
  public Color InteractableDisabledTextColor;
  public Vector3 WorldOffset;
  public PhysicMaterial NpcColliderMaterial;
  [GetSetConsoleCommand("child_gun_jam")]
  public float MaxLoaderProgress;
  [Header("Mixers")]
  public AudioMixerGroup NpcSpeechOutdoorMixer;
  public AudioMixerGroup NpcSpeechIndoorMixer;
  public AudioMixerGroup NpcFootOutdoorMixer;
  public AudioMixerGroup NpcFootIndoorMixer;
  public AudioMixerGroup NpcFootEffectsOutdoorMixer;
  public AudioMixerGroup NpcFootEffectsIndoorMixer;
  public AudioMixerGroup NpcWeaponOutdoorMixer;
  public AudioMixerGroup NpcWeaponIndoorMixer;
  public AudioMixerGroup NpcClothesOutdoorMixer;
  public AudioMixerGroup NpcClothesIndoorMixer;
  public AudioMixerGroup ProtagonistFootMixer;
  public AudioMixerGroup ProtagonistFootEffectsMixer;
  public AudioMixerGroup ProtagonistHitsMixer;
  [Header("Steps")]
  public LayerMask PuddlesLayer;
  public LayerMask StepsLayer;
  [Header("Times")]
  public DateTimeField OffsetTime;
  public TimeSpanField Morning;
  public TimeSpanField Day;
  public TimeSpanField Evening;
  public TimeSpanField Night;
  [Header("Navigation")]
  public int IndoorNavigationAreaIndex;
  public int OutdoorNavigationAreaIndex;
}
