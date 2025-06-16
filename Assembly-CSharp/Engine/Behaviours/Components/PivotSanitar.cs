using System;
using System.Collections.Generic;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common;

namespace Engine.Behaviours.Components
{
  [RequireComponent(typeof (IKController))]
  [DisallowMultipleComponent]
  public class PivotSanitar : MonoBehaviour
  {
    [SerializeField]
    [FormerlySerializedAs("FlamethrowerPS")]
    private Flamethrower flamethrowerPS = null;
    private Transform flamethrowerPSParent;
    private Vector3 flamethrowerLocalPosition;
    private Quaternion flamethrowerLocalRotation;
    private Animator animator;
    private FightAnimatorBehavior.AnimatorState animatorState;
    private bool flamethrower;
    private float stanceOnPoseWeigth;
    private bool attackStance;
    private float aimingTime;
    private Transform targetObject;

    private FightAnimatorBehavior.AnimatorState AnimatorState
    {
      get
      {
        if (animatorState == null)
          animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
        return animatorState;
      }
    }

    public event Action OnInvalidate;

    public bool AttackStance
    {
      get => attackStance;
      set
      {
        if (attackStance == value)
          return;
        attackStance = value;
        Action onInvalidate = OnInvalidate;
        if (onInvalidate == null)
          return;
        onInvalidate();
      }
    }

    public bool Flamethrower
    {
      get => flamethrower;
      set
      {
        if (flamethrower == value)
          return;
        flamethrower = value;
        Action onInvalidate = OnInvalidate;
        if (onInvalidate == null)
          return;
        onInvalidate();
      }
    }

    public float AimingTime => aimingTime;

    public Transform TargetObject
    {
      get => targetObject;
      set
      {
        if ((UnityEngine.Object) targetObject == (UnityEngine.Object) null)
          aimingTime = 0.0f;
        targetObject = value;
      }
    }

    public IEnumerable<IEntity> Targets
    {
      get
      {
        if ((UnityEngine.Object) flamethrowerPS != (UnityEngine.Object) null && Flamethrower)
        {
          foreach (IFlamable flamable in flamethrowerPS.MovablesHit)
          {
            IFlamable movable = flamable;
            if (movable != null && !((UnityEngine.Object) movable.gameObject == (UnityEngine.Object) null))
            {
              IEntity entityMovable = EntityUtility.GetEntity(movable.gameObject);
              if (entityMovable != null)
                yield return entityMovable;
              entityMovable = null;
              movable = null;
            }
          }
        }
      }
    }

    public bool IsIndoor
    {
      set
      {
        if (!((UnityEngine.Object) flamethrowerPS != (UnityEngine.Object) null))
          return;
        flamethrowerPS.SetIndoor(value);
      }
    }

    private void Awake()
    {
      animator = this.gameObject.GetComponent<Pivot>().GetAnimator();
      if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat("{0} doesn't contain {1} unity component.", (object) this.gameObject.name, (object) typeof (Animator).Name);
      }
      else
      {
        animatorState = FightAnimatorBehavior.GetAnimatorState(animator);
        if (!((UnityEngine.Object) flamethrowerPS == (UnityEngine.Object) null))
          return;
        Debug.LogWarningFormat("{0}: doesn't contain FlamethrowerPS", (object) this.gameObject.name);
      }
    }

    private void Start()
    {
      if (!((UnityEngine.Object) flamethrowerPS != (UnityEngine.Object) null))
        return;
      flamethrowerLocalPosition = flamethrowerPS.transform.localPosition;
      flamethrowerLocalRotation = flamethrowerPS.transform.localRotation;
      flamethrowerPSParent = flamethrowerPS.transform.parent;
      flamethrowerPS.transform.parent = (Transform) null;
    }

    private void Update()
    {
      if ((UnityEngine.Object) flamethrowerPS != (UnityEngine.Object) null)
      {
        flamethrowerPS.Fire = flamethrower;
        flamethrowerPS.transform.position = flamethrowerPSParent.TransformPoint(flamethrowerLocalPosition);
        flamethrowerPS.transform.rotation = flamethrowerPSParent.rotation * flamethrowerLocalRotation;
      }
      stanceOnPoseWeigth = Mathf.Clamp01(stanceOnPoseWeigth + (AttackStance ? 1f : -1f) * Time.deltaTime);
      aimingTime += Time.deltaTime;
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) flamethrowerPS != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) flamethrowerPS.gameObject);
    }
  }
}
