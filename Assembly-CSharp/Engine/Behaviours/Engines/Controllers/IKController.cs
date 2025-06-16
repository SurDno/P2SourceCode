using System;
using System.Collections;
using Engine.Behaviours.Components;
using Engine.Common.Services;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.CameraServices;
using Inspectors;
using RootMotion.FinalIK;

namespace Engine.Behaviours.Engines.Controllers
{
  [DisallowMultipleComponent]
  public class IKController : MonoBehaviour
  {
    [Header("Weapon")]
    [Tooltip("Максимальное отклонение оружия по горизонтали в градусах (от направления вперед)")]
    public float MaxWeaponAimAngle = 45f;
    [Tooltip("Время наведения оружия.")]
    public float WeaponAimTime = 1f;
    [Header("Look At")]
    [Tooltip("Максимальное отклонение головы по вертикали в градусах (от направления вперед)")]
    public float MaxAngleVertical = 30f;
    [Tooltip("Максимальное отклонение головы по горизонтали в градусах (от направления вперед)")]
    public float MaxAngleHorizontal = 75f;
    [Tooltip("Время наведения взгляда.")]
    public float LookTime = 0.5f;
    private Transform weaponTarget;
    private Pivot.AimWeaponType weaponAimTo = Pivot.AimWeaponType.Unknown;
    [SerializeField]
    private Transform lookAtTarget;
    [Header("You can use ComputeNpc to fill this!")]
    [SerializeField]
    private AimIK aimIK;
    [SerializeField]
    private LookAtIK lookAtIK;
    [SerializeField]
    private Pivot pivot;
    [SerializeField]
    private Animator animator;
    private float initialLookAtBodyWeight;
    private float initialLookAtHeadWeight;
    private GameObject targetAimDummy;
    private GameObject lookAtTargetDummy;
    private float weightAim;
    private float weigthLookAt;
    private Coroutine coroutine;
    private bool lookEyeContactOnly;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    [Inspected(Mutable = true)]
    public bool TestMode { get; set; }

    [Inspected(Mutable = true)]
    public bool TestModeLookEyeContactOnly { get; set; }

    public Transform WeaponTarget
    {
      set => weaponTarget = value;
      get => weaponTarget;
    }

    public Pivot.AimWeaponType WeaponAimTo
    {
      set => weaponAimTo = value;
      get => weaponAimTo;
    }

    public Transform LookTarget
    {
      get => lookAtTarget;
      set
      {
        if ((UnityEngine.Object) value == (UnityEngine.Object) null)
        {
          lookAtTarget = (Transform) null;
        }
        else
        {
          PivotPlayer component1 = value.GetComponent<PivotPlayer>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          {
            lookAtTarget = component1.AnimatedCameraBone;
          }
          else
          {
            Pivot component2 = value.GetComponent<Pivot>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            {
              Transform aimTransform = component2.GetAimTransform(Pivot.AimWeaponType.Head);
              lookAtTarget = !((UnityEngine.Object) aimTransform != (UnityEngine.Object) null) ? value : aimTransform;
            }
            else
              lookAtTarget = value;
          }
        }
      }
    }

    public bool LookEyeContactOnly
    {
      get => TestMode ? TestModeLookEyeContactOnly : lookEyeContactOnly;
      set => lookEyeContactOnly = value;
    }

    public bool StopIfOutOfLimits { get; set; }

    private void Awake()
    {
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
        Debug.LogError((object) (this.gameObject.name + " doesn't contain " + typeof (Pivot).Name + " unity component."));
      else if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
      {
        Debug.LogError((object) (this.gameObject.name + " doesn't contain " + typeof (Animator).Name + " unity component."));
      }
      else
      {
        if ((UnityEngine.Object) aimIK != (UnityEngine.Object) null)
        {
          targetAimDummy = new GameObject("[AimWeapon] Target Dummy");
          targetAimDummy.transform.parent = this.transform;
          if ((UnityEngine.Object) pivot.AimTransform != (UnityEngine.Object) null)
            aimIK.solver.transform = pivot.AimTransform.transform;
          aimIK.solver.axis = pivot.AimAxis.normalized;
          aimIK.solver.poleAxis = pivot.PoleAxis.normalized;
          aimIK.solver.target = targetAimDummy.transform;
          IKSolverAim solver1 = aimIK.solver;
          solver1.OnPreUpdate = solver1.OnPreUpdate + PreAimUpdate;
          IKSolverAim solver2 = aimIK.solver;
          solver2.OnPostUpdate = solver2.OnPostUpdate + PostAimUpdate;
          aimIK.solver.IKPositionWeight = 0.0f;
        }
        if ((UnityEngine.Object) lookAtIK == (UnityEngine.Object) null)
        {
          Debug.LogWarningFormat("{0} doesn't contain {1} unity component. Aiming is impossible.", (object) this.gameObject.name, (object) typeof (LookAtIK).Name);
        }
        else
        {
          lookAtTargetDummy = new GameObject("[LookAtIK] Target Dummy");
          lookAtTargetDummy.transform.parent = this.transform;
          lookAtIK.solver.target = lookAtTargetDummy.transform;
          lookAtIK.solver.IKPositionWeight = 0.0f;
          IKSolverLookAt solver = lookAtIK.solver;
          solver.OnPreUpdate = solver.OnPreUpdate + OnPreLookAtIKUpdate;
          initialLookAtBodyWeight = lookAtIK.solver.bodyWeight;
          initialLookAtHeadWeight = lookAtIK.solver.headWeight;
        }
      }
    }

    private void OnEnable()
    {
      coroutine = this.StartCoroutine(UpdateIKRightAfterPhysicsUpdate());
    }

    private void OnDisable()
    {
      if (coroutine == null)
        return;
      this.StopCoroutine(coroutine);
      coroutine = (Coroutine) null;
    }

    private IEnumerator UpdateIKRightAfterPhysicsUpdate()
    {
      while (true)
      {
        if (animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
          UpdateIK();
        yield return (object) waitForFixedUpdate;
      }
    }

    private bool ClampTargetDirection()
    {
      Vector3 vector3_1 = targetAimDummy.transform.position - pivot.AimTransform.transform.position;
      Vector3 vector3_2 = pivot.AimTransform.transform.rotation * pivot.AimAxis.normalized;
      if ((double) Vector3.Angle(vector3_2, vector3_1) < MaxWeaponAimAngle)
        return false;
      float magnitude = vector3_1.magnitude;
      Quaternion to = Quaternion.LookRotation(vector3_1);
      targetAimDummy.transform.position = pivot.AimTransform.transform.position + Quaternion.RotateTowards(Quaternion.LookRotation(vector3_2), to, MaxWeaponAimAngle) * Vector3.forward * magnitude;
      return true;
    }

    private Vector3 ClampLookAtDirectionInForwardSpace(
      Vector3 targetDirection,
      float verticalAngleLimit,
      float horizontalAngleLimit,
      out bool clamped)
    {
      clamped = false;
      float f1 = Mathf.Atan2(targetDirection.y, Mathf.Sqrt((float) ((double) targetDirection.x * (double) targetDirection.x + (double) targetDirection.z * (double) targetDirection.z)));
      if (f1 > (double) verticalAngleLimit)
      {
        f1 = verticalAngleLimit;
        clamped = true;
      }
      else if (f1 < -(double) verticalAngleLimit)
      {
        f1 = -verticalAngleLimit;
        clamped = true;
      }
      float f2 = Mathf.Atan2(targetDirection.x, targetDirection.z);
      if (f2 > (double) horizontalAngleLimit)
      {
        f2 = horizontalAngleLimit;
        clamped = true;
      }
      else if (f2 < -(double) horizontalAngleLimit)
      {
        f2 = -horizontalAngleLimit;
        clamped = true;
      }
      return clamped ? new Vector3(Mathf.Cos(f1) * Mathf.Sin(f2), Mathf.Sin(f1), Mathf.Cos(f1) * Mathf.Cos(f2)) : targetDirection;
    }

    private void OnPreLookAtIKUpdate()
    {
      if ((UnityEngine.Object) pivot == (UnityEngine.Object) null)
        return;
      CameraService service = ServiceLocator.GetService<CameraService>();
      bool flag1 = service != null && service.Kind == CameraKindEnum.FirstPerson_Controlling;
      Transform aimTransform = pivot.GetAimTransform(Pivot.AimWeaponType.Head);
      if ((UnityEngine.Object) lookAtTarget == (UnityEngine.Object) null || lookAtIK.solver.eyes.Length == 0 || (UnityEngine.Object) aimTransform == (UnityEngine.Object) null)
      {
        weigthLookAt = Mathf.Clamp01(weigthLookAt - Time.deltaTime / LookTime);
        lookAtIK.solver.IKPositionWeight = SmoothUtility.Smooth22(weigthLookAt);
        if ((UnityEngine.Object) aimTransform == (UnityEngine.Object) null)
          return;
        if (lookAtIK.solver.IKPositionWeight < 0.0099999997764825821)
        {
          lookAtTargetDummy.transform.position = aimTransform.position + this.transform.forward * 1f;
        }
        else
        {
          Vector3 normalized = (lookAtTargetDummy.transform.position - aimTransform.position).normalized;
          Vector3 forward = this.transform.forward;
          float num = Mathf.Clamp01(Vector3.Angle(normalized, forward) / (MaxAngleHorizontal * 0.3f)) + 0.01f;
          Vector3 vector3 = Vector3.RotateTowards(normalized, forward, (float) (num * (double) MaxAngleHorizontal * (Math.PI / 180.0)) * Time.deltaTime / LookTime, 0.0f);
          lookAtTargetDummy.transform.position = aimTransform.position + vector3;
        }
      }
      else
      {
        bool clamped = false;
        Vector3 targetDirection = this.transform.InverseTransformDirection(lookAtTarget.position - aimTransform.position);
        Vector3 vector3_1 = this.transform.TransformDirection(!LookEyeContactOnly ? ClampLookAtDirectionInForwardSpace(targetDirection, MaxAngleVertical * ((float) Math.PI / 180f), MaxAngleHorizontal * ((float) Math.PI / 180f), out clamped) : ClampLookAtDirectionInForwardSpace(targetDirection, 0.157079637f, 0.34906584f, out clamped));
        bool flag2 = !StopIfOutOfLimits || !clamped;
        if (LookEyeContactOnly)
        {
          lookAtIK.solver.bodyWeight = Mathf.MoveTowards(lookAtIK.solver.bodyWeight, 0.0f, Time.deltaTime / LookTime);
          lookAtIK.solver.headWeight = Mathf.MoveTowards(lookAtIK.solver.headWeight, 0.0f, Time.deltaTime / LookTime);
          weigthLookAt = Mathf.Clamp01(weigthLookAt + (float) ((double) Time.deltaTime * (flag2 ? 1.0 : -1.0) / 0.10000000149011612));
        }
        else
        {
          lookAtIK.solver.bodyWeight = Mathf.MoveTowards(lookAtIK.solver.bodyWeight, initialLookAtBodyWeight, Time.deltaTime / LookTime);
          lookAtIK.solver.headWeight = Mathf.MoveTowards(lookAtIK.solver.headWeight, initialLookAtBodyWeight, Time.deltaTime / LookTime);
          weigthLookAt = Mathf.Clamp01(weigthLookAt + Time.deltaTime * (flag2 ? 1f : -1f) / LookTime);
        }
        Vector3 vector3_2 = lookAtTargetDummy.transform.position - aimTransform.position;
        float num = Mathf.Clamp01(Vector3.Angle(vector3_2, vector3_1) / (MaxAngleHorizontal * 0.3f)) + 0.01f;
        Vector3 vector3_3 = Vector3.RotateTowards(vector3_2, vector3_1, (float) (num * (double) MaxAngleHorizontal * (Math.PI / 180.0)) * Time.deltaTime / LookTime, 0.0f);
        lookAtTargetDummy.transform.position = aimTransform.position + vector3_3;
        if (TestMode)
          lookAtIK.solver.IKPositionWeight = SmoothUtility.Smooth22(weigthLookAt);
        else
          lookAtIK.solver.IKPositionWeight = flag1 ? SmoothUtility.Smooth22(weigthLookAt) : 0.0f;
      }
    }

    public void PreAimUpdate()
    {
      if ((UnityEngine.Object) weaponTarget != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) pivot.AimTransform != (UnityEngine.Object) null && (UnityEngine.Object) aimIK.solver.transform != (UnityEngine.Object) pivot.AimTransform.transform)
          aimIK.solver.transform = pivot.AimTransform.transform;
        if ((UnityEngine.Object) aimIK.solver.transform == (UnityEngine.Object) null)
          return;
        Vector3 position = weaponTarget.position;
        Pivot component = weaponTarget.GetComponent<Pivot>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          Transform aimTransform = component.GetAimTransform(weaponAimTo == Pivot.AimWeaponType.Unknown ? pivot.AimType : weaponAimTo);
          if ((UnityEngine.Object) aimTransform != (UnityEngine.Object) null)
            position = aimTransform.position;
        }
        targetAimDummy.transform.position = position;
        if (ClampTargetDirection())
        {
          if (weightAim <= 0.0)
            return;
          weightAim = Mathf.Clamp01(weightAim - Time.deltaTime / (3f * WeaponAimTime));
          aimIK.solver.IKPositionWeight = weightAim;
        }
        else
        {
          if (weightAim >= 1.0)
            return;
          weightAim = Mathf.Clamp01(weightAim + Time.deltaTime / WeaponAimTime);
          aimIK.solver.IKPositionWeight = weightAim;
        }
      }
      else if (weightAim > 0.0)
      {
        weightAim = Mathf.Clamp01(weightAim - Time.deltaTime / WeaponAimTime);
        aimIK.solver.IKPositionWeight = weightAim;
      }
    }

    public void PostAimUpdate()
    {
      if (!((UnityEngine.Object) pivot.WeaponDummyTransform != (UnityEngine.Object) null) || !((UnityEngine.Object) pivot.WeaponTransform != (UnityEngine.Object) null))
        return;
      pivot.WeaponTransform.transform.position = pivot.WeaponDummyTransform.transform.position;
      pivot.WeaponTransform.transform.rotation = pivot.WeaponDummyTransform.transform.rotation;
    }

    private void LateUpdate()
    {
      if (!((UnityEngine.Object) animator != (UnityEngine.Object) null) || animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
        return;
      UpdateIK();
    }

    private void UpdateIK()
    {
      if ((UnityEngine.Object) aimIK != (UnityEngine.Object) null)
        aimIK.solver.Update();
      if (!((UnityEngine.Object) lookAtIK != (UnityEngine.Object) null))
        return;
      lookAtIK.solver.Update();
    }
  }
}
