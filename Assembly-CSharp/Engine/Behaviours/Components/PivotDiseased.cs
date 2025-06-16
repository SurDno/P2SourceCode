// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Components.PivotDiseased
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Engines.Controllers;
using Engine.Behaviours.Unity.Mecanim;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Components
{
  [RequireComponent(typeof (IKController))]
  [DisallowMultipleComponent]
  public class PivotDiseased : MonoBehaviour
  {
    private Animator animator;
    private int stanceLayerIndex = -1;
    private int reactionLayerIndex = -1;
    private float stanceOnPoseWeigth = 0.0f;

    public bool AttackStance { get; set; }

    private void Awake()
    {
      this.animator = this.GetComponent<Animator>();
      if ((Object) this.animator == (Object) null)
      {
        Debug.LogErrorFormat("{0} doesn't contain {1} unity component.", (object) this.gameObject.name, (object) typeof (Animator).Name);
      }
      else
      {
        this.stanceLayerIndex = this.animator.GetLayerIndex("Attack Stance Layer");
        this.reactionLayerIndex = this.animator.GetLayerIndex("Reaction Layer");
      }
    }

    public void Push(GameObject whoPushes)
    {
      DiseasedAnimatorState animatorState = DiseasedAnimatorState.GetAnimatorState(this.animator);
      animatorState.TriggerPlayerPush();
      Vector3 vector3 = this.transform.InverseTransformDirection(-whoPushes.transform.forward);
      float num = Mathf.Atan2(vector3.x, vector3.z) * 57.29578f;
      animatorState.PlayerPushAngle = num;
    }

    public void AttackPlayerFendOff(GameObject player)
    {
      DiseasedAnimatorState.GetAnimatorState(this.animator).TriggerPlayerFendOff();
    }

    private void Update()
    {
      this.stanceOnPoseWeigth = Mathf.Clamp01(this.stanceOnPoseWeigth + (this.AttackStance ? 1f : -1f) * Time.deltaTime);
      this.animator.SetLayerWeight(this.stanceLayerIndex, this.stanceOnPoseWeigth);
    }
  }
}
