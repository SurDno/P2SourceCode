using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using System;
using UnityEngine;

namespace Engine.Behaviours.Engines.Controllers
{
  public class PlayerVisirWeaponController : IWeaponController
  {
    private GameObject gameObject;
    private Animator animator;
    private PlayerAnimatorState animatorState;
    private PivotPlayer pivot;
    private IEntity entity;
    private DetectableComponent detectable;
    private bool geometryVisible;
    private bool weaponVisible;
    private IEntity item;
    private float layerWeight;

    public event Action WeaponUnholsterEndEvent;

    public event Action WeaponHolsterStartEvent;

    public event Action<IEntity, ShotType, ReactionType, ShotSubtypeEnum> WeaponShootEvent;

    bool IWeaponController.GeometryVisible
    {
      set
      {
        this.geometryVisible = value;
        this.ApplyVisibility();
      }
      get => this.geometryVisible;
    }

    private bool WeaponVisible
    {
      set
      {
        this.weaponVisible = value;
        this.ApplyVisibility();
      }
    }

    private void ApplyVisibility()
    {
      this.pivot.HandsGeometryVisible = this.geometryVisible;
      this.pivot.VisirGeometryVisible = this.geometryVisible && this.weaponVisible;
    }

    public void OnEnable() => this.animator.SetTrigger("Triggers/Restore");

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
      this.WeaponVisible = true;
      Action unholsterEndEvent = this.WeaponUnholsterEndEvent;
      if (unholsterEndEvent == null)
        return;
      unholsterEndEvent();
    }

    public void Shutdown()
    {
      Action holsterStartEvent = this.WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      this.WeaponVisible = false;
    }

    public void Reset() => this.animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void Update(IEntity target)
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      this.layerWeight = Mathf.MoveTowards(this.layerWeight, 1f, Time.deltaTime / 0.5f);
      this.animatorState.VisirLightLayerWeight = SmoothUtility.Smooth22(this.layerWeight);
    }

    public void UpdateSilent(IEntity target)
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      this.layerWeight = Mathf.MoveTowards(this.layerWeight, 0.0f, Time.deltaTime / 0.5f);
      this.animatorState.VisirLightLayerWeight = SmoothUtility.Smooth22(this.layerWeight);
    }

    public void LateUpdate(IEntity target) => this.pivot.ApplyWeaponTransform(WeaponKind.Visir);

    public void FixedUpdate(IEntity target)
    {
    }

    public void OnAnimatorEvent(string data)
    {
    }

    public void Reaction()
    {
    }
  }
}
