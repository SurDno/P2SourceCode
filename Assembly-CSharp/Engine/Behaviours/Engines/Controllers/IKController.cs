// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.Controllers.IKController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Common.Services;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.CameraServices;
using Inspectors;
using RootMotion.FinalIK;
using System;
using System.Collections;
using UnityEngine;

#nullable disable
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
    private float weightAim = 0.0f;
    private float weigthLookAt = 0.0f;
    private Coroutine coroutine;
    private bool lookEyeContactOnly;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    [Inspected(Mutable = true)]
    public bool TestMode { get; set; }

    [Inspected(Mutable = true)]
    public bool TestModeLookEyeContactOnly { get; set; }

    public Transform WeaponTarget
    {
      set => this.weaponTarget = value;
      get => this.weaponTarget;
    }

    public Pivot.AimWeaponType WeaponAimTo
    {
      set => this.weaponAimTo = value;
      get => this.weaponAimTo;
    }

    public Transform LookTarget
    {
      get => this.lookAtTarget;
      set
      {
        if ((UnityEngine.Object) value == (UnityEngine.Object) null)
        {
          this.lookAtTarget = (Transform) null;
        }
        else
        {
          PivotPlayer component1 = value.GetComponent<PivotPlayer>();
          if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          {
            this.lookAtTarget = component1.AnimatedCameraBone;
          }
          else
          {
            Pivot component2 = value.GetComponent<Pivot>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            {
              Transform aimTransform = component2.GetAimTransform(Pivot.AimWeaponType.Head);
              this.lookAtTarget = !((UnityEngine.Object) aimTransform != (UnityEngine.Object) null) ? value : aimTransform;
            }
            else
              this.lookAtTarget = value;
          }
        }
      }
    }

    public bool LookEyeContactOnly
    {
      get => this.TestMode ? this.TestModeLookEyeContactOnly : this.lookEyeContactOnly;
      set => this.lookEyeContactOnly = value;
    }

    public bool StopIfOutOfLimits { get; set; }

    private void Awake()
    {
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
        Debug.LogError((object) (this.gameObject.name + " doesn't contain " + typeof (Pivot).Name + " unity component."));
      else if ((UnityEngine.Object) this.animator == (UnityEngine.Object) null)
      {
        Debug.LogError((object) (this.gameObject.name + " doesn't contain " + typeof (Animator).Name + " unity component."));
      }
      else
      {
        if ((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null)
        {
          this.targetAimDummy = new GameObject("[AimWeapon] Target Dummy");
          this.targetAimDummy.transform.parent = this.transform;
          if ((UnityEngine.Object) this.pivot.AimTransform != (UnityEngine.Object) null)
            this.aimIK.solver.transform = this.pivot.AimTransform.transform;
          this.aimIK.solver.axis = this.pivot.AimAxis.normalized;
          this.aimIK.solver.poleAxis = this.pivot.PoleAxis.normalized;
          this.aimIK.solver.target = this.targetAimDummy.transform;
          IKSolverAim solver1 = this.aimIK.solver;
          solver1.OnPreUpdate = solver1.OnPreUpdate + new IKSolver.UpdateDelegate(this.PreAimUpdate);
          IKSolverAim solver2 = this.aimIK.solver;
          solver2.OnPostUpdate = solver2.OnPostUpdate + new IKSolver.UpdateDelegate(this.PostAimUpdate);
          this.aimIK.solver.IKPositionWeight = 0.0f;
        }
        if ((UnityEngine.Object) this.lookAtIK == (UnityEngine.Object) null)
        {
          Debug.LogWarningFormat("{0} doesn't contain {1} unity component. Aiming is impossible.", (object) this.gameObject.name, (object) typeof (LookAtIK).Name);
        }
        else
        {
          this.lookAtTargetDummy = new GameObject("[LookAtIK] Target Dummy");
          this.lookAtTargetDummy.transform.parent = this.transform;
          this.lookAtIK.solver.target = this.lookAtTargetDummy.transform;
          this.lookAtIK.solver.IKPositionWeight = 0.0f;
          IKSolverLookAt solver = this.lookAtIK.solver;
          solver.OnPreUpdate = solver.OnPreUpdate + new IKSolver.UpdateDelegate(this.OnPreLookAtIKUpdate);
          this.initialLookAtBodyWeight = this.lookAtIK.solver.bodyWeight;
          this.initialLookAtHeadWeight = this.lookAtIK.solver.headWeight;
        }
      }
    }

    private void OnEnable()
    {
      this.coroutine = this.StartCoroutine(this.UpdateIKRightAfterPhysicsUpdate());
    }

    private void OnDisable()
    {
      if (this.coroutine == null)
        return;
      this.StopCoroutine(this.coroutine);
      this.coroutine = (Coroutine) null;
    }

    private IEnumerator UpdateIKRightAfterPhysicsUpdate()
    {
      while (true)
      {
        if (this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
          this.UpdateIK();
        yield return (object) this.waitForFixedUpdate;
      }
    }

    private bool ClampTargetDirection()
    {
      Vector3 vector3_1 = this.targetAimDummy.transform.position - this.pivot.AimTransform.transform.position;
      Vector3 vector3_2 = this.pivot.AimTransform.transform.rotation * this.pivot.AimAxis.normalized;
      if ((double) Vector3.Angle(vector3_2, vector3_1) < (double) this.MaxWeaponAimAngle)
        return false;
      float magnitude = vector3_1.magnitude;
      Quaternion to = Quaternion.LookRotation(vector3_1);
      this.targetAimDummy.transform.position = this.pivot.AimTransform.transform.position + Quaternion.RotateTowards(Quaternion.LookRotation(vector3_2), to, this.MaxWeaponAimAngle) * Vector3.forward * magnitude;
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
      if ((double) f1 > (double) verticalAngleLimit)
      {
        f1 = verticalAngleLimit;
        clamped = true;
      }
      else if ((double) f1 < -(double) verticalAngleLimit)
      {
        f1 = -verticalAngleLimit;
        clamped = true;
      }
      float f2 = Mathf.Atan2(targetDirection.x, targetDirection.z);
      if ((double) f2 > (double) horizontalAngleLimit)
      {
        f2 = horizontalAngleLimit;
        clamped = true;
      }
      else if ((double) f2 < -(double) horizontalAngleLimit)
      {
        f2 = -horizontalAngleLimit;
        clamped = true;
      }
      return clamped ? new Vector3(Mathf.Cos(f1) * Mathf.Sin(f2), Mathf.Sin(f1), Mathf.Cos(f1) * Mathf.Cos(f2)) : targetDirection;
    }

    private void OnPreLookAtIKUpdate()
    {
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
        return;
      CameraService service = ServiceLocator.GetService<CameraService>();
      bool flag1 = service != null && service.Kind == CameraKindEnum.FirstPerson_Controlling;
      Transform aimTransform = this.pivot.GetAimTransform(Pivot.AimWeaponType.Head);
      if ((UnityEngine.Object) this.lookAtTarget == (UnityEngine.Object) null || this.lookAtIK.solver.eyes.Length == 0 || (UnityEngine.Object) aimTransform == (UnityEngine.Object) null)
      {
        this.weigthLookAt = Mathf.Clamp01(this.weigthLookAt - Time.deltaTime / this.LookTime);
        this.lookAtIK.solver.IKPositionWeight = SmoothUtility.Smooth22(this.weigthLookAt);
        if ((UnityEngine.Object) aimTransform == (UnityEngine.Object) null)
          return;
        if ((double) this.lookAtIK.solver.IKPositionWeight < 0.0099999997764825821)
        {
          this.lookAtTargetDummy.transform.position = aimTransform.position + this.transform.forward * 1f;
        }
        else
        {
          Vector3 normalized = (this.lookAtTargetDummy.transform.position - aimTransform.position).normalized;
          Vector3 forward = this.transform.forward;
          float num = Mathf.Clamp01(Vector3.Angle(normalized, forward) / (this.MaxAngleHorizontal * 0.3f)) + 0.01f;
          Vector3 vector3 = Vector3.RotateTowards(normalized, forward, (float) ((double) num * (double) this.MaxAngleHorizontal * (Math.PI / 180.0)) * Time.deltaTime / this.LookTime, 0.0f);
          this.lookAtTargetDummy.transform.position = aimTransform.position + vector3;
        }
      }
      else
      {
        bool clamped = false;
        Vector3 targetDirection = this.transform.InverseTransformDirection(this.lookAtTarget.position - aimTransform.position);
        Vector3 vector3_1 = this.transform.TransformDirection(!this.LookEyeContactOnly ? this.ClampLookAtDirectionInForwardSpace(targetDirection, this.MaxAngleVertical * ((float) Math.PI / 180f), this.MaxAngleHorizontal * ((float) Math.PI / 180f), out clamped) : this.ClampLookAtDirectionInForwardSpace(targetDirection, 0.157079637f, 0.34906584f, out clamped));
        bool flag2 = !this.StopIfOutOfLimits || !clamped;
        if (this.LookEyeContactOnly)
        {
          this.lookAtIK.solver.bodyWeight = Mathf.MoveTowards(this.lookAtIK.solver.bodyWeight, 0.0f, Time.deltaTime / this.LookTime);
          this.lookAtIK.solver.headWeight = Mathf.MoveTowards(this.lookAtIK.solver.headWeight, 0.0f, Time.deltaTime / this.LookTime);
          this.weigthLookAt = Mathf.Clamp01(this.weigthLookAt + (float) ((double) Time.deltaTime * (flag2 ? 1.0 : -1.0) / 0.10000000149011612));
        }
        else
        {
          this.lookAtIK.solver.bodyWeight = Mathf.MoveTowards(this.lookAtIK.solver.bodyWeight, this.initialLookAtBodyWeight, Time.deltaTime / this.LookTime);
          this.lookAtIK.solver.headWeight = Mathf.MoveTowards(this.lookAtIK.solver.headWeight, this.initialLookAtBodyWeight, Time.deltaTime / this.LookTime);
          this.weigthLookAt = Mathf.Clamp01(this.weigthLookAt + Time.deltaTime * (flag2 ? 1f : -1f) / this.LookTime);
        }
        Vector3 vector3_2 = this.lookAtTargetDummy.transform.position - aimTransform.position;
        float num = Mathf.Clamp01(Vector3.Angle(vector3_2, vector3_1) / (this.MaxAngleHorizontal * 0.3f)) + 0.01f;
        Vector3 vector3_3 = Vector3.RotateTowards(vector3_2, vector3_1, (float) ((double) num * (double) this.MaxAngleHorizontal * (Math.PI / 180.0)) * Time.deltaTime / this.LookTime, 0.0f);
        this.lookAtTargetDummy.transform.position = aimTransform.position + vector3_3;
        if (this.TestMode)
          this.lookAtIK.solver.IKPositionWeight = SmoothUtility.Smooth22(this.weigthLookAt);
        else
          this.lookAtIK.solver.IKPositionWeight = flag1 ? SmoothUtility.Smooth22(this.weigthLookAt) : 0.0f;
      }
    }

    public void PreAimUpdate()
    {
      if ((UnityEngine.Object) this.weaponTarget != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.pivot.AimTransform != (UnityEngine.Object) null && (UnityEngine.Object) this.aimIK.solver.transform != (UnityEngine.Object) this.pivot.AimTransform.transform)
          this.aimIK.solver.transform = this.pivot.AimTransform.transform;
        if ((UnityEngine.Object) this.aimIK.solver.transform == (UnityEngine.Object) null)
          return;
        Vector3 position = this.weaponTarget.position;
        Pivot component = this.weaponTarget.GetComponent<Pivot>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          Transform aimTransform = component.GetAimTransform(this.weaponAimTo == Pivot.AimWeaponType.Unknown ? this.pivot.AimType : this.weaponAimTo);
          if ((UnityEngine.Object) aimTransform != (UnityEngine.Object) null)
            position = aimTransform.position;
        }
        this.targetAimDummy.transform.position = position;
        if (this.ClampTargetDirection())
        {
          if ((double) this.weightAim <= 0.0)
            return;
          this.weightAim = Mathf.Clamp01(this.weightAim - Time.deltaTime / (3f * this.WeaponAimTime));
          this.aimIK.solver.IKPositionWeight = this.weightAim;
        }
        else
        {
          if ((double) this.weightAim >= 1.0)
            return;
          this.weightAim = Mathf.Clamp01(this.weightAim + Time.deltaTime / this.WeaponAimTime);
          this.aimIK.solver.IKPositionWeight = this.weightAim;
        }
      }
      else if ((double) this.weightAim > 0.0)
      {
        this.weightAim = Mathf.Clamp01(this.weightAim - Time.deltaTime / this.WeaponAimTime);
        this.aimIK.solver.IKPositionWeight = this.weightAim;
      }
    }

    public void PostAimUpdate()
    {
      if (!((UnityEngine.Object) this.pivot.WeaponDummyTransform != (UnityEngine.Object) null) || !((UnityEngine.Object) this.pivot.WeaponTransform != (UnityEngine.Object) null))
        return;
      this.pivot.WeaponTransform.transform.position = this.pivot.WeaponDummyTransform.transform.position;
      this.pivot.WeaponTransform.transform.rotation = this.pivot.WeaponDummyTransform.transform.rotation;
    }

    private void LateUpdate()
    {
      if (!((UnityEngine.Object) this.animator != (UnityEngine.Object) null) || this.animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
        return;
      this.UpdateIK();
    }

    private void UpdateIK()
    {
      if ((UnityEngine.Object) this.aimIK != (UnityEngine.Object) null)
        this.aimIK.solver.Update();
      if (!((UnityEngine.Object) this.lookAtIK != (UnityEngine.Object) null))
        return;
      this.lookAtIK.solver.Update();
    }
  }
}
