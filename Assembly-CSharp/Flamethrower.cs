using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
  [Header("Physics")]
  public float EventBufferingTime = 0.25f;
  [Header("Audio")]
  public AudioSource AudioSource;
  public AudioClip StartClip;
  public AudioClip EndClip;
  [Header("State")]
  public bool Fire;
  private bool currentFire;
  private ParticleSystem[] particleSystems;
  private HashSet<IFlamable> movableSet0;
  private HashSet<IFlamable> movableSet1;
  private bool movableSetSwapped;
  private float movableSetSwapTime;
  private int targetCount;
  private float audioLoopDelay;

  public HashSet<IFlamable> MovablesHit => movableSetSwapped ? movableSet1 : movableSet0;

  private HashSet<IFlamable> BackMovableSet => movableSetSwapped ? movableSet0 : movableSet1;

  private void Awake()
  {
    particleSystems = GetComponentsInChildren<ParticleSystem>();
    for (int index = 0; index < particleSystems.Length; ++index)
    {
      ParticleSystem.CollisionModule collision = particleSystems[index].collision;
      if (collision.enabled && collision.sendCollisionMessages)
      {
        ref ParticleSystem.CollisionModule local = ref collision;
        local.collidesWith = local.collidesWith | ScriptableObjectInstance<GameSettingsData>.Instance.FlamethrowerLayer;
      }
    }
    movableSet0 = [];
    movableSet1 = [];
    movableSetSwapped = false;
    movableSetSwapTime = Time.time;
    audioLoopDelay = StartClip != null ? StartClip.length : 0.0f;
    TurnParticles(currentFire);
  }

  private void OnParticleCollision(GameObject go)
  {
    if (!ScriptableObjectInstance<GameSettingsData>.Instance.FlamethrowerLayer.Contains(go.layer))
      return;
    IFlamable componentInParent = go.GetComponentInParent<IFlamable>();
    if (componentInParent != null)
      BackMovableSet.Add(componentInParent);
  }

  private void TurnAudio(bool enabled)
  {
    if (!(AudioSource != null))
      return;
    if (enabled)
    {
      AudioSource.PlayOneShot(StartClip);
      AudioSource.PlayDelayed(audioLoopDelay);
    }
    else
    {
      AudioSource.Stop();
      AudioSource.PlayOneShot(EndClip);
    }
  }

  private void TurnParticles(bool enabled)
  {
    for (int index = 0; index < particleSystems.Length; ++index) 
    {
      ParticleSystem.EmissionModule emission = particleSystems[index].emission;
      emission.enabled = enabled;
    }
  }

  public void SetIndoor(bool indoor)
  {
    AudioSource.outputAudioMixerGroup = indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponOutdoorMixer;
  }

  private void Update()
  {
    if (currentFire != Fire)
    {
      currentFire = Fire;
      TurnParticles(currentFire);
      TurnAudio(currentFire);
    }
    UpdateMovableSets(Time.time);
  }

  private void UpdateMovableSets(float time)
  {
    if (time < (double) movableSetSwapTime)
      return;
    movableSetSwapped = !movableSetSwapped;
    targetCount = MovablesHit.Count;
    BackMovableSet.Clear();
    movableSetSwapTime = time + EventBufferingTime;
  }
}
