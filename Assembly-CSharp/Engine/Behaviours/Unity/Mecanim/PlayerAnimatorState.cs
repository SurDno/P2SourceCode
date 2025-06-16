// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Unity.Mecanim.PlayerAnimatorState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Unity.Mecanim
{
  public class PlayerAnimatorState
  {
    private static Dictionary<Animator, PlayerAnimatorState> playerAnimatorStates = new Dictionary<Animator, PlayerAnimatorState>();
    public Animator Animator;

    public static void Clear() => PlayerAnimatorState.playerAnimatorStates.Clear();

    public static PlayerAnimatorState GetAnimatorState(Animator animator)
    {
      PlayerAnimatorState animatorState;
      if (!PlayerAnimatorState.playerAnimatorStates.TryGetValue(animator, out animatorState))
      {
        animatorState = new PlayerAnimatorState();
        animatorState.Animator = animator;
        PlayerAnimatorState.playerAnimatorStates[animator] = animatorState;
      }
      return animatorState;
    }

    public float HandsLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Hands Layer"), Mathf.Clamp01(value));
      }
    }

    public float KnifeLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Knife Layer"), Mathf.Clamp01(value));
      }
    }

    public float LockpickLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Lockpick Layer"), Mathf.Clamp01(value));
      }
    }

    public float ScalpelLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Scalpel Layer"), Mathf.Clamp01(value));
      }
    }

    public float RevolverLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Revolver Layer"), Mathf.Clamp01(value));
      }
    }

    public float RifleLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Rifle Layer"), Mathf.Clamp01(value));
      }
    }

    public float ShotgunLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Shotgun Layer"), Mathf.Clamp01(value));
      }
    }

    public float FlashlightLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Flashlight Layer"), Mathf.Clamp01(value));
      }
    }

    public float VisirLightLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Visir Layer"), Mathf.Clamp01(value));
      }
    }

    public float ReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float FlashlightReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Flashlight Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float ScalpelReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Scalpel Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float KnifeReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Knife Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float RifleReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Rifle Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float ShotgunReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Shotgun Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float RevolverReactionLayerWeight
    {
      set
      {
        this.Animator.SetLayerWeight(this.Animator.GetLayerIndex("Revolver Reaction Layer"), Mathf.Clamp01(value));
      }
    }

    public float WalkSpeed
    {
      set => this.Animator.SetFloat("Speed", value);
    }

    public void ResetAnimator()
    {
      this.Animator.SetInteger("Mecanim.State.Control", 0);
      this.Animator.SetInteger("AttackerPlayer.Weapon.State.Control", 0);
      this.Animator.SetTrigger("Reset");
    }

    public bool Fire
    {
      set => this.Animator.SetBool("AttackerPlayer.Weapon.Revolver.Fire", value);
    }

    public bool Reload
    {
      get => this.Animator.GetBool("AttackerPlayer.Weapon.Revolver.Reload");
      set => this.Animator.SetBool("AttackerPlayer.Weapon.Revolver.Reload", value);
    }

    public bool Empty
    {
      get => this.Animator.GetBool("AttackerPlayer.Weapon.Revolver.Empty");
      set => this.Animator.SetBool("AttackerPlayer.Weapon.Revolver.Empty", value);
    }

    public bool AlternativeFire
    {
      set => this.Animator.SetBool("AttackerPlayer.Weapon.Revolver.Alternative.Fire", value);
    }

    public int Rotate
    {
      set => this.Animator.SetInteger("Movable.Stance.Rotate", value);
    }

    public void Unholster() => this.Animator.SetTrigger("Triggers/Unholster");

    public void Holster() => this.Animator.SetTrigger("Triggers/Holster");

    public void PunchLowStamina()
    {
      this.Animator.SetTrigger("Triggers/CancelUppercut");
      this.Animator.SetTrigger("Triggers/PunchLowStamina");
    }

    public void PunchLeft()
    {
      this.Animator.SetTrigger("Triggers/CancelUppercut");
      this.Animator.SetTrigger("Triggers/PunchLeft");
    }

    public void PunchRight()
    {
      this.Animator.SetTrigger("Triggers/CancelUppercut");
      this.Animator.SetTrigger("Triggers/PunchRight");
    }

    public void PunchUppercut()
    {
      this.Animator.ResetTrigger("Triggers/CancelUppercut");
      this.Animator.SetTrigger("Triggers/PunchUppercut");
    }

    public void PunchBackstab()
    {
      this.Animator.ResetTrigger("Triggers/CancelUppercut");
      this.Animator.SetTrigger("Triggers/PunchBackstab");
    }

    public void Push() => this.Animator.SetTrigger("Triggers/Push");

    public void RiflePush() => this.Animator.SetTrigger("Triggers/RiflePush");

    public void RifleShot(int bullets, bool jam = false)
    {
      if (jam)
      {
        this.Animator.SetTrigger("Triggers/RifleJamShot");
      }
      else
      {
        this.Animator.SetTrigger("Triggers/RifleShot");
        this.Animator.SetInteger("RifleBullets", bullets);
      }
    }

    public void RifleShotCancel() => this.Animator.SetTrigger("Triggers/RifleShotCancel");

    public void RifleAim(bool aim) => this.Animator.SetBool("RifleIsAiming", aim);

    public void RifleReload(bool reload, bool hard = false)
    {
      this.Animator.ResetTrigger("Triggers/RifleRestore");
      if (reload)
        this.Animator.SetTrigger("Triggers/RifleReload");
      this.Animator.SetBool("RifleIsReloading", reload);
      this.Animator.SetBool("RifleIsHard", hard);
      this.Animator.ResetTrigger("Triggers/RifleCancelReload");
    }

    public void RifleCancelReload()
    {
      this.Animator.SetTrigger("Triggers/RifleCancelReload");
      this.Animator.ResetTrigger("Triggers/RifleReload");
    }

    public void RifleUnholster() => this.Animator.SetTrigger("Triggers/RifleUnholster");

    public void RifleHolster() => this.Animator.SetTrigger("Triggers/RifleHolster");

    public void RifleRestore()
    {
      this.Animator.ResetTrigger("Triggers/RifleReload");
      this.Animator.ResetTrigger("Triggers/RifleCancelReload");
      this.Animator.SetTrigger("Triggers/RifleRestore");
    }

    public void ShotgunPush() => this.Animator.SetTrigger("Triggers/ShotgunPush");

    public void ShotgunShot(bool empty = false, bool jam = false)
    {
      if (empty)
        this.Animator.SetTrigger("Triggers/ShotgunEmptyShot");
      else if (jam)
        this.Animator.SetTrigger("Triggers/ShotgunJamShot");
      else
        this.Animator.SetTrigger("Triggers/ShotgunShot");
    }

    public void ShotgunShotCancel() => this.Animator.SetTrigger("Triggers/ShotgunShotCancel");

    public void ShotgunAim(bool aim) => this.Animator.SetBool("ShotgunIsAiming", aim);

    public void ShotgunReload(bool reload, int startAmmo = 0)
    {
      this.Animator.ResetTrigger("Triggers/ShotgunRestore");
      if (reload)
        this.Animator.SetTrigger("Triggers/ShotgunReload");
      this.Animator.SetBool("ShotgunIsReloading", reload);
      this.Animator.SetInteger("ShotgunReloadStart", startAmmo);
      this.Animator.ResetTrigger("Triggers/ShotgunCancelReload");
    }

    public void ShotgunCancelReload()
    {
      this.Animator.SetTrigger("Triggers/ShotgunCancelReload");
      this.Animator.ResetTrigger("Triggers/ShotgunReload");
    }

    public void ShotgunUnholster() => this.Animator.SetTrigger("Triggers/ShotgunUnholster");

    public void ShotgunHolster() => this.Animator.SetTrigger("Triggers/ShotgunHolster");

    public void ShotgunRestore()
    {
      this.Animator.ResetTrigger("Triggers/ShotgunReload");
      this.Animator.ResetTrigger("Triggers/ShotgunCancelReload");
      this.Animator.SetTrigger("Triggers/ShotgunRestore");
    }

    public void RevolverPush() => this.Animator.SetTrigger("Triggers/RevolverPush");

    public void RevolverShot(bool empty, bool jam)
    {
      if (empty)
        this.Animator.SetTrigger("Triggers/RevolverEmptyShot");
      else if (jam)
        this.Animator.SetTrigger("Triggers/RevolverJamShot");
      else
        this.Animator.SetTrigger("Triggers/RevolverShot");
    }

    public void RevolverShotCancel() => this.Animator.SetTrigger("Triggers/RevolverShotCancel");

    public void RevolverAim(bool aim) => this.Animator.SetBool("RevolverIsAiming", aim);

    public void RevolverReload(bool reload, int bulletsStart = 0, bool firstReload = false)
    {
      this.Animator.ResetTrigger("Triggers/RevolverRestore");
      if (reload)
        this.Animator.SetTrigger("Triggers/RevolverReload");
      this.Animator.SetInteger("RevolverReloadStart", bulletsStart);
      this.Animator.SetBool("RevolverIsReloading", reload);
      this.Animator.SetBool("RevolverFirstReload", firstReload);
      this.Animator.ResetTrigger("Triggers/RevolverCancelReload");
    }

    public void RevolverCancelReload()
    {
      this.Animator.SetTrigger("Triggers/RevolverCancelReload");
      this.Animator.ResetTrigger("Triggers/RevolverReload");
    }

    public void RevolverUnholster() => this.Animator.SetTrigger("Triggers/RevolverUnholster");

    public void RevolverHolster() => this.Animator.SetTrigger("Triggers/RevolverHolster");

    public void RevolverRestore()
    {
      this.Animator.ResetTrigger("Triggers/RevolverReload");
      this.Animator.ResetTrigger("Triggers/RevolverCancelReload");
      this.Animator.SetTrigger("Triggers/RevolverRestore");
    }

    public void FlashlightPunch() => this.Animator.SetTrigger("Triggers/FlashlightPunch");

    public void FlashlightPunchLowStamina()
    {
      this.Animator.SetTrigger("Triggers/FlashlightPunchLowStamina");
    }

    public void FlashlightFire() => this.Animator.SetTrigger("Triggers/FlashlightFire");

    public void FlashlightUnholster() => this.Animator.SetTrigger("Triggers/FlashlightUnholster");

    public void FlashlightHolster() => this.Animator.SetTrigger("Triggers/FlashlightHolster");

    public float BlockStance
    {
      set => this.Animator.SetFloat(nameof (BlockStance), value);
    }

    public float Stamina
    {
      set => this.Animator.SetFloat(nameof (Stamina), value);
    }
  }
}
