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

    public bool inInteraction => (UnityEngine.Object) this.interactionObject != (UnityEngine.Object) null;

    public InteractionEffector(FullBodyBipedEffector effectorType)
    {
      this.effectorType = effectorType;
    }

    public void Initiate(InteractionSystem interactionSystem)
    {
      this.interactionSystem = interactionSystem;
      if (this.effector == null)
      {
        this.effector = interactionSystem.ik.solver.GetEffector(this.effectorType);
        this.poser = this.effector.bone.GetComponent<Poser>();
      }
      this.StoreDefaults();
    }

    private void StoreDefaults()
    {
      this.defaultPositionWeight = this.interactionSystem.ik.solver.GetEffector(this.effectorType).positionWeight;
      this.defaultRotationWeight = this.interactionSystem.ik.solver.GetEffector(this.effectorType).rotationWeight;
      this.defaultPull = this.interactionSystem.ik.solver.GetChain(this.effectorType).pull;
      this.defaultReach = this.interactionSystem.ik.solver.GetChain(this.effectorType).reach;
      this.defaultPush = this.interactionSystem.ik.solver.GetChain(this.effectorType).push;
      this.defaultPushParent = this.interactionSystem.ik.solver.GetChain(this.effectorType).pushParent;
    }

    public bool ResetToDefaults(float speed)
    {
      if (this.inInteraction || this.isPaused || this.defaults)
        return false;
      this.resetTimer = Mathf.MoveTowards(this.resetTimer, 0.0f, Time.deltaTime * speed);
      if (this.effector.isEndEffector)
      {
        if (this.pullUsed)
          this.interactionSystem.ik.solver.GetChain(this.effectorType).pull = Mathf.Lerp(this.defaultPull, this.interactionSystem.ik.solver.GetChain(this.effectorType).pull, this.resetTimer);
        if (this.reachUsed)
          this.interactionSystem.ik.solver.GetChain(this.effectorType).reach = Mathf.Lerp(this.defaultReach, this.interactionSystem.ik.solver.GetChain(this.effectorType).reach, this.resetTimer);
        if (this.pushUsed)
          this.interactionSystem.ik.solver.GetChain(this.effectorType).push = Mathf.Lerp(this.defaultPush, this.interactionSystem.ik.solver.GetChain(this.effectorType).push, this.resetTimer);
        if (this.pushParentUsed)
          this.interactionSystem.ik.solver.GetChain(this.effectorType).pushParent = Mathf.Lerp(this.defaultPushParent, this.interactionSystem.ik.solver.GetChain(this.effectorType).pushParent, this.resetTimer);
      }
      if (this.positionWeightUsed)
        this.effector.positionWeight = Mathf.Lerp(this.defaultPositionWeight, this.effector.positionWeight, this.resetTimer);
      if (this.rotationWeightUsed)
        this.effector.rotationWeight = Mathf.Lerp(this.defaultRotationWeight, this.effector.rotationWeight, this.resetTimer);
      if ((double) this.resetTimer <= 0.0)
      {
        this.pullUsed = false;
        this.reachUsed = false;
        this.pushUsed = false;
        this.pushParentUsed = false;
        this.positionWeightUsed = false;
        this.rotationWeightUsed = false;
        this.defaults = true;
      }
      return true;
    }

    public bool Pause()
    {
      if (!this.inInteraction)
        return false;
      this.isPaused = true;
      this.pausePositionRelative = this.target.InverseTransformPoint(this.effector.position);
      this.pauseRotationRelative = Quaternion.Inverse(this.target.rotation) * this.effector.rotation;
      if (this.interactionSystem.OnInteractionPause != null)
        this.interactionSystem.OnInteractionPause(this.effectorType, this.interactionObject);
      return true;
    }

    public bool Resume()
    {
      if (!this.inInteraction)
        return false;
      this.isPaused = false;
      if (this.interactionSystem.OnInteractionResume != null)
        this.interactionSystem.OnInteractionResume(this.effectorType, this.interactionObject);
      return true;
    }

    public bool Start(
      InteractionObject interactionObject,
      string tag,
      float fadeInTime,
      bool interrupt)
    {
      if (!this.inInteraction)
      {
        this.effector.position = this.effector.bone.position;
        this.effector.rotation = this.effector.bone.rotation;
      }
      else
      {
        if (!interrupt)
          return false;
        this.defaults = false;
      }
      this.target = interactionObject.GetTarget(this.effectorType, tag);
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
        return false;
      this.interactionTarget = this.target.GetComponent<InteractionTarget>();
      this.interactionObject = interactionObject;
      if (this.interactionSystem.OnInteractionStart != null)
        this.interactionSystem.OnInteractionStart(this.effectorType, interactionObject);
      interactionObject.OnStartInteraction(this.interactionSystem);
      this.triggered.Clear();
      for (int index = 0; index < interactionObject.events.Length; ++index)
        this.triggered.Add(false);
      if ((UnityEngine.Object) this.poser != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.poser.poseRoot == (UnityEngine.Object) null)
          this.poser.weight = 0.0f;
        this.poser.poseRoot = !((UnityEngine.Object) this.interactionTarget != (UnityEngine.Object) null) ? (Transform) null : this.target.transform;
        this.poser.AutoMapping();
      }
      this.positionWeightUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.PositionWeight);
      this.rotationWeightUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.RotationWeight);
      this.pullUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.Pull);
      this.reachUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.Reach);
      this.pushUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.Push);
      this.pushParentUsed = interactionObject.CurveUsed(InteractionObject.WeightCurve.Type.PushParent);
      if (this.defaults)
        this.StoreDefaults();
      this.timer = 0.0f;
      this.weight = 0.0f;
      this.fadeInSpeed = (double) fadeInTime > 0.0 ? 1f / fadeInTime : 1000f;
      this.length = interactionObject.length;
      this.isPaused = false;
      this.pickedUp = false;
      this.pickUpPosition = Vector3.zero;
      this.pickUpRotation = Quaternion.identity;
      if ((UnityEngine.Object) this.interactionTarget != (UnityEngine.Object) null)
        this.interactionTarget.RotateTo(this.effector.bone.position);
      this.started = true;
      return true;
    }

    public void Update(Transform root, float speed)
    {
      if (!this.inInteraction)
      {
        if (!this.started)
          return;
        this.isPaused = false;
        this.pickedUp = false;
        this.defaults = false;
        this.resetTimer = 1f;
        this.started = false;
      }
      else
      {
        if ((UnityEngine.Object) this.interactionTarget != (UnityEngine.Object) null && !this.interactionTarget.rotateOnce)
          this.interactionTarget.RotateTo(this.effector.bone.position);
        if (this.isPaused)
        {
          this.effector.position = this.target.TransformPoint(this.pausePositionRelative);
          this.effector.rotation = this.target.rotation * this.pauseRotationRelative;
          this.interactionObject.Apply(this.interactionSystem.ik.solver, this.effectorType, this.interactionTarget, this.timer, this.weight);
        }
        else
        {
          this.timer += (float) ((double) Time.deltaTime * (double) speed * ((UnityEngine.Object) this.interactionTarget != (UnityEngine.Object) null ? (double) this.interactionTarget.interactionSpeedMlp : 1.0));
          this.weight = Mathf.Clamp(this.weight + Time.deltaTime * this.fadeInSpeed * speed, 0.0f, 1f);
          bool pickUp = false;
          bool pause = false;
          this.TriggerUntriggeredEvents(true, out pickUp, out pause);
          Vector3 b1 = this.pickedUp ? this.pickUpPosition : this.target.position;
          Quaternion b2 = this.pickedUp ? this.pickUpRotation : this.target.rotation;
          this.effector.position = Vector3.Lerp(this.effector.bone.position, b1, this.weight);
          this.effector.rotation = Quaternion.Lerp(this.effector.bone.rotation, b2, this.weight);
          this.interactionObject.Apply(this.interactionSystem.ik.solver, this.effectorType, this.interactionTarget, this.timer, this.weight);
          if (pickUp)
            this.PickUp(root);
          if (pause)
            this.Pause();
          float b3 = this.interactionObject.GetValue(InteractionObject.WeightCurve.Type.PoserWeight, this.interactionTarget, this.timer);
          if ((UnityEngine.Object) this.poser != (UnityEngine.Object) null)
            this.poser.weight = Mathf.Lerp(this.poser.weight, b3, this.weight);
          else if ((double) b3 > 0.0)
            Warning.Log("InteractionObject " + this.interactionObject.name + " has a curve/multipler for Poser Weight, but the bone of effector " + this.effectorType.ToString() + " has no HandPoser/GenericPoser attached.", this.effector.bone);
          if ((double) this.timer < (double) this.length)
            return;
          this.Stop();
        }
      }
    }

    public float progress
    {
      get => !this.inInteraction || (double) this.length == 0.0 ? 0.0f : this.timer / this.length;
    }

    private void TriggerUntriggeredEvents(bool checkTime, out bool pickUp, out bool pause)
    {
      pickUp = false;
      pause = false;
      for (int index = 0; index < this.triggered.Count; ++index)
      {
        if (!this.triggered[index] && (!checkTime || (double) this.interactionObject.events[index].time < (double) this.timer))
        {
          this.interactionObject.events[index].Activate(this.effector.bone);
          if (this.interactionObject.events[index].pickUp)
          {
            if ((double) this.timer >= (double) this.interactionObject.events[index].time)
              this.timer = this.interactionObject.events[index].time;
            pickUp = true;
          }
          if (this.interactionObject.events[index].pause)
          {
            if ((double) this.timer >= (double) this.interactionObject.events[index].time)
              this.timer = this.interactionObject.events[index].time;
            pause = true;
          }
          if (this.interactionSystem.OnInteractionEvent != null)
            this.interactionSystem.OnInteractionEvent(this.effectorType, this.interactionObject, this.interactionObject.events[index]);
          this.triggered[index] = true;
        }
      }
    }

    private void PickUp(Transform root)
    {
      this.pickUpPosition = this.effector.position;
      this.pickUpRotation = this.effector.rotation;
      this.pickUpOnPostFBBIK = true;
      this.pickedUp = true;
      Rigidbody component = this.interactionObject.targetsRoot.GetComponent<Rigidbody>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        if (!component.isKinematic)
          component.isKinematic = true;
        if ((UnityEngine.Object) root.GetComponent<Collider>() != (UnityEngine.Object) null)
        {
          foreach (Collider componentsInChild in this.interactionObject.targetsRoot.GetComponentsInChildren<Collider>())
          {
            if (!componentsInChild.isTrigger)
              Physics.IgnoreCollision(root.GetComponent<Collider>(), componentsInChild);
          }
        }
      }
      if (this.interactionSystem.OnInteractionPickUp == null)
        return;
      this.interactionSystem.OnInteractionPickUp(this.effectorType, this.interactionObject);
    }

    public bool Stop()
    {
      if (!this.inInteraction)
        return false;
      bool pickUp = false;
      bool pause = false;
      this.TriggerUntriggeredEvents(false, out pickUp, out pause);
      if (this.interactionSystem.OnInteractionStop != null)
        this.interactionSystem.OnInteractionStop(this.effectorType, this.interactionObject);
      if ((UnityEngine.Object) this.interactionTarget != (UnityEngine.Object) null)
        this.interactionTarget.ResetRotation();
      this.interactionObject = (InteractionObject) null;
      this.weight = 0.0f;
      this.timer = 0.0f;
      this.isPaused = false;
      this.target = (Transform) null;
      this.defaults = false;
      this.resetTimer = 1f;
      if ((UnityEngine.Object) this.poser != (UnityEngine.Object) null && !this.pickedUp)
        this.poser.weight = 0.0f;
      this.pickedUp = false;
      this.started = false;
      return true;
    }

    public void OnPostFBBIK()
    {
      if (!this.inInteraction)
        return;
      float num = this.interactionObject.GetValue(InteractionObject.WeightCurve.Type.RotateBoneWeight, this.interactionTarget, this.timer) * this.weight;
      if ((double) num > 0.0)
      {
        Quaternion quaternion = Quaternion.Slerp(this.effector.bone.rotation, this.pickedUp ? this.pickUpRotation : this.effector.rotation, num * num);
        this.effector.bone.localRotation = Quaternion.Inverse(this.effector.bone.parent.rotation) * quaternion;
      }
      if (!this.pickUpOnPostFBBIK)
        return;
      Vector3 position = this.effector.bone.position;
      this.effector.bone.position = this.pickUpPosition;
      this.interactionObject.targetsRoot.parent = this.effector.bone;
      this.effector.bone.position = position;
      this.pickUpOnPostFBBIK = false;
    }
  }
}
