using Cofe.Utility;
using Engine.Source.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Behaviours.Unity.Mecanim
{
  [DisallowMultipleComponent]
  public class AnimatorPivot3DRandomSound : StateMachineBehaviour
  {
    [SerializeField]
    private List<AudioClip> Clips;
    [SerializeField]
    private AudioMixerGroup AudioMixer2;
    [SerializeField]
    private float Volume = 1f;
    [SerializeField]
    private float Delay = 0.0f;
    [SerializeField]
    private float MinDistance = 1f;
    [SerializeField]
    private float MaxDistance = 10f;

    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      if (this.Clips == null || this.Clips.Count == 0 || (double) animator.GetLayerWeight(layerIndex) < 0.5)
        return;
      AudioClip clip = this.Clips[UnityEngine.Random.Range(0, this.Clips.Count)];
      CoroutineService.Instance.WaitSecond(this.Delay, (Action) (() =>
      {
        if (!((UnityEngine.Object) animator != (UnityEngine.Object) null))
          return;
        SoundUtility.PlayAudioClip3D(animator.transform, clip, this.AudioMixer2, this.Volume, this.MinDistance, this.MaxDistance, true, 0.0f, context: TypeUtility.GetTypeName(((object) this).GetType()) + " " + this.name);
      }));
    }
  }
}
