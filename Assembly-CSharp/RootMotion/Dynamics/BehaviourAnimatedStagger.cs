// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.BehaviourAnimatedStagger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourAnimatedStagger")]
  public class BehaviourAnimatedStagger : BehaviourBase
  {
    [Header("Master Properties")]
    public LayerMask groundLayers;
    public float animationBlendSpeed = 2f;
    public float animationMag = 5f;
    public float momentumMag = 0.1f;
    public float unbalancedMuscleWeightMlp = 0.05f;
    public float unbalancedMuscleDamperAdd = 1f;
    public bool dropProps;
    public float maxGetUpVelocity = 0.3f;
    public float minHipHeight = 0.3f;
    public SubBehaviourCOM centerOfMass;
    [Header("Muscle Group Properties")]
    public BehaviourAnimatedStagger.FallParams defaults;
    public BehaviourAnimatedStagger.FallParamsGroup[] groupOverrides;
    [Header("Events")]
    public BehaviourBase.PuppetEvent onUngrounded;
    public BehaviourBase.PuppetEvent onFallOver;
    public BehaviourBase.PuppetEvent onRest;
    [HideInInspector]
    public Vector3 moveVector;
    [HideInInspector]
    public bool isGrounded = true;
    [HideInInspector]
    public Vector3 forward;

    protected override void OnInitiate()
    {
      this.centerOfMass.Initiate((BehaviourBase) this, this.groundLayers);
    }

    protected override void OnActivate() => this.StartCoroutine(this.LoseBalance());

    public override void OnReactivate()
    {
    }

    private IEnumerator LoseBalance()
    {
      Muscle[] muscleArray1 = this.puppetMaster.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        BehaviourAnimatedStagger.FallParams fallParams = this.GetFallParams(m.props.group);
        m.state.pinWeightMlp = Mathf.Min(m.state.pinWeightMlp, fallParams.startPinWeightMlp);
        m.state.muscleWeightMlp = Mathf.Min(m.state.muscleWeightMlp, fallParams.startMuscleWeightMlp);
        m.state.muscleDamperAdd = -this.puppetMaster.muscleDamper;
        m = (Muscle) null;
      }
      muscleArray1 = (Muscle[]) null;
      this.puppetMaster.internalCollisions = true;
      bool done = false;
      while (!done)
      {
        Vector3 vel = Quaternion.Inverse(this.puppetMaster.targetRoot.rotation) * this.centerOfMass.direction * this.animationMag;
        this.moveVector = Vector3.Lerp(this.moveVector, vel, Time.deltaTime * this.animationBlendSpeed);
        this.moveVector = Vector3.ClampMagnitude(this.moveVector, 2f);
        Muscle[] muscleArray2 = this.puppetMaster.muscles;
        for (int index = 0; index < muscleArray2.Length; ++index)
        {
          Muscle m = muscleArray2[index];
          BehaviourAnimatedStagger.FallParams fallParams = this.GetFallParams(m.props.group);
          m.state.pinWeightMlp = Mathf.MoveTowards(m.state.pinWeightMlp, 0.0f, Time.deltaTime * fallParams.losePinSpeed);
          m.state.mappingWeightMlp = Mathf.MoveTowards(m.state.mappingWeightMlp, 1f, Time.deltaTime * this.animationBlendSpeed);
          m = (Muscle) null;
        }
        muscleArray2 = (Muscle[]) null;
        done = true;
        Muscle[] muscleArray3 = this.puppetMaster.muscles;
        for (int index = 0; index < muscleArray3.Length; ++index)
        {
          Muscle m = muscleArray3[index];
          if ((double) m.state.pinWeightMlp > 0.0 || (double) m.state.mappingWeightMlp < 1.0)
          {
            done = false;
            break;
          }
          m = (Muscle) null;
        }
        muscleArray3 = (Muscle[]) null;
        if ((double) this.puppetMaster.muscles[0].rigidbody.position.y - (double) this.puppetMaster.targetRoot.position.y < (double) this.minHipHeight)
          done = true;
        yield return (object) null;
        vel = new Vector3();
      }
      if (this.dropProps)
        this.RemoveMusclesOfGroup(Muscle.Group.Prop);
      if (!this.isGrounded)
      {
        Muscle[] muscleArray4 = this.puppetMaster.muscles;
        for (int index = 0; index < muscleArray4.Length; ++index)
        {
          Muscle m = muscleArray4[index];
          m.state.pinWeightMlp = 0.0f;
          m.state.muscleWeightMlp = 1f;
          m = (Muscle) null;
        }
        muscleArray4 = (Muscle[]) null;
        this.onUngrounded.Trigger(this.puppetMaster);
        if (this.onUngrounded.switchBehaviour)
          yield break;
      }
      this.moveVector = Vector3.zero;
      this.puppetMaster.mappingWeight = 1f;
      Muscle[] muscleArray5 = this.puppetMaster.muscles;
      for (int index = 0; index < muscleArray5.Length; ++index)
      {
        Muscle m = muscleArray5[index];
        m.state.pinWeightMlp = 0.0f;
        m.state.muscleWeightMlp = this.unbalancedMuscleWeightMlp;
        m.state.muscleDamperAdd = this.unbalancedMuscleDamperAdd;
        m = (Muscle) null;
      }
      muscleArray5 = (Muscle[]) null;
      this.onFallOver.Trigger(this.puppetMaster);
      if (!this.onFallOver.switchBehaviour)
      {
        yield return (object) new WaitForSeconds(1f);
        while ((double) this.puppetMaster.muscles[0].rigidbody.velocity.magnitude > (double) this.maxGetUpVelocity || !this.isGrounded)
          yield return (object) null;
        this.onRest.Trigger(this.puppetMaster);
        if (!this.onRest.switchBehaviour)
          ;
      }
    }

    private BehaviourAnimatedStagger.FallParams GetFallParams(Muscle.Group group)
    {
      foreach (BehaviourAnimatedStagger.FallParamsGroup groupOverride in this.groupOverrides)
      {
        foreach (Muscle.Group group1 in groupOverride.groups)
        {
          if (group1 == group)
            return groupOverride.fallParams;
        }
      }
      return this.defaults;
    }

    [Serializable]
    public struct FallParams
    {
      public float startPinWeightMlp;
      public float startMuscleWeightMlp;
      public float losePinSpeed;
    }

    [Serializable]
    public struct FallParamsGroup
    {
      public Muscle.Group[] groups;
      public BehaviourAnimatedStagger.FallParams fallParams;
    }
  }
}
