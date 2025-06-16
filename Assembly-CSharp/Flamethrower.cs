using System.Collections.Generic;

public class Flamethrower : MonoBehaviour
{
  [Header("Physics")]
  public float EventBufferingTime = 0.25f;
  [Header("Audio")]
  public AudioSource AudioSource = (AudioSource) null;
  public AudioClip StartClip = (AudioClip) null;
  public AudioClip EndClip = (AudioClip) null;
  [Header("State")]
  public bool Fire = false;
  private bool currentFire;
  private ParticleSystem[] particleSystems;
  private HashSet<IFlamable> movableSet0;
  private HashSet<IFlamable> movableSet1;
  private bool movableSetSwapped;
  private float movableSetSwapTime;
  private int targetCount;
  private float audioLoopDelay;

  public HashSet<IFlamable> MovablesHit
  {
    get => movableSetSwapped ? movableSet1 : movableSet0;
  }

  private HashSet<IFlamable> BackMovableSet
  {
    get => movableSetSwapped ? movableSet0 : movableSet1;
  }

  private void Awake()
  {
    particleSystems = this.GetComponentsInChildren<ParticleSystem>();
    for (int index = 0; index < particleSystems.Length; ++index)
    {
      ParticleSystem.CollisionModule collision = particleSystems[index].collision;
      if (collision.enabled && collision.sendCollisionMessages)
      {
        ref ParticleSystem.CollisionModule local = ref collision;
        local.collidesWith = (LayerMask) ((int) local.collidesWith | (int) ScriptableObjectInstance<GameSettingsData>.Instance.FlamethrowerLayer);
      }
    }
    movableSet0 = new HashSet<IFlamable>();
    movableSet1 = new HashSet<IFlamable>();
    movableSetSwapped = false;
    movableSetSwapTime = Time.time;
    audioLoopDelay = (Object) StartClip != (Object) null ? StartClip.length : 0.0f;
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
    if (!((Object) AudioSource != (Object) null))
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
      particleSystems[index].emission.enabled = enabled;
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
