using System;
using System.Collections;
using UnityEngine;

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
    public FallParams defaults;
    public FallParamsGroup[] groupOverrides;
    [Header("Events")]
    public PuppetEvent onUngrounded;
    public PuppetEvent onFallOver;
    public PuppetEvent onRest;
    [HideInInspector]
    public Vector3 moveVector;
    [HideInInspector]
    public bool isGrounded = true;
    [HideInInspector]
    public Vector3 forward;

    protected override void OnInitiate()
    {
      centerOfMass.Initiate(this, groundLayers);
    }

    protected override void OnActivate() => StartCoroutine(LoseBalance());

    public override void OnReactivate()
    {
    }

    private IEnumerator LoseBalance()
    {
      Muscle[] muscleArray1 = puppetMaster.muscles;
      for (int index = 0; index < muscleArray1.Length; ++index)
      {
        Muscle m = muscleArray1[index];
        FallParams fallParams = GetFallParams(m.props.group);
        m.state.pinWeightMlp = Mathf.Min(m.state.pinWeightMlp, fallParams.startPinWeightMlp);
        m.state.muscleWeightMlp = Mathf.Min(m.state.muscleWeightMlp, fallParams.startMuscleWeightMlp);
        m.state.muscleDamperAdd = -puppetMaster.muscleDamper;
        m = null;
      }
      muscleArray1 = null;
      puppetMaster.internalCollisions = true;
      bool done = false;
      while (!done)
      {
        Vector3 vel = Quaternion.Inverse(puppetMaster.targetRoot.rotation) * centerOfMass.direction * animationMag;
        moveVector = Vector3.Lerp(moveVector, vel, Time.deltaTime * animationBlendSpeed);
        moveVector = Vector3.ClampMagnitude(moveVector, 2f);
        Muscle[] muscleArray2 = puppetMaster.muscles;
        for (int index = 0; index < muscleArray2.Length; ++index)
        {
          Muscle m = muscleArray2[index];
          FallParams fallParams = GetFallParams(m.props.group);
          m.state.pinWeightMlp = Mathf.MoveTowards(m.state.pinWeightMlp, 0.0f, Time.deltaTime * fallParams.losePinSpeed);
          m.state.mappingWeightMlp = Mathf.MoveTowards(m.state.mappingWeightMlp, 1f, Time.deltaTime * animationBlendSpeed);
          m = null;
        }
        muscleArray2 = null;
        done = true;
        Muscle[] muscleArray3 = puppetMaster.muscles;
        for (int index = 0; index < muscleArray3.Length; ++index)
        {
          Muscle m = muscleArray3[index];
          if (m.state.pinWeightMlp > 0.0 || m.state.mappingWeightMlp < 1.0)
          {
            done = false;
            break;
          }
          m = null;
        }
        muscleArray3 = null;
        if (puppetMaster.muscles[0].rigidbody.position.y - (double) puppetMaster.targetRoot.position.y < minHipHeight)
          done = true;
        yield return null;
        vel = new Vector3();
      }
      if (dropProps)
        RemoveMusclesOfGroup(Muscle.Group.Prop);
      if (!isGrounded)
      {
        Muscle[] muscleArray4 = puppetMaster.muscles;
        for (int index = 0; index < muscleArray4.Length; ++index)
        {
          Muscle m = muscleArray4[index];
          m.state.pinWeightMlp = 0.0f;
          m.state.muscleWeightMlp = 1f;
          m = null;
        }
        muscleArray4 = null;
        onUngrounded.Trigger(puppetMaster);
        if (onUngrounded.switchBehaviour)
          yield break;
      }
      moveVector = Vector3.zero;
      puppetMaster.mappingWeight = 1f;
      Muscle[] muscleArray5 = puppetMaster.muscles;
      for (int index = 0; index < muscleArray5.Length; ++index)
      {
        Muscle m = muscleArray5[index];
        m.state.pinWeightMlp = 0.0f;
        m.state.muscleWeightMlp = unbalancedMuscleWeightMlp;
        m.state.muscleDamperAdd = unbalancedMuscleDamperAdd;
        m = null;
      }
      muscleArray5 = null;
      onFallOver.Trigger(puppetMaster);
      if (!onFallOver.switchBehaviour)
      {
        yield return new WaitForSeconds(1f);
        while (puppetMaster.muscles[0].rigidbody.velocity.magnitude > (double) maxGetUpVelocity || !isGrounded)
          yield return null;
        onRest.Trigger(puppetMaster);
        if (!onRest.switchBehaviour)
          ;
      }
    }

    private FallParams GetFallParams(Muscle.Group group)
    {
      foreach (FallParamsGroup groupOverride in groupOverrides)
      {
        foreach (Muscle.Group group1 in groupOverride.groups)
        {
          if (group1 == group)
            return groupOverride.fallParams;
        }
      }
      return defaults;
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
      public FallParams fallParams;
    }
  }
}
