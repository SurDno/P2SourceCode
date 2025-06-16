// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.AnimatorPivot3DSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Behaviours.Components;
using Engine.Source.Audio;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  [DisallowMultipleComponent]
  public class AnimatorPivot3DSound : StateMachineBehaviour
  {
    [SerializeField]
    private AudioClip Clip;
    [SerializeField]
    private AudioMixerGroup AudioMixer2;
    [SerializeField]
    private float Volume = 1f;
    [SerializeField]
    private float MinDistance = 1f;
    [SerializeField]
    private float MaxDistance = 10f;
    [SerializeField]
    private Pivot.AimWeaponType BodyPart = Pivot.AimWeaponType.Head;

    public override void OnStateEnter(
      Animator animator,
      AnimatorStateInfo stateInfo,
      int layerIndex)
    {
      if ((UnityEngine.Object) this.Clip == (UnityEngine.Object) null)
        return;
      Pivot pivot = animator.gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
        Debug.LogWarningFormat("{0} doesn't contain {1} unity component.", (object) animator.gameObject.name, (object) typeof (Pivot).Name);
      else
        CoroutineService.Instance.WaitFrame((Action) (() => SoundUtility.PlayAudioClip3D(pivot.GetAimTransform(this.BodyPart), this.Clip, this.AudioMixer2, this.Volume, this.MinDistance, this.MaxDistance, true, 0.0f, context: TypeUtility.GetTypeName(((object) this).GetType()) + " " + this.name)));
    }
  }
}
