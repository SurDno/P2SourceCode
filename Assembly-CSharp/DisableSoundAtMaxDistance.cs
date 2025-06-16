// Decompiled with JetBrains decompiler
// Type: DisableSoundAtMaxDistance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings.External;
using System.Collections;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class DisableSoundAtMaxDistance : MonoBehaviour
{
  [SerializeField]
  private AudioSource audioSource;

  private IEnumerator Start()
  {
    if ((Object) this.audioSource == (Object) null)
    {
      Debug.LogError((object) ("Null audio source, gameobject (need to fix by level designer), trying to get component: " + this.GetInfo()), (Object) this.gameObject);
      this.audioSource = this.GetComponent<AudioSource>();
      if ((Object) this.audioSource == (Object) null)
      {
        Debug.LogError((object) ("Can't get component: " + this.GetInfo()), (Object) this.gameObject);
        yield break;
      }
    }
    if (!ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
    {
      WaitForSeconds wait = new WaitForSeconds(ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.SoundUpdateDelay);
      while (true)
      {
        if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableAudio)
        {
          yield return (object) wait;
        }
        else
        {
          yield return (object) wait;
          this.UpdateEnable();
        }
      }
    }
  }

  private void UpdateEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (this.audioSource.maxDistance * this.audioSource.maxDistance);
    if (this.audioSource.enabled == flag)
      return;
    this.audioSource.enabled = flag;
  }
}
