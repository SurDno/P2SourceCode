// Decompiled with JetBrains decompiler
// Type: Flamethrower
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
  private bool currentFire = false;
  private ParticleSystem[] particleSystems;
  private HashSet<IFlamable> movableSet0;
  private HashSet<IFlamable> movableSet1;
  private bool movableSetSwapped;
  private float movableSetSwapTime;
  private int targetCount;
  private float audioLoopDelay;

  public HashSet<IFlamable> MovablesHit
  {
    get => this.movableSetSwapped ? this.movableSet1 : this.movableSet0;
  }

  private HashSet<IFlamable> BackMovableSet
  {
    get => this.movableSetSwapped ? this.movableSet0 : this.movableSet1;
  }

  private void Awake()
  {
    this.particleSystems = this.GetComponentsInChildren<ParticleSystem>();
    for (int index = 0; index < this.particleSystems.Length; ++index)
    {
      ParticleSystem.CollisionModule collision = this.particleSystems[index].collision;
      if (collision.enabled && collision.sendCollisionMessages)
      {
        ref ParticleSystem.CollisionModule local = ref collision;
        local.collidesWith = (LayerMask) ((int) local.collidesWith | (int) ScriptableObjectInstance<GameSettingsData>.Instance.FlamethrowerLayer);
      }
    }
    this.movableSet0 = new HashSet<IFlamable>();
    this.movableSet1 = new HashSet<IFlamable>();
    this.movableSetSwapped = false;
    this.movableSetSwapTime = Time.time;
    this.audioLoopDelay = (Object) this.StartClip != (Object) null ? this.StartClip.length : 0.0f;
    this.TurnParticles(this.currentFire);
  }

  private void OnParticleCollision(GameObject go)
  {
    if (!ScriptableObjectInstance<GameSettingsData>.Instance.FlamethrowerLayer.Contains(go.layer))
      return;
    IFlamable componentInParent = go.GetComponentInParent<IFlamable>();
    if (componentInParent != null)
      this.BackMovableSet.Add(componentInParent);
  }

  private void TurnAudio(bool enabled)
  {
    if (!((Object) this.AudioSource != (Object) null))
      return;
    if (enabled)
    {
      this.AudioSource.PlayOneShot(this.StartClip);
      this.AudioSource.PlayDelayed(this.audioLoopDelay);
    }
    else
    {
      this.AudioSource.Stop();
      this.AudioSource.PlayOneShot(this.EndClip);
    }
  }

  private void TurnParticles(bool enabled)
  {
    for (int index = 0; index < this.particleSystems.Length; ++index)
      this.particleSystems[index].emission.enabled = enabled;
  }

  public void SetIndoor(bool indoor)
  {
    this.AudioSource.outputAudioMixerGroup = indoor ? ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponIndoorMixer : ScriptableObjectInstance<GameSettingsData>.Instance.NpcWeaponOutdoorMixer;
  }

  private void Update()
  {
    if (this.currentFire != this.Fire)
    {
      this.currentFire = this.Fire;
      this.TurnParticles(this.currentFire);
      this.TurnAudio(this.currentFire);
    }
    this.UpdateMovableSets(Time.time);
  }

  private void UpdateMovableSets(float time)
  {
    if ((double) time < (double) this.movableSetSwapTime)
      return;
    this.movableSetSwapped = !this.movableSetSwapped;
    this.targetCount = this.MovablesHit.Count;
    this.BackMovableSet.Clear();
    this.movableSetSwapTime = time + this.EventBufferingTime;
  }
}
