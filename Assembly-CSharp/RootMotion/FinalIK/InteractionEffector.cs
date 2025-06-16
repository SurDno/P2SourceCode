using System;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{
  [Serializable]
  public class InteractionEffector
  {
    private Poser poser;
    private IKEffector effector;
    private float timer;
    private float length;
    private float weight;
    private float fadeInSpeed;
    private float defaultPositionWeight;
    private float defaultRotationWeight;
    private float defaultPull;
    private float defaultReach;
    private float defaultPush;
    private float defaultPushParent;
    private float resetTimer;
    private bool positionWeightUsed;
    private bool rotationWeightUsed;
    private bool pullUsed;
    private bool reachUsed;
    private bool pushUsed;
    private bool pushParentUsed;
    private bool pickedUp;
    private bool defaults;
    private bool pickUpOnPostFBBIK;
    private Vector3 pickUpPosition;
    private Vector3 pausePositionRelative;
    private Quaternion pickUpRotation;
    private Quaternion pauseRotationRelative;
    private InteractionTarget interactionTarget;
    private Transform target;
    private List<bool> triggered = new List<bool>();
    private InteractionSystem interactionSystem;
    private bool started;

    public FullBodyBipedEffector effectorType { get; private set; }

    public bool isPaused { get; private set; }

    public InteractionObject interactionObject { get; private set; }

    public bool inInteraction => interactionObject != null;

    public InteractionEffector(FullBodyBipedEffector effectorType)
    {
      this.effectorType = effectorType;
    }

    public void Initiate(InteractionSystem interactionSystem)
    {
      this.interactionSystem = interactionSystem;
      if (effector == null)
      {
        effector = interactionSystem.ik.solver.GetEffector(effectorType);
        poser = effector.bone.GetComponent<Poser>();
      }
      StoreDefaults();
    }

    private void StoreDefaults()
    {
      defaultPositionWeight = interactionSystem.ik.solver.GetEffector(effectorType).positionWeight;
      defaultRotationWeight = interactionSystem.ik.solver.GetEffector(effectorType).rotationWeight;
      defaultPull = interactionSystem.ik.solver.GetChain(effectorType).pull;
      defaultReach = interactionSystem.ik.solver.GetChain(effectorType).reach;
      defaultPush = interactionSystem.ik.solver.GetChain(effectorType).push;
      defaultPushParent = interactionSystem.ik.solver.GetChain(effectorType).pushParent;
    }

    public bool ResetToDefaults(float speed)
    {
      if (inInteraction || isPaused || defaults)
        return false;
      resetTimer = Mathf.MoveTowards(resetTimer, 0.0f, Time.deltaTime * speed);
      if (effector.isEndEffector)
      {
        if (pullUsed)
          interactionSystem.ik.solver.GetChain(effectorType).pull = Mathf.Lerp(defaultPull, interactionSystem.ik.solver.GetChain(effectorType).pull, resetTimer);
        if (reachUsed)
          interactionSystem.ik.solver.GetChain(effectorType).reach = Mathf.Lerp(defaultReach, interactionSystem.ik.solver.GetChain(effectorType).reach, resetTimer);
        if (pushUsed)
          interactionSystem.ik.solver.GetChain(effectorType).push = Mathf.Lerp(defaultPush, interactionSystem.ik.solver.GetChain(effectorType).push, resetTimer);
        if (pushParentUsed)
          interactionSystem.ik.solver.GetChain(effectorType).pushParent = Mathf.Lerp(defaultPushParent, interactionSystem.ik.solver.GetChain(effectorType).pushParent, resetTimer);
      }
      if (positionWeightUsed)
        effector.positionWeight = Mathf.Lerp(defaultPositionWeight, effector.positionWeight, resetTimer);
      if (rotationWeightUsed)
        effector.rotationWeight = Mathf.Lerp(defaultRotationWeight, effector.rotationWeight, resetTimer);
      if (resetTimer <= 0.0)
      {
        pullUsed = false;
        reachUsed = false;
        pushUsed = false;
        pushParentUsed = false;
        positionWeightUsed = false;
        rotationWeightUsed = false;
        defaults = true;
      }
      return true;
    }

    public bool Pause()
    {
      if (!inInteraction)
        return false;
      isPaused = true;
      pausePositionRelative = target.InverseTransformPoint(effector.position);
      pauseRotationRelative = Quaternion.Inverse(target.rotation) * effector.rotation;
      if (interactionSystem.OnInteractionPause != null)
        interactionSystem.OnInteractionPause(effectorType, interactionObject);
      return true;
    }

    public bool Resume()
    {
      if (!inInteraction)
        return false;
      isPaused = false;
      if (interactionSystem.OnInteractionResume != null)
        interactionSystem.OnInteractionResume(effectorType, interactionObject);
      return true;
    }

    public bool Start(
      InteractionObject interactionObject,
      string tag,
      float fadeInTime,
      bool interrupt)
    {
      if (!inInteraction)
      {
        effector.position = effector.bone.position;
        effector.rotation = effector.bone.rotation;
      }
      else
      {
        if (!interrupt)
          return false;
        defaults = false;
      }
      target = interactionObject.GetTarget(effectorType, tag);
      if (target == null)
        return false;
      interactionTarget = target.GetComponent<InteractionTarget>();
      this.interactionObject = interactionObject;
      if (interactionSystem.OnInteractionStart != null)
        interactionSystem.OnInteractionStart(effectorType, interactionObject);
      interactionObject.OnStartInteraction(interactionSystem);
      triggered.Clear();
      for (int index = 0; index < interactionObject.events.Length; ++index)
        triggered.Add(false);
      if (poser != null)
      {
        if (poser.poseRoot == null)
          poser.weight = 0.0f;
        poser.poseRoot = !(interactionTarget != null) ? null : target.transform;
        poser.AutoMapping();
      }
      positionWeightUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.PositionWeight);
      rotationWeightUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.RotationWeight);
      pullUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.Pull);
      reachUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.Reach);
      pushUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.Push);
      pushParentUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.PushParent);
      if (defaults)
        StoreDefaults();
      timer = 0.0f;
      weight = 0.0f;
      fadeInSpeed = fadeInTime > 0.0 ? 1f / fadeInTime : 1000f;
      length = interactionObject.length;
      isPaused = false;
      pickedUp = false;
      pickUpPosition = Vector3.zero;
      pickUpRotation = Quaternion.identity;
      if (interactionTarget != null)
        interactionTarget.RotateTo(effector.bone.position);
      started = true;
      return true;
    }

    public void Update(Transform root, float speed)
    {
      if (!inInteraction)
      {
        if (!started)
          return;
        isPaused = false;
        pickedUp = false;
        defaults = false;
        resetTimer = 1f;
        started = false;
      }
      else
      {
        if (interactionTarget != null && !interactionTarget.rotateOnce)
          interactionTarget.RotateTo(effector.bone.position);
        if (isPaused)
        {
          effector.position = target.TransformPoint(pausePositionRelative);
          effector.rotation = target.rotation * pauseRotationRelative;
          interactionObject.Apply(interactionSystem.ik.solver, effectorType, interactionTarget, timer, weight);
        }
        else
        {
          timer += (float) (Time.deltaTime * (double) speed * (interactionTarget != null ? interactionTarget.interactionSpeedMlp : 1.0));
          weight = Mathf.Clamp(weight + Time.deltaTime * fadeInSpeed * speed, 0.0f, 1f);
          bool pickUp = false;
          bool pause = false;
          TriggerUntriggeredEvents(true, out pickUp, out pause);
          Vector3 b1 = pickedUp ? pickUpPosition : target.position;
          Quaternion b2 = pickedUp ? pickUpRotation : target.rotation;
          effector.position = Vector3.Lerp(effector.bone.position, b1, weight);
          effector.rotation = Quaternion.Lerp(effector.bone.rotation, b2, weight);
          interactionObject.Apply(interactionSystem.ik.solver, effectorType, interactionTarget, timer, weight);
          if (pickUp)
            PickUp(root);
          if (pause)
            Pause();
          float b3 = interactionObject.GetValue(InteractionObject.WeightCurve.Type.PoserWeight, interactionTarget, timer);
          if (poser != null)
            poser.weight = Mathf.Lerp(poser.weight, b3, weight);
          else if (b3 > 0.0)
            Warning.Log("InteractionObject " + interactionObject.name + " has a curve/multipler for Poser Weight, but the bone of effector " + effectorType + " has no HandPoser/GenericPoser attached.", effector.bone);
          if (timer < (double) length)
            return;
          Stop();
        }
      }
    }

    public float progress
    {
      get => !inInteraction || length == 0.0 ? 0.0f : timer / length;
    }

    private void TriggerUntriggeredEvents(bool checkTime, out bool pickUp, out bool pause)
    {
      pickUp = false;
      pause = false;
      for (int index = 0; index < triggered.Count; ++index)
      {
        if (!triggered[index] && (!checkTime || interactionObject.events[index].time < (double) timer))
        {
          interactionObject.events[index].Activate(effector.bone);
          if (interactionObject.events[index].pickUp)
          {
            if (timer >= (double) interactionObject.events[index].time)
              timer = interactionObject.events[index].time;
            pickUp = true;
          }
          if (interactionObject.events[index].pause)
          {
            if (timer >= (double) interactionObject.events[index].time)
              timer = interactionObject.events[index].time;
            pause = true;
          }
          if (interactionSystem.OnInteractionEvent != null)
            interactionSystem.OnInteractionEvent(effectorType, interactionObject, interactionObject.events[index]);
          triggered[index] = true;
        }
      }
    }

    private void PickUp(Transform root)
    {
      pickUpPosition = effector.position;
      pickUpRotation = effector.rotation;
      pickUpOnPostFBBIK = true;
      pickedUp = true;
      Rigidbody component = interactionObject.targetsRoot.GetComponent<Rigidbody>();
      if (component != null)
      {
        if (!component.isKinematic)
          component.isKinematic = true;
        if (root.GetComponent<Collider>() != null)
        {
          foreach (Collider componentsInChild in interactionObject.targetsRoot.GetComponentsInChildren<Collider>())
          {
            if (!componentsInChild.isTrigger)
              Physics.IgnoreCollision(root.GetComponent<Collider>(), componentsInChild);
          }
        }
      }
      if (interactionSystem.OnInteractionPickUp == null)
        return;
      interactionSystem.OnInteractionPickUp(effectorType, interactionObject);
    }

    public bool Stop()
    {
      if (!inInteraction)
        return false;
      bool pickUp = false;
      bool pause = false;
      TriggerUntriggeredEvents(false, out pickUp, out pause);
      if (interactionSystem.OnInteractionStop != null)
        interactionSystem.OnInteractionStop(effectorType, interactionObject);
      if (interactionTarget != null)
        interactionTarget.ResetRotation();
      interactionObject = null;
      weight = 0.0f;
      timer = 0.0f;
      isPaused = false;
      target = null;
      defaults = false;
      resetTimer = 1f;
      if (poser != null && !pickedUp)
        poser.weight = 0.0f;
      pickedUp = false;
      started = false;
      return true;
    }

    public void OnPostFBBIK()
    {
      if (!inInteraction)
        return;
      float num = interactionObject.GetValue(InteractionObject.WeightCurve.Type.RotateBoneWeight, interactionTarget, timer) * weight;
      if (num > 0.0)
      {
        Quaternion quaternion = Quaternion.Slerp(effector.bone.rotation, pickedUp ? pickUpRotation : effector.rotation, num * num);
        effector.bone.localRotation = Quaternion.Inverse(effector.bone.parent.rotation) * quaternion;
      }
      if (!pickUpOnPostFBBIK)
        return;
      Vector3 position = effector.bone.position;
      effector.bone.position = pickUpPosition;
      interactionObject.targetsRoot.parent = effector.bone;
      effector.bone.position = position;
      pickUpOnPostFBBIK = false;
    }
  }
}
