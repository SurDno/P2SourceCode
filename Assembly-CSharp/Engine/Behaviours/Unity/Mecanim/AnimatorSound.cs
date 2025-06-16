// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  [Serializable]
  public class AnimatorSound : StateMachineBehaviour
  {
    public AudioClip[] Clips;
    public float Volume = 1f;
    public float Delay = 0.0f;
    private GameObject audioGO;

    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      PivotHead component = animator.gameObject.GetComponent<PivotHead>();
      this.audioGO = new GameObject("Sound3D");
      this.audioGO.transform.SetParent(component.Head, false);
      AudioSource audioSource = this.audioGO.AddComponent<AudioSource>();
      audioSource.clip = this.Clips[UnityEngine.Random.Range(0, this.Clips.Length)];
      audioSource.volume = this.Volume * 0.5f;
      audioSource.spatialBlend = 1f;
      audioSource.minDistance = 0.4f;
      audioSource.PlayDelayed(this.Delay);
    }

    public override void OnStateExit(
      Animator animator,
      AnimatorStateInfo animatorStateInfo,
      int layerIndex)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.audioGO);
    }
  }
}
