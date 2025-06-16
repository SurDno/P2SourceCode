// Decompiled with JetBrains decompiler
// Type: Cinemachine.CinemachineFreeLook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cinemachine.Utility;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(11f, DocumentationSortingAttribute.Level.UserRef)]
  [ExecuteInEditMode]
  [DisallowMultipleComponent]
  [AddComponentMenu("Cinemachine/CinemachineFreeLook")]
  public class CinemachineFreeLook : CinemachineVirtualCameraBase
  {
    [Tooltip("Object for the camera children to look at (the aim target).")]
    [NoSaveDuringPlay]
    public Transform m_LookAt = (Transform) null;
    [Tooltip("Object for the camera children wants to move with (the body target).")]
    [NoSaveDuringPlay]
    public Transform m_Follow = (Transform) null;
    [Tooltip("If enabled, this lens setting will apply to all three child rigs, otherwise the child rig lens settings will be used")]
    [FormerlySerializedAs("m_UseCommonLensSetting")]
    public bool m_CommonLens = true;
    [FormerlySerializedAs("m_LensAttributes")]
    [Tooltip("Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active")]
    [LensSettingsProperty]
    public LensSettings m_Lens = LensSettings.Default;
    [Header("Axis Control")]
    [Tooltip("The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs")]
    public AxisState m_YAxis = new AxisState(2f, 0.2f, 0.1f, 0.5f, "Mouse Y", false);
    [Tooltip("The Horizontal axis.  Value is 0..359.  This is passed on to the rigs' OrbitalTransposer component")]
    public AxisState m_XAxis = new AxisState(300f, 0.1f, 0.1f, 0.0f, "Mouse X", true);
    [Tooltip("The definition of Forward.  Camera will follow behind.")]
    public CinemachineOrbitalTransposer.Heading m_Heading = new CinemachineOrbitalTransposer.Heading(CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0.0f);
    [Tooltip("Controls how automatic recentering of the X axis is accomplished")]
    public CinemachineOrbitalTransposer.Recentering m_RecenterToTargetHeading = new CinemachineOrbitalTransposer.Recentering(false, 1f, 2f);
    [Header("Orbits")]
    [Tooltip("The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
    public CinemachineTransposer.BindingMode m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
    [Tooltip("Controls how taut is the line that connects the rigs' orbits, which determines final placement on the Y axis")]
    [Range(0.0f, 1f)]
    [FormerlySerializedAs("m_SplineTension")]
    public float m_SplineCurvature = 0.2f;
    [Tooltip("The radius and height of the three orbiting rigs.")]
    public CinemachineFreeLook.Orbit[] m_Orbits = new CinemachineFreeLook.Orbit[3]
    {
      new CinemachineFreeLook.Orbit(4.5f, 1.75f),
      new CinemachineFreeLook.Orbit(2.5f, 3f),
      new CinemachineFreeLook.Orbit(0.4f, 1.3f)
    };
    [SerializeField]
    [HideInInspector]
    [FormerlySerializedAs("m_HeadingBias")]
    private float m_LegacyHeadingBias = float.MaxValue;
    private bool mUseLegacyRigDefinitions = false;
    private bool mIsDestroyed = false;
    private CameraState m_State = CameraState.Default;
    [SerializeField]
    [HideInInspector]
    [NoSaveDuringPlay]
    private CinemachineVirtualCamera[] m_Rigs = new CinemachineVirtualCamera[3];
    private CinemachineOrbitalTransposer[] mOrbitals = (CinemachineOrbitalTransposer[]) null;
    private CinemachineBlend mBlendA;
    private CinemachineBlend mBlendB;
    public static CinemachineFreeLook.CreateRigDelegate CreateRigOverride;
    public static CinemachineFreeLook.DestroyRigDelegate DestroyRigOverride;
    private CinemachineFreeLook.Orbit[] m_CachedOrbits;
    private float m_CachedTension;
    private Vector4[] m_CachedKnots;
    private Vector4[] m_CachedCtrl1;
    private Vector4[] m_CachedCtrl2;

    protected override void OnValidate()
    {
      base.OnValidate();
      if ((double) this.m_LegacyHeadingBias != 3.4028234663852886E+38)
      {
        this.m_Heading.m_HeadingBias = this.m_LegacyHeadingBias;
        this.m_LegacyHeadingBias = float.MaxValue;
        this.m_RecenterToTargetHeading.LegacyUpgrade(ref this.m_Heading.m_HeadingDefinition, ref this.m_Heading.m_VelocityFilterStrength);
        this.mUseLegacyRigDefinitions = true;
      }
      this.m_YAxis.Validate();
      this.m_XAxis.Validate();
      this.m_RecenterToTargetHeading.Validate();
      this.m_Lens.Validate();
      this.InvalidateRigCache();
    }

    public CinemachineVirtualCamera GetRig(int i)
    {
      this.UpdateRigCache();
      return i < 0 || i > 2 ? (CinemachineVirtualCamera) null : this.m_Rigs[i];
    }

    public static string[] RigNames
    {
      get
      {
        return new string[3]
        {
          "TopRig",
          "MiddleRig",
          "BottomRig"
        };
      }
    }

    protected override void OnEnable()
    {
      this.mIsDestroyed = false;
      base.OnEnable();
      this.InvalidateRigCache();
    }

    protected override void OnDestroy()
    {
      if (this.m_Rigs != null)
      {
        foreach (CinemachineVirtualCamera rig in this.m_Rigs)
        {
          if ((UnityEngine.Object) rig != (UnityEngine.Object) null && (UnityEngine.Object) rig.gameObject != (UnityEngine.Object) null)
            rig.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
        }
      }
      this.mIsDestroyed = true;
      base.OnDestroy();
    }

    private void OnTransformChildrenChanged() => this.InvalidateRigCache();

    private void Reset() => this.DestroyRigs();

    public override CameraState State => this.m_State;

    public override Transform LookAt
    {
      get => this.ResolveLookAt(this.m_LookAt);
      set => this.m_LookAt = value;
    }

    public override Transform Follow
    {
      get => this.ResolveFollow(this.m_Follow);
      set => this.m_Follow = value;
    }

    public override ICinemachineCamera LiveChildOrSelf
    {
      get
      {
        if (this.m_Rigs == null || this.m_Rigs.Length != 3)
          return (ICinemachineCamera) this;
        if ((double) this.m_YAxis.Value < 0.33000001311302185)
          return (ICinemachineCamera) this.m_Rigs[2];
        return (double) this.m_YAxis.Value > 0.6600000262260437 ? (ICinemachineCamera) this.m_Rigs[0] : (ICinemachineCamera) this.m_Rigs[1];
      }
    }

    public override bool IsLiveChild(ICinemachineCamera vcam)
    {
      if (this.m_Rigs == null || this.m_Rigs.Length != 3)
        return false;
      if ((double) this.m_YAxis.Value < 0.33000001311302185)
        return vcam == this.m_Rigs[2];
      return (double) this.m_YAxis.Value > 0.6600000262260437 ? vcam == this.m_Rigs[0] : vcam == this.m_Rigs[1];
    }

    public override void RemovePostPipelineStageHook(
      CinemachineVirtualCameraBase.OnPostPipelineStageDelegate d)
    {
      base.RemovePostPipelineStageHook(d);
      this.UpdateRigCache();
      if (this.m_Rigs == null)
        return;
      foreach (CinemachineVirtualCamera rig in this.m_Rigs)
      {
        if ((UnityEngine.Object) rig != (UnityEngine.Object) null)
          rig.RemovePostPipelineStageHook(d);
      }
    }

    public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
    {
      if (!this.PreviousStateIsValid)
        deltaTime = -1f;
      this.UpdateRigCache();
      if ((double) deltaTime < 0.0)
        this.m_State = this.PullStateFromVirtualCamera(worldUp);
      this.m_State = this.CalculateNewState(worldUp, deltaTime);
      if ((UnityEngine.Object) this.Follow != (UnityEngine.Object) null)
      {
        Vector3 vector3 = this.State.RawPosition - this.transform.position;
        this.transform.position = this.State.RawPosition;
        this.m_Rigs[0].transform.position -= vector3;
        this.m_Rigs[1].transform.position -= vector3;
        this.m_Rigs[2].transform.position -= vector3;
      }
      this.PreviousStateIsValid = true;
      if ((double) deltaTime >= 0.0 || CinemachineCore.Instance.IsLive((ICinemachineCamera) this))
        this.m_YAxis.Update(deltaTime);
      this.PushSettingsToRigs();
    }

    public override void OnTransitionFromCamera(
      ICinemachineCamera fromCam,
      Vector3 worldUp,
      float deltaTime)
    {
      base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
      if (fromCam == null || !(fromCam is CinemachineFreeLook))
        return;
      CinemachineFreeLook cinemachineFreeLook = fromCam as CinemachineFreeLook;
      if ((UnityEngine.Object) cinemachineFreeLook.Follow == (UnityEngine.Object) this.Follow)
      {
        this.m_XAxis.Value = cinemachineFreeLook.m_XAxis.Value;
        this.m_YAxis.Value = cinemachineFreeLook.m_YAxis.Value;
        this.UpdateCameraState(worldUp, deltaTime);
      }
    }

    private void InvalidateRigCache() => this.mOrbitals = (CinemachineOrbitalTransposer[]) null;

    private void DestroyRigs()
    {
      CinemachineVirtualCamera[] cinemachineVirtualCameraArray = new CinemachineVirtualCamera[CinemachineFreeLook.RigNames.Length];
      for (int index = 0; index < CinemachineFreeLook.RigNames.Length; ++index)
      {
        foreach (Transform transform in this.transform)
        {
          if (transform.gameObject.name == CinemachineFreeLook.RigNames[index])
            cinemachineVirtualCameraArray[index] = transform.GetComponent<CinemachineVirtualCamera>();
        }
      }
      for (int index = 0; index < cinemachineVirtualCameraArray.Length; ++index)
      {
        if ((UnityEngine.Object) cinemachineVirtualCameraArray[index] != (UnityEngine.Object) null)
        {
          if (CinemachineFreeLook.DestroyRigOverride != null)
            CinemachineFreeLook.DestroyRigOverride(cinemachineVirtualCameraArray[index].gameObject);
          else
            UnityEngine.Object.Destroy((UnityEngine.Object) cinemachineVirtualCameraArray[index].gameObject);
        }
      }
      this.m_Rigs = (CinemachineVirtualCamera[]) null;
      this.mOrbitals = (CinemachineOrbitalTransposer[]) null;
    }

    private CinemachineVirtualCamera[] CreateRigs(CinemachineVirtualCamera[] copyFrom)
    {
      this.mOrbitals = (CinemachineOrbitalTransposer[]) null;
      float[] numArray = new float[3]{ 0.5f, 0.55f, 0.6f };
      CinemachineVirtualCamera[] rigs = new CinemachineVirtualCamera[3];
      for (int index = 0; index < CinemachineFreeLook.RigNames.Length; ++index)
      {
        CinemachineVirtualCamera cinemachineVirtualCamera = (CinemachineVirtualCamera) null;
        if (copyFrom != null && copyFrom.Length > index)
          cinemachineVirtualCamera = copyFrom[index];
        if (CinemachineFreeLook.CreateRigOverride != null)
        {
          rigs[index] = CinemachineFreeLook.CreateRigOverride(this, CinemachineFreeLook.RigNames[index], cinemachineVirtualCamera);
        }
        else
        {
          rigs[index] = new GameObject(CinemachineFreeLook.RigNames[index])
          {
            transform = {
              parent = this.transform
            }
          }.AddComponent<CinemachineVirtualCamera>();
          if ((UnityEngine.Object) cinemachineVirtualCamera != (UnityEngine.Object) null)
          {
            ReflectionHelpers.CopyFields((object) cinemachineVirtualCamera, (object) rigs[index]);
          }
          else
          {
            GameObject gameObject = rigs[index].GetComponentOwner().gameObject;
            gameObject.AddComponent<CinemachineOrbitalTransposer>();
            gameObject.AddComponent<CinemachineComposer>();
          }
        }
        rigs[index].InvalidateComponentPipeline();
        CinemachineOrbitalTransposer orbitalTransposer = rigs[index].GetCinemachineComponent<CinemachineOrbitalTransposer>();
        if ((UnityEngine.Object) orbitalTransposer == (UnityEngine.Object) null)
          orbitalTransposer = rigs[index].AddCinemachineComponent<CinemachineOrbitalTransposer>();
        if ((UnityEngine.Object) cinemachineVirtualCamera == (UnityEngine.Object) null)
        {
          orbitalTransposer.m_YawDamping = 0.0f;
          CinemachineComposer cinemachineComponent = rigs[index].GetCinemachineComponent<CinemachineComposer>();
          if ((UnityEngine.Object) cinemachineComponent != (UnityEngine.Object) null)
          {
            cinemachineComponent.m_HorizontalDamping = cinemachineComponent.m_VerticalDamping = 0.0f;
            cinemachineComponent.m_ScreenX = 0.5f;
            cinemachineComponent.m_ScreenY = numArray[index];
            cinemachineComponent.m_DeadZoneWidth = cinemachineComponent.m_DeadZoneHeight = 0.1f;
            cinemachineComponent.m_SoftZoneWidth = cinemachineComponent.m_SoftZoneHeight = 0.8f;
            cinemachineComponent.m_BiasX = cinemachineComponent.m_BiasY = 0.0f;
          }
        }
      }
      return rigs;
    }

    private void UpdateRigCache()
    {
      if (this.mIsDestroyed)
        return;
      if (this.m_Rigs != null && this.m_Rigs.Length == 3 && (UnityEngine.Object) this.m_Rigs[0] != (UnityEngine.Object) null && (UnityEngine.Object) this.m_Rigs[0].transform.parent != (UnityEngine.Object) this.transform)
      {
        this.DestroyRigs();
        this.m_Rigs = this.CreateRigs(this.m_Rigs);
      }
      if (this.mOrbitals != null && this.mOrbitals.Length == 3)
        return;
      if (this.LocateExistingRigs(CinemachineFreeLook.RigNames, false) != 3)
      {
        this.DestroyRigs();
        this.m_Rigs = this.CreateRigs((CinemachineVirtualCamera[]) null);
        this.LocateExistingRigs(CinemachineFreeLook.RigNames, true);
      }
      foreach (CinemachineVirtualCamera rig in this.m_Rigs)
      {
        CinemachineVirtualCamera cinemachineVirtualCamera = rig;
        string[] strArray;
        if (!this.m_CommonLens)
          strArray = new string[5]
          {
            "m_Script",
            "Header",
            "Extensions",
            "m_Priority",
            "m_Follow"
          };
        else
          strArray = new string[6]
          {
            "m_Script",
            "Header",
            "Extensions",
            "m_Priority",
            "m_Follow",
            "m_Lens"
          };
        cinemachineVirtualCamera.m_ExcludedPropertiesInInspector = strArray;
        rig.m_LockStageInInspector = new CinemachineCore.Stage[1];
      }
      this.mBlendA = new CinemachineBlend((ICinemachineCamera) this.m_Rigs[1], (ICinemachineCamera) this.m_Rigs[0], AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f), 1f, 0.0f);
      this.mBlendB = new CinemachineBlend((ICinemachineCamera) this.m_Rigs[2], (ICinemachineCamera) this.m_Rigs[1], AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f), 1f, 0.0f);
      this.m_XAxis.SetThresholds(0.0f, 360f, true);
      this.m_YAxis.SetThresholds(0.0f, 1f, false);
    }

    private int LocateExistingRigs(string[] rigNames, bool forceOrbital)
    {
      this.mOrbitals = new CinemachineOrbitalTransposer[rigNames.Length];
      this.m_Rigs = new CinemachineVirtualCamera[rigNames.Length];
      int num = 0;
      foreach (Transform transform in this.transform)
      {
        CinemachineVirtualCamera component = transform.GetComponent<CinemachineVirtualCamera>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          GameObject gameObject = transform.gameObject;
          for (int index = 0; index < rigNames.Length; ++index)
          {
            if ((UnityEngine.Object) this.mOrbitals[index] == (UnityEngine.Object) null && gameObject.name == rigNames[index])
            {
              this.mOrbitals[index] = component.GetCinemachineComponent<CinemachineOrbitalTransposer>();
              if ((UnityEngine.Object) this.mOrbitals[index] == (UnityEngine.Object) null & forceOrbital)
                this.mOrbitals[index] = component.AddCinemachineComponent<CinemachineOrbitalTransposer>();
              if ((UnityEngine.Object) this.mOrbitals[index] != (UnityEngine.Object) null)
              {
                this.mOrbitals[index].m_HeadingIsSlave = true;
                if (index == 0)
                  this.mOrbitals[index].HeadingUpdater = (CinemachineOrbitalTransposer.UpdateHeadingDelegate) ((orbital, deltaTime, up) => orbital.UpdateHeading(deltaTime, up, ref this.m_XAxis));
                this.m_Rigs[index] = component;
                ++num;
              }
            }
          }
        }
      }
      return num;
    }

    private void PushSettingsToRigs()
    {
      this.UpdateRigCache();
      for (int index = 0; index < this.m_Rigs.Length; ++index)
      {
        if (!((UnityEngine.Object) this.m_Rigs[index] == (UnityEngine.Object) null))
        {
          if (this.m_CommonLens)
            this.m_Rigs[index].m_Lens = this.m_Lens;
          if (this.mUseLegacyRigDefinitions)
          {
            this.mUseLegacyRigDefinitions = false;
            this.m_Orbits[index].m_Height = this.mOrbitals[index].m_FollowOffset.y;
            this.m_Orbits[index].m_Radius = -this.mOrbitals[index].m_FollowOffset.z;
            if ((UnityEngine.Object) this.m_Rigs[index].Follow != (UnityEngine.Object) null)
              this.Follow = this.m_Rigs[index].Follow;
          }
          this.m_Rigs[index].Follow = (Transform) null;
          if (CinemachineCore.sShowHiddenObjects)
            this.m_Rigs[index].gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
          else
            this.m_Rigs[index].gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
          this.mOrbitals[index].m_FollowOffset = this.GetLocalPositionForCameraFromInput(this.m_YAxis.Value);
          this.mOrbitals[index].m_BindingMode = this.m_BindingMode;
          this.mOrbitals[index].m_Heading = this.m_Heading;
          this.mOrbitals[index].m_XAxis = this.m_XAxis;
          this.mOrbitals[index].m_RecenterToTargetHeading = this.m_RecenterToTargetHeading;
          if (index > 0)
            this.mOrbitals[index].m_RecenterToTargetHeading.m_enabled = false;
          if (this.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
            this.m_Rigs[index].SetStateRawPosition(this.State.RawPosition);
        }
      }
    }

    private CameraState CalculateNewState(Vector3 worldUp, float deltaTime)
    {
      CameraState newState = this.PullStateFromVirtualCamera(worldUp);
      float num = this.m_YAxis.Value;
      if ((double) num > 0.5)
      {
        if (this.mBlendA != null)
        {
          this.mBlendA.TimeInBlend = (float) (((double) num - 0.5) * 2.0);
          this.mBlendA.UpdateCameraState(worldUp, deltaTime);
          newState = this.mBlendA.State;
        }
      }
      else if (this.mBlendB != null)
      {
        this.mBlendB.TimeInBlend = num * 2f;
        this.mBlendB.UpdateCameraState(worldUp, deltaTime);
        newState = this.mBlendB.State;
      }
      return newState;
    }

    private CameraState PullStateFromVirtualCamera(Vector3 worldUp)
    {
      CameraState cameraState = CameraState.Default with
      {
        RawPosition = this.transform.position,
        RawOrientation = this.transform.rotation,
        ReferenceUp = worldUp
      };
      CinemachineBrain potentialTargetBrain = CinemachineCore.Instance.FindPotentialTargetBrain((ICinemachineCamera) this);
      this.m_Lens.Aspect = (UnityEngine.Object) potentialTargetBrain != (UnityEngine.Object) null ? potentialTargetBrain.OutputCamera.aspect : 1f;
      this.m_Lens.Orthographic = (UnityEngine.Object) potentialTargetBrain != (UnityEngine.Object) null && potentialTargetBrain.OutputCamera.orthographic;
      cameraState.Lens = this.m_Lens;
      return cameraState;
    }

    public Vector3 GetLocalPositionForCameraFromInput(float t)
    {
      if (this.mOrbitals == null)
        return Vector3.zero;
      this.UpdateCachedSpline();
      int index = 1;
      if ((double) t > 0.5)
      {
        t -= 0.5f;
        index = 2;
      }
      return SplineHelpers.Bezier3(t * 2f, (Vector3) this.m_CachedKnots[index], (Vector3) this.m_CachedCtrl1[index], (Vector3) this.m_CachedCtrl2[index], (Vector3) this.m_CachedKnots[index + 1]);
    }

    private void UpdateCachedSpline()
    {
      bool flag = this.m_CachedOrbits != null && (double) this.m_CachedTension == (double) this.m_SplineCurvature;
      for (int index = 0; index < 3 & flag; ++index)
        flag = (double) this.m_CachedOrbits[index].m_Height == (double) this.m_Orbits[index].m_Height && (double) this.m_CachedOrbits[index].m_Radius == (double) this.m_Orbits[index].m_Radius;
      if (flag)
        return;
      float splineCurvature = this.m_SplineCurvature;
      this.m_CachedKnots = new Vector4[5];
      this.m_CachedCtrl1 = new Vector4[5];
      this.m_CachedCtrl2 = new Vector4[5];
      this.m_CachedKnots[1] = new Vector4(0.0f, this.m_Orbits[2].m_Height, -this.m_Orbits[2].m_Radius, 0.0f);
      this.m_CachedKnots[2] = new Vector4(0.0f, this.m_Orbits[1].m_Height, -this.m_Orbits[1].m_Radius, 0.0f);
      this.m_CachedKnots[3] = new Vector4(0.0f, this.m_Orbits[0].m_Height, -this.m_Orbits[0].m_Radius, 0.0f);
      this.m_CachedKnots[0] = Vector4.Lerp(this.m_CachedKnots[1], Vector4.zero, splineCurvature);
      this.m_CachedKnots[4] = Vector4.Lerp(this.m_CachedKnots[3], Vector4.zero, splineCurvature);
      SplineHelpers.ComputeSmoothControlPoints(ref this.m_CachedKnots, ref this.m_CachedCtrl1, ref this.m_CachedCtrl2);
      this.m_CachedOrbits = new CinemachineFreeLook.Orbit[3];
      for (int index = 0; index < 3; ++index)
        this.m_CachedOrbits[index] = this.m_Orbits[index];
      this.m_CachedTension = this.m_SplineCurvature;
    }

    [Serializable]
    public struct Orbit
    {
      public float m_Height;
      public float m_Radius;

      public Orbit(float h, float r)
      {
        this.m_Height = h;
        this.m_Radius = r;
      }
    }

    public delegate CinemachineVirtualCamera CreateRigDelegate(
      CinemachineFreeLook vcam,
      string name,
      CinemachineVirtualCamera copyFrom);

    public delegate void DestroyRigDelegate(GameObject rig);
  }
}
