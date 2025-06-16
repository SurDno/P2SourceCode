// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.EmptyWeaponController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
    private float smoothedNormalizedSpeed = 0.0f;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    public bool GeometryVisible
    {
      set
      {
        this.geometryVisible = value;
        this.ApplyVisibility();
      }
      get => this.geometryVisible;
    }

    private void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.animatorState.ReactionLayerWeight = this.geometryVisible ? 1f : 0.0f;
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
      this.pivot = gameObject.GetComponent<PivotPlayer>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} has no {1} unity component", (object) gameObject.name, (object) typeof (PivotPlayer).Name);
      }
      else
      {
        this.gameObject = gameObject;
        this.animator = animator;
        this.animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        this.controllerComponent = entity.GetComponent<ControllerComponent>();
        this.detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
        if (this.detectable != null)
          return;
        Debug.LogWarningFormat("{0} doesn't have {1} engine component", (object) gameObject.name, (object) typeof (IDetectableComponent).Name);
      }
    }

    public IEntity GetItem() => this.item;

    public void SetItem(IEntity item) => this.item = item;

    public void Activate(bool geometryVisible)
    {
      this.geometryVisible = geometryVisible;
      Action unholsterEndEvent1 = this.WeaponUnholsterEndEvent;
      if (unholsterEndEvent1 != null)
        unholsterEndEvent1();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Fire, new GameActionHandle(this.FireListener));
      Action unholsterEndEvent2 = this.WeaponUnholsterEndEvent;
      if (unholsterEndEvent2 == null)
        return;
      unholsterEndEvent2();
    }

    public void Shutdown()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Fire, new GameActionHandle(this.FireListener));
      Action holsterStartEvent = this.WeaponHolsterStartEvent;
      if (holsterStartEvent == null)
        return;
      holsterStartEvent();
    }

    public void Reset() => this.animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void Update(IEntity target)
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      float target1 = 0.0f;
      bool flag = this.controllerComponent != null && this.controllerComponent.IsRun.Value;
      if (this.controllerComponent.IsWalk.Value)
        target1 = (flag ? 1f : 0.5f) * this.controllerComponent.WalkModifier.Value;
      this.smoothedNormalizedSpeed = Mathf.MoveTowards(this.smoothedNormalizedSpeed, target1, Time.deltaTime / 1f);
      this.animatorState.WalkSpeed = this.smoothedNormalizedSpeed;
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
      return down && this.entity == ServiceLocator.GetService<ISimulation>().Player && PlayerUtility.IsPlayerCanControlling;
    }

    public int Compare(Collider a, Collider b)
    {
      float num = Vector3.Dot(a.transform.forward, this.gameObject.transform.forward);
      return Vector3.Dot(b.transform.forward, this.gameObject.transform.forward).CompareTo(num);
    }

    public void Reaction()
    {
    }
  }
}
