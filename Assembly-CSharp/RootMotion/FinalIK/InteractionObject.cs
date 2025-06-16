using System;

namespace RootMotion.FinalIK
{
  [HelpURL("https://www.youtube.com/watch?v=r5jiZnsDH3M")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Interaction System/Interaction Object")]
  public class InteractionObject : MonoBehaviour
  {
    [Tooltip("If the Interaction System has a 'Look At' LookAtIK component assigned, will use it to make the character look at the specified Transform. If unassigned, will look at this GameObject.")]
    public Transform otherLookAtTarget;
    [Tooltip("The root Transform of the InteractionTargets. If null, will use this GameObject. GetComponentsInChildren<InteractionTarget>() will be used at initiation to find all InteractionTargets associated with this InteractionObject.")]
    public Transform otherTargetsRoot;
    [Tooltip("If assigned, all PositionOffset channels will be applied in the rotation space of this Transform. If not, they will be in the rotation space of the character.")]
    public Transform positionOffsetSpace;
    public WeightCurve[] weightCurves;
    public Multiplier[] multipliers;
    public InteractionEvent[] events;
    private InteractionTarget[] targets = new InteractionTarget[0];

    [ContextMenu("TUTORIAL VIDEO (PART 1: BASICS)")]
    private void OpenTutorial1()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=r5jiZnsDH3M");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 2: PICKING UP...)")]
    private void OpenTutorial2()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=eP9-zycoHLk");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 3: ANIMATION)")]
    private void OpenTutorial3()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=sQfB2RcT1T4&index=14&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 4: TRIGGERS)")]
    private void OpenTutorial4()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=-TDZpNjt2mk&index=15&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    public float length { get; private set; }

    public InteractionSystem lastUsedInteractionSystem { get; private set; }

    public void Initiate()
    {
      for (int index = 0; index < weightCurves.Length; ++index)
      {
        if (weightCurves[index].curve.length > 0)
          length = Mathf.Clamp(length, weightCurves[index].curve.keys[weightCurves[index].curve.length - 1].time, length);
      }
      for (int index = 0; index < events.Length; ++index)
        length = Mathf.Clamp(length, events[index].time, length);
      targets = targetsRoot.GetComponentsInChildren<InteractionTarget>();
    }

    public Transform lookAtTarget
    {
      get
      {
        return (UnityEngine.Object) otherLookAtTarget != (UnityEngine.Object) null ? otherLookAtTarget : this.transform;
      }
    }

    public InteractionTarget GetTarget(
      FullBodyBipedEffector effectorType,
      InteractionSystem interactionSystem)
    {
      if (interactionSystem.tag == string.Empty || interactionSystem.tag == "")
      {
        foreach (InteractionTarget target in targets)
        {
          if (target.effectorType == effectorType)
            return target;
        }
        return null;
      }
      foreach (InteractionTarget target in targets)
      {
        if (target.effectorType == effectorType && target.tag == interactionSystem.tag)
          return target;
      }
      return null;
    }

    public bool CurveUsed(WeightCurve.Type type)
    {
      foreach (WeightCurve weightCurve in weightCurves)
      {
        if (weightCurve.type == type)
          return true;
      }
      foreach (Multiplier multiplier in multipliers)
      {
        if (multiplier.result == type)
          return true;
      }
      return false;
    }

    public InteractionTarget[] GetTargets() => targets;

    public Transform GetTarget(FullBodyBipedEffector effectorType, string tag)
    {
      if (tag == string.Empty || tag == "")
        return GetTarget(effectorType);
      for (int index = 0; index < targets.Length; ++index)
      {
        if (targets[index].effectorType == effectorType && targets[index].tag == tag)
          return targets[index].transform;
      }
      return this.transform;
    }

    public void OnStartInteraction(InteractionSystem interactionSystem)
    {
      lastUsedInteractionSystem = interactionSystem;
    }

    public void Apply(
      IKSolverFullBodyBiped solver,
      FullBodyBipedEffector effector,
      InteractionTarget target,
      float timer,
      float weight)
    {
      for (int index = 0; index < weightCurves.Length; ++index)
      {
        float num = (UnityEngine.Object) target == (UnityEngine.Object) null ? 1f : target.GetValue(weightCurves[index].type);
        Apply(solver, effector, weightCurves[index].type, weightCurves[index].GetValue(timer), weight * num);
      }
      for (int index = 0; index < multipliers.Length; ++index)
      {
        if (multipliers[index].curve == multipliers[index].result && !Warning.logged)
          Warning.Log("InteractionObject Multiplier 'Curve' " + multipliers[index].curve + "and 'Result' are the same.", this.transform);
        int weightCurveIndex = GetWeightCurveIndex(multipliers[index].curve);
        if (weightCurveIndex != -1)
        {
          float num = (UnityEngine.Object) target == (UnityEngine.Object) null ? 1f : target.GetValue(multipliers[index].result);
          Apply(solver, effector, multipliers[index].result, multipliers[index].GetValue(weightCurves[weightCurveIndex], timer), weight * num);
        }
        else if (!Warning.logged)
          Warning.Log("InteractionObject Multiplier curve " + multipliers[index].curve + "does not exist.", this.transform);
      }
    }

    public float GetValue(
      WeightCurve.Type weightCurveType,
      InteractionTarget target,
      float timer)
    {
      int weightCurveIndex1 = GetWeightCurveIndex(weightCurveType);
      if (weightCurveIndex1 != -1)
      {
        float num = (UnityEngine.Object) target == (UnityEngine.Object) null ? 1f : target.GetValue(weightCurveType);
        return weightCurves[weightCurveIndex1].GetValue(timer) * num;
      }
      for (int index = 0; index < multipliers.Length; ++index)
      {
        if (multipliers[index].result == weightCurveType)
        {
          int weightCurveIndex2 = GetWeightCurveIndex(multipliers[index].curve);
          if (weightCurveIndex2 != -1)
          {
            float num = (UnityEngine.Object) target == (UnityEngine.Object) null ? 1f : target.GetValue(multipliers[index].result);
            return multipliers[index].GetValue(weightCurves[weightCurveIndex2], timer) * num;
          }
        }
      }
      return 0.0f;
    }

    public Transform targetsRoot
    {
      get
      {
        return (UnityEngine.Object) otherTargetsRoot != (UnityEngine.Object) null ? otherTargetsRoot : this.transform;
      }
    }

    private void Awake() => Initiate();

    private void Apply(
      IKSolverFullBodyBiped solver,
      FullBodyBipedEffector effector,
      WeightCurve.Type type,
      float value,
      float weight)
    {
      switch (type)
      {
        case WeightCurve.Type.PositionWeight:
          solver.GetEffector(effector).positionWeight = Mathf.Lerp(solver.GetEffector(effector).positionWeight, value, weight);
          break;
        case WeightCurve.Type.RotationWeight:
          solver.GetEffector(effector).rotationWeight = Mathf.Lerp(solver.GetEffector(effector).rotationWeight, value, weight);
          break;
        case WeightCurve.Type.PositionOffsetX:
          solver.GetEffector(effector).position += ((UnityEngine.Object) positionOffsetSpace != (UnityEngine.Object) null ? positionOffsetSpace.rotation : solver.GetRoot().rotation) * Vector3.right * value * weight;
          break;
        case WeightCurve.Type.PositionOffsetY:
          solver.GetEffector(effector).position += ((UnityEngine.Object) positionOffsetSpace != (UnityEngine.Object) null ? positionOffsetSpace.rotation : solver.GetRoot().rotation) * Vector3.up * value * weight;
          break;
        case WeightCurve.Type.PositionOffsetZ:
          solver.GetEffector(effector).position += ((UnityEngine.Object) positionOffsetSpace != (UnityEngine.Object) null ? positionOffsetSpace.rotation : solver.GetRoot().rotation) * Vector3.forward * value * weight;
          break;
        case WeightCurve.Type.Pull:
          solver.GetChain(effector).pull = Mathf.Lerp(solver.GetChain(effector).pull, value, weight);
          break;
        case WeightCurve.Type.Reach:
          solver.GetChain(effector).reach = Mathf.Lerp(solver.GetChain(effector).reach, value, weight);
          break;
        case WeightCurve.Type.Push:
          solver.GetChain(effector).push = Mathf.Lerp(solver.GetChain(effector).push, value, weight);
          break;
        case WeightCurve.Type.PushParent:
          solver.GetChain(effector).pushParent = Mathf.Lerp(solver.GetChain(effector).pushParent, value, weight);
          break;
      }
    }

    private Transform GetTarget(FullBodyBipedEffector effectorType)
    {
      for (int index = 0; index < targets.Length; ++index)
      {
        if (targets[index].effectorType == effectorType)
          return targets[index].transform;
      }
      return this.transform;
    }

    private int GetWeightCurveIndex(WeightCurve.Type weightCurveType)
    {
      for (int weightCurveIndex = 0; weightCurveIndex < weightCurves.Length; ++weightCurveIndex)
      {
        if (weightCurves[weightCurveIndex].type == weightCurveType)
          return weightCurveIndex;
      }
      return -1;
    }

    private int GetMultiplierIndex(WeightCurve.Type weightCurveType)
    {
      for (int multiplierIndex = 0; multiplierIndex < multipliers.Length; ++multiplierIndex)
      {
        if (multipliers[multiplierIndex].result == weightCurveType)
          return multiplierIndex;
      }
      return -1;
    }

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page10.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_interaction_object.html");
    }

    [Serializable]
    public class InteractionEvent
    {
      [Tooltip("The time of the event since interaction start.")]
      public float time;
      [Tooltip("If true, the interaction will be paused on this event. The interaction can be resumed by InteractionSystem.ResumeInteraction() or InteractionSystem.ResumeAll;")]
      public bool pause;
      [Tooltip("If true, the object will be parented to the effector bone on this event. Note that picking up like this can be done by only a single effector at a time. If you wish to pick up an object with both hands, see the Interaction PickUp2Handed demo scene.")]
      public bool pickUp;
      [Tooltip("The animations called on this event.")]
      public AnimatorEvent[] animations;
      [Tooltip("The messages sent on this event using GameObject.SendMessage().")]
      public Message[] messages;
      [Tooltip("The UnityEvent to invoke on this event.")]
      public UnityEvent unityEvent;

      public void Activate(Transform t)
      {
        unityEvent.Invoke();
        foreach (AnimatorEvent animation in animations)
          animation.Activate(pickUp);
        foreach (Message message in messages)
          message.Send(t);
      }
    }

    [Serializable]
    public class Message
    {
      [Tooltip("The name of the function called.")]
      public string function;
      [Tooltip("The recipient game object.")]
      public GameObject recipient;
      private const string empty = "";

      public void Send(Transform t)
      {
        if ((UnityEngine.Object) recipient == (UnityEngine.Object) null || function == string.Empty || function == "")
          return;
        recipient.SendMessage(function, (object) t, SendMessageOptions.RequireReceiver);
      }
    }

    [Serializable]
    public class AnimatorEvent
    {
      [Tooltip("The Animator component that will receive the AnimatorEvents.")]
      public Animator animator;
      [Tooltip("The Animation component that will receive the AnimatorEvents (Legacy).")]
      public Animation animation;
      [Tooltip("The name of the animation state.")]
      public string animationState;
      [Tooltip("The crossfading time.")]
      public float crossfadeTime = 0.3f;
      [Tooltip("The layer of the animation state (if using Legacy, the animation state will be forced to this layer).")]
      public int layer;
      [Tooltip("Should the animation always start from 0 normalized time?")]
      public bool resetNormalizedTime;
      private const string empty = "";

      public void Activate(bool pickUp)
      {
        if ((UnityEngine.Object) animator != (UnityEngine.Object) null)
        {
          if (pickUp)
            animator.applyRootMotion = false;
          this.Activate(animator);
        }
        if (!((UnityEngine.Object) animation != (UnityEngine.Object) null))
          return;
        this.Activate(animation);
      }

      private void Activate(Animator animator)
      {
        if (animationState == "")
          return;
        if (resetNormalizedTime)
          animator.CrossFade(animationState, crossfadeTime, layer, 0.0f);
        else
          animator.CrossFade(animationState, crossfadeTime, layer);
      }

      private void Activate(Animation animation)
      {
        if (animationState == "")
          return;
        if (resetNormalizedTime)
          animation[animationState].normalizedTime = 0.0f;
        animation[animationState].layer = layer;
        animation.CrossFade(animationState, crossfadeTime);
      }
    }

    [Serializable]
    public class WeightCurve
    {
      [Tooltip("The type of the curve (InteractionObject.WeightCurve.Type).")]
      public Type type;
      [Tooltip("The weight curve.")]
      public AnimationCurve curve;

      public float GetValue(float timer) => curve.Evaluate(timer);

      [Serializable]
      public enum Type
      {
        PositionWeight,
        RotationWeight,
        PositionOffsetX,
        PositionOffsetY,
        PositionOffsetZ,
        Pull,
        Reach,
        RotateBoneWeight,
        Push,
        PushParent,
        PoserWeight,
      }
    }

    [Serializable]
    public class Multiplier
    {
      [Tooltip("The curve type to multiply.")]
      public WeightCurve.Type curve;
      [Tooltip("The multiplier of the curve's value.")]
      public float multiplier = 1f;
      [Tooltip("The resulting value will be applied to this channel.")]
      public WeightCurve.Type result;

      public float GetValue(WeightCurve weightCurve, float timer)
      {
        return weightCurve.GetValue(timer) * multiplier;
      }
    }
  }
}
