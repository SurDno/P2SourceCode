using System;
using System.Collections.Generic;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public class EmptyWeaponController : IWeaponController, IComparer<Collider>
  {
    private const float diseasedPushCooldownTime = 1f;
    private const float diseasedPushDistanceStart = 1f;
    private const float diseasedPushDistanceEnd = 0.75f;
    private GameObject gameObject;
    private Animator animator;
    private PlayerAnimatorState animatorState;
    private PivotPlayer pivot;
    private ControllerComponent controllerComponent;
    private IEntity entity;
    private DetectableComponent detectable;
    private bool geometryVisible;
    private bool weaponVisible;
    private static Collider[] tmp = new Collider[64];
    private IEntity item;
    private float smoothedNormalizedSpeed;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    public bool GeometryVisible
    {
      set
      {
        geometryVisible = value;
        ApplyVisibility();
      }
      get => geometryVisible;
    }

    private void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      animatorState.ReactionLayerWeight = geometryVisible ? 1f : 0.0f;
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
    }

    public void Initialise(IEntity entity, GameObject gameObject, Animator animator)
    {
      this.entity = entity;
      pivot = gameObject.GetComponent<PivotPlayer>();
      if (pivot == null)
      {
        Debug.LogErrorFormat("{0} has no {1} unity component", gameObject.name, typeof (PivotPlayer).Name);
      }
      else
      {
        this.gameObject = gameObject;
        this.animator = animator;
        animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        controllerComponent = entity.GetComponent<ControllerComponent>();
        detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
        if (detectable != null)
          return;
        Debug.LogWarningFormat("{0} doesn't have {1} engine component", gameObject.name, typeof (IDetectableComponent).Name);
      }
    }

    public IEntity GetItem() => item;

    public void SetItem(IEntity item) => this.item = item;

    public void Activate(bool geometryVisible)
    {
      this.geometryVisible = geometryVisible;
      Action unholsterEndEvent1 = WeaponUnholsterEndEvent;
      if (unholsterEndEvent1 != null)
        unholsterEndEvent1();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, FireListener);
      Action unholsterEndEvent2 = WeaponUnholsterEndEvent;
      if (unholsterEndEvent2 == null)
        return;
      unholsterEndEvent2();
    }

    public void Shutdown()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, FireListener);
      Action holsterStartEvent = WeaponHolsterStartEvent;
      if (holsterStartEvent == null)
        return;
      holsterStartEvent();
    }

    public void Reset() => animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void Update(IEntity target)
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      float target1 = 0.0f;
      bool flag = controllerComponent != null && controllerComponent.IsRun.Value;
      if (controllerComponent.IsWalk.Value)
        target1 = (flag ? 1f : 0.5f) * controllerComponent.WalkModifier.Value;
      smoothedNormalizedSpeed = Mathf.MoveTowards(smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
      animatorState.WalkSpeed = smoothedNormalizedSpeed;
    }

    public void UpdateSilent(IEntity target)
    {
    }

    public void LateUpdate(IEntity target)
    {
    }

    public void FixedUpdate(IEntity target)
    {
    }

    public void OnAnimatorEvent(string data)
    {
    }

    private bool FireListener(GameActionType type, bool down)
    {
      return down && entity == ServiceLocator.GetService<ISimulation>().Player && PlayerUtility.IsPlayerCanControlling;
    }

    public int Compare(Collider a, Collider b)
    {
      float num = Vector3.Dot(a.transform.forward, gameObject.transform.forward);
      return Vector3.Dot(b.transform.forward, gameObject.transform.forward).CompareTo(num);
    }

    public void Reaction()
    {
    }
  }
}
