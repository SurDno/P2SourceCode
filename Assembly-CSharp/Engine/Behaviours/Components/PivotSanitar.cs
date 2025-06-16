using Engine.Behaviours.Engines.Controllers;
using Engine.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Behaviours.Components
{
  [RequireComponent(typeof (IKController))]
  [DisallowMultipleComponent]
  public class PivotSanitar : MonoBehaviour
  {
    [SerializeField]
    [FormerlySerializedAs("FlamethrowerPS")]
    private global::Flamethrower flamethrowerPS = (global::Flamethrower) null;
    private Transform flamethrowerPSParent;
    private Vector3 flamethrowerLocalPosition;
    private Quaternion flamethrowerLocalRotation;
    private Animator animator;
    private FightAnimatorBehavior.AnimatorState animatorState;
    private bool flamethrower;
    private float stanceOnPoseWeigth = 0.0f;
    private bool attackStance;
    private float aimingTime;
    private Transform targetObject;

    private FightAnimatorBehavior.AnimatorState AnimatorState
    {
      get
      {
        if (this.animatorState == null)
          this.animatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
        return this.animatorState;
      }
    }

    public event Action OnInvalidate;

    public bool AttackStance
    {
      get => this.attackStance;
      set
      {
        if (this.attackStance == value)
          return;
        this.attackStance = value;
        Action onInvalidate = this.OnInvalidate;
        if (onInvalidate == null)
          return;
        onInvalidate();
      }
    }

    public bool Flamethrower
    {
      get => this.flamethrower;
      set
      {
        if (this.flamethrower == value)
          return;
        this.flamethrower = value;
        Action onInvalidate = this.OnInvalidate;
        if (onInvalidate == null)
          return;
        onInvalidate();
      }
    }

    public float AimingTime => this.aimingTime;

    public Transform TargetObject
    {
      get => this.targetObject;
      set
      {
        if ((UnityEngine.Object) this.targetObject == (UnityEngine.Object) null)
          this.aimingTime = 0.0f;
        this.targetObject = value;
      }
    }

    public IEnumerable<IEntity> Targets
    {
      get
      {
        if ((UnityEngine.Object) this.flamethrowerPS != (UnityEngine.Object) null && this.Flamethrower)
        {
          foreach (IFlamable flamable in this.flamethrowerPS.MovablesHit)
          {
            IFlamable movable = flamable;
            if (movable != null && !((UnityEngine.Object) movable.gameObject == (UnityEngine.Object) null))
            {
              IEntity entityMovable = EntityUtility.GetEntity(movable.gameObject);
              if (entityMovable != null)
                yield return entityMovable;
              entityMovable = (IEntity) null;
              movable = (IFlamable) null;
            }
          }
        }
      }
    }

    public bool IsIndoor
    {
      set
      {
        if (!((UnityEngine.Object) this.flamethrowerPS != (UnityEngine.Object) null))
          return;
        this.flamethrowerPS.SetIndoor(value);
      }
    }

    private void Awake()
    {
      this.animator = this.gameObject.GetComponent<Pivot>().GetAnimator();
      if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} doesn't contain {1} unity component.", (object) this.gameObject.name, (object) typeof (Animator).Name);
      }
      else
      {
        this.animatorState = FightAnimatorBehavior.GetAnimatorState(this.animator);
        if (!((UnityEngine.Object) this.flamethrowerPS == (UnityEngine.Object) null))
          return;
        Debug.LogWarningFormat("{0}: doesn't contain FlamethrowerPS", (object) this.gameObject.name);
      }
    }

    private void Start()
    {
      if (!((UnityEngine.Object) this.flamethrowerPS != (UnityEngine.Object) null))
        return;
      this.flamethrowerLocalPosition = this.flamethrowerPS.transform.localPosition;
      this.flamethrowerLocalRotation = this.flamethrowerPS.transform.localRotation;
      this.flamethrowerPSParent = this.flamethrowerPS.transform.parent;
      this.flamethrowerPS.transform.parent = (Transform) null;
    }

    private void Update()
    {
      if ((UnityEngine.Object) this.flamethrowerPS != (UnityEngine.Object) null)
      {
        this.flamethrowerPS.Fire = this.flamethrower;
        this.flamethrowerPS.transform.position = this.flamethrowerPSParent.TransformPoint(this.flamethrowerLocalPosition);
        this.flamethrowerPS.transform.rotation = this.flamethrowerPSParent.rotation * this.flamethrowerLocalRotation;
      }
      this.stanceOnPoseWeigth = Mathf.Clamp01(this.stanceOnPoseWeigth + (this.AttackStance ? 1f : -1f) * Time.deltaTime);
      this.aimingTime += Time.deltaTime;
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) this.flamethrowerPS != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.flamethrowerPS.gameObject);
    }
  }
}
