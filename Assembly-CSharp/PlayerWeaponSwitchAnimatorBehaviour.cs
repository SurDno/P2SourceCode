// Decompiled with JetBrains decompiler
// Type: PlayerWeaponSwitchAnimatorBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Engines.Services;
using Engine.Common.Components.AttackerPlayer;
using UnityEngine;

#nullable disable
public class PlayerWeaponSwitchAnimatorBehaviour : StateMachineBehaviour
{
  [SerializeField]
  private WeaponKind Kind;
  [SerializeField]
  private bool SwitchOn;

  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if ((double) animator.GetLayerWeight(layerIndex) <= 0.0)
      return;
    animator.gameObject.GetComponent<PlayerWeaponServiceNew>().OnWeaponSwitch(this.Kind, this.SwitchOn);
  }
}
