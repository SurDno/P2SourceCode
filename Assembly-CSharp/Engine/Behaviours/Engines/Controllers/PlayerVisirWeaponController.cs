using System;
using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;

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
        geometryVisible = value;
        ApplyVisibility();
      }
      get => geometryVisible;
    }

    private bool WeaponVisible
    {
      set
      {
        weaponVisible = value;
        ApplyVisibility();
      }
    }

    private void ApplyVisibility()
    {
      pivot.HandsGeometryVisible = geometryVisible;
      pivot.VisirGeometryVisible = geometryVisible && weaponVisible;
    }

    public void OnEnable() => animator.SetTrigger("Triggers/Restore");

    public void OnDisable()
    {
    }

    public void Initialise(IEntity entity, GameObject gameObject, Animator animator)
    {
      this.entity = entity;
      pivot = gameObject.GetComponent<PivotPlayer>();
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} has no {1} unity component", (object) gameObject.name, (object) typeof (PivotPlayer).Name);
      }
      else
      {
        this.gameObject = gameObject;
        this.animator = animator;
        animatorState = PlayerAnimatorState.GetAnimatorState(animator);
        detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
        if (detectable != null)
          return;
        Debug.LogWarningFormat("{0} doesn't have {1} engine component", (object) gameObject.name, (object) typeof (IDetectableComponent).Name);
      }
    }

    public IEntity GetItem() => item;

    public void SetItem(IEntity item) => this.item = item;

    public void Activate(bool geometryVisible)
    {
      WeaponVisible = true;
      Action unholsterEndEvent = WeaponUnholsterEndEvent;
      if (unholsterEndEvent == null)
        return;
      unholsterEndEvent();
    }

    public void Shutdown()
    {
      Action holsterStartEvent = WeaponHolsterStartEvent;
      if (holsterStartEvent != null)
        holsterStartEvent();
      WeaponVisible = false;
    }

    public void Reset() => animatorState.ResetAnimator();

    public bool Validate(GameObject gameObject, IEntity item) => true;

    public void Update(IEntity target)
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      layerWeight = Mathf.MoveTowards(layerWeight, 1f, Time.deltaTime / 0.5f);
      animatorState.VisirLightLayerWeight = SmoothUtility.Smooth22(layerWeight);
    }

    public void UpdateSilent(IEntity target)
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
        return;
      layerWeight = Mathf.MoveTowards(layerWeight, 0.0f, Time.deltaTime / 0.5f);
      animatorState.VisirLightLayerWeight = SmoothUtility.Smooth22(layerWeight);
    }

    public void LateUpdate(IEntity target) => pivot.ApplyWeaponTransform(WeaponKind.Visir);

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
