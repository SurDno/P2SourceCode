using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class SoundPlayerPolyhedron : MonoBehaviour
{
  [SerializeField]
  private bool playInstantly;
  [SerializeField]
  private AudioClip[] clips = new AudioClip[0];
  [SerializeField]
  private float minRadius;
  [SerializeField]
  private float maxRadius;
  [SerializeField]
  private float minAngle;
  [SerializeField]
  private float maxAngle;
  [SerializeField]
  private float baseVolume;
  [SerializeField]
  private float soundFadeSpeed = 0.2f;
  private AudioSource audioSource;
  private float wait = 0.1f;
  private float waitingForUpdate = 0.0f;

  private void Start()
  {
    this.audioSource = this.GetComponent<AudioSource>();
    this.clips = ((IEnumerable<AudioClip>) this.clips).Where<AudioClip>((Func<AudioClip, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null)).ToArray<AudioClip>();
    if (this.clips.Length == 0)
      return;
    AudioClip audioClip = ((IEnumerable<AudioClip>) this.clips).Random<AudioClip>();
    this.audioSource.loop = true;
    this.audioSource.clip = audioClip;
    this.audioSource.PlayAndCheck();
    this.audioSource.volume = 0.0f;
  }

  private void Update()
  {
    if (this.clips.Length == 0)
      return;
    this.waitingForUpdate += Time.deltaTime;
    if ((double) this.waitingForUpdate < (double) this.wait)
      return;
    this.UpdateVolume();
    this.waitingForUpdate = 0.0f;
  }

  private void UpdateVolume()
  {
    ISimulation service = ServiceLocator.GetService<ISimulation>();
    if (service == null)
    {
      this.audioSource.enabled = false;
    }
    else
    {
      IEntity player = service.Player;
      if (player == null || (UnityEngine.Object) ((IEntityView) player).GameObject == (UnityEngine.Object) null)
      {
        this.audioSource.enabled = false;
      }
      else
      {
        Vector3 vector3 = (player != null ? ((IEntityView) player).Position : Vector3.zero) with
        {
          y = this.transform.position.y
        };
        float maxRadius = this.maxRadius;
        float magnitude = (this.transform.position - vector3).magnitude;
        bool flag = (double) magnitude < (double) maxRadius;
        if (this.audioSource.enabled != flag)
          this.audioSource.enabled = flag;
        if ((double) magnitude < (double) this.minRadius)
        {
          this.audioSource.volume = this.baseVolume;
        }
        else
        {
          float num = Vector3.Angle(((IEntityView) player).GameObject.transform.forward with
          {
            y = 0.0f
          }, (this.transform.position - vector3) with
          {
            y = 0.0f
          });
          this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, this.baseVolume * ((double) num < (double) this.minAngle ? 1f : 1f - Mathf.InverseLerp(this.minAngle, this.maxAngle, num)) * (1f - Mathf.InverseLerp(this.minRadius, this.maxRadius, magnitude)), this.wait * this.soundFadeSpeed);
        }
      }
    }
  }
}
