using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.Detectablies;
using Engine.Source.Settings.External;
using Engine.Source.Utility;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IDetectorComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DetectorComponent : 
    EngineComponent,
    IDetectorComponent,
    IComponent,
    IUpdatable,
    INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isEnabled = true;
    [Inspected]
    private List<DetectableCandidatInfo> eyeCandidates = new List<DetectableCandidatInfo>();
    [Inspected]
    private List<DetectableCandidatInfo> hearingCandidates = new List<DetectableCandidatInfo>();
    private HashSet<IDetectableComponent> visible = new HashSet<IDetectableComponent>();
    private HashSet<IDetectableComponent> hearing = new HashSet<IDetectableComponent>();
    [FromThis]
    private ILocationItemComponent locationItem;
    [FromThis]
    private ParametersComponent parametersComponent;
    private static HashSet<IDetectableComponent> tmps = new HashSet<IDetectableComponent>();
    private static HashSet<IDetectableComponent> tmps2 = new HashSet<IDetectableComponent>();
    private static List<RaycastHit> raycastBuffer = new List<RaycastHit>();
    private IParameter<float> eyeDistanceParameter;
    private IParameter<float> eyeAngleParameter;
    private IParameter<float> hearingDistanceParameter;
    private bool isIndoor;
    private GameObject gameObject;
    private Pivot pivot;
    private bool updateSkipped;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.isEnabled;
      set
      {
        this.isEnabled = value;
        this.OnChangeEnabled();
      }
    }

    [Inspected]
    public HashSet<IDetectableComponent> Visible => this.visible;

    [Inspected]
    public HashSet<IDetectableComponent> Hearing => this.hearing;

    [Inspected]
    public float EyeDistance
    {
      get => this.eyeDistanceParameter != null ? this.eyeDistanceParameter.Value : 0.0f;
    }

    [Inspected]
    public float BaseEyeDistance
    {
      get => this.eyeDistanceParameter != null ? this.eyeDistanceParameter.BaseValue : 0.0f;
    }

    [Inspected]
    public float EyeAngle => this.eyeAngleParameter != null ? this.eyeAngleParameter.Value : 0.0f;

    [Inspected]
    public float HearingDistance
    {
      get => this.hearingDistanceParameter != null ? this.hearingDistanceParameter.Value : 0.0f;
    }

    [Inspected]
    public float BaseHearingDistance
    {
      get => this.hearingDistanceParameter != null ? this.hearingDistanceParameter.BaseValue : 0.0f;
    }

    public bool NeedSave
    {
      get
      {
        if (!(this.Owner.Template is IEntity template))
        {
          Debug.LogError((object) ("Template not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        DetectorComponent component = template.GetComponent<DetectorComponent>();
        if (component == null)
        {
          Debug.LogError((object) (this.GetType().Name + " not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        return this.isEnabled != component.isEnabled;
      }
    }

    public event Action<IDetectableComponent> OnSee;

    public event Action<IDetectableComponent> OnStopSee;

    public event Action<IDetectableComponent> OnHear;

    public event Action<IDetectableComponent> OnStopHear;

    public override void OnAdded()
    {
      base.OnAdded();
      if (this.parametersComponent != null)
      {
        this.eyeDistanceParameter = this.parametersComponent.GetByName<float>(ParameterNameEnum.EyeDistance);
        this.eyeAngleParameter = this.parametersComponent.GetByName<float>(ParameterNameEnum.EyeAngle);
        this.hearingDistanceParameter = this.parametersComponent.GetByName<float>(ParameterNameEnum.HearingDistance);
      }
      this.locationItem.OnHibernationChanged += new Action<ILocationItemComponent>(this.LocationItemOnChangeHibernation);
      this.locationItem.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.LocationItemOnLocationChanged);
      this.isIndoor = this.locationItem.IsIndoor;
      InstanceByRequest<UpdateService>.Instance.DetectorUpdater.AddUpdatable((IUpdatable) this);
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.Owner_OnGameObjectChangedEvent);
    }

    public override void OnRemoved()
    {
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.Owner_OnGameObjectChangedEvent);
      this.locationItem.OnHibernationChanged -= new Action<ILocationItemComponent>(this.LocationItemOnChangeHibernation);
      this.locationItem.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.LocationItemOnLocationChanged);
      this.locationItem = (ILocationItemComponent) null;
      InstanceByRequest<UpdateService>.Instance.DetectorUpdater.RemoveUpdatable((IUpdatable) this);
      base.OnRemoved();
    }

    private void Owner_OnGameObjectChangedEvent()
    {
      this.gameObject = ((IEntityView) this.Owner).GameObject;
      this.pivot = (UnityEngine.Object) this.gameObject != (UnityEngine.Object) null ? this.gameObject.GetComponent<Pivot>() : (Pivot) null;
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !this.Owner.IsEnabledInHierarchy || !this.IsEnabled || this.locationItem.IsHibernation)
        return;
      IEntityView owner = (IEntityView) this.Owner;
      if (!owner.IsAttached)
        return;
      if ((UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
        throw new Exception("gameObject == null , owner : " + this.Owner.GetInfo());
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjects && !DetectorUtility.CheckDistance(owner.Position, EngineApplication.PlayerPosition, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjectsDistance))
      {
        this.updateSkipped = !this.updateSkipped;
        if (this.updateSkipped)
          return;
      }
      this.ComputeCandidats();
      this.ComputeEye();
      this.ComputeHearing();
    }

    private void ComputeHearing() => this.ComputeHearing(this.hearingCandidates);

    private void ComputeEye() => this.ComputeEye(this.eyeCandidates);

    private void ComputeEye(List<DetectableCandidatInfo> candidates)
    {
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null || (UnityEngine.Object) this.pivot.Head == (UnityEngine.Object) null)
        return;
      DetectorComponent.tmps.Clear();
      float eyeAngle = this.EyeAngle;
      float eyeDistance = this.EyeDistance;
      for (int index1 = 0; index1 < candidates.Count; ++index1)
      {
        DetectableCandidatInfo candidate = candidates[index1];
        Vector3 position = ((IEntityView) candidate.Detectable.Owner).Position;
        Vector3 forward = position - ((IEntityView) this.Owner).Position;
        float magnitude1 = forward.magnitude;
        Quaternion rotation = ((IEntityView) this.Owner).Rotation;
        Quaternion quaternion = Quaternion.identity;
        if (!Mathf.Approximately(magnitude1, 0.0f))
          quaternion = rotation * Quaternion.Inverse(Quaternion.LookRotation(forward));
        float f = Mathf.DeltaAngle(quaternion.eulerAngles.y, 0.0f);
        if ((double) Mathf.Abs(f) < (double) eyeAngle * 0.5)
        {
          float num1 = FunctionUtility.EyeFunction(f + eyeAngle * 0.5f, eyeAngle);
          float num2 = candidate.Detectable.VisibleDistance + eyeDistance * num1;
          if ((double) magnitude1 <= (double) num2)
          {
            Vector3 direction = position + candidate.Offset - this.pivot.Head.transform.position;
            float magnitude2 = direction.magnitude;
            RaycastHit raycastHit1 = new RaycastHit();
            float num3 = float.MaxValue;
            LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
            PhysicsUtility.Raycast(DetectorComponent.raycastBuffer, this.pivot.Head.transform.position, direction, magnitude2, -1 ^ (int) triggerInteractLayer);
            for (int index2 = 0; index2 < DetectorComponent.raycastBuffer.Count; ++index2)
            {
              RaycastHit raycastHit2 = DetectorComponent.raycastBuffer[index2];
              if (!raycastHit2.collider.isTrigger && (double) raycastHit2.distance < (double) num3)
              {
                raycastHit1 = raycastHit2;
                num3 = raycastHit2.distance;
              }
            }
            if ((double) num3 <= (double) magnitude2 && !((UnityEngine.Object) raycastHit1.collider == (UnityEngine.Object) null) && !((UnityEngine.Object) raycastHit1.collider.gameObject != (UnityEngine.Object) candidate.GameObject))
              DetectorComponent.tmps.Add((IDetectableComponent) candidate.Detectable);
          }
        }
      }
      foreach (IDetectableComponent tmp in DetectorComponent.tmps)
      {
        if (this.visible.Add(tmp))
        {
          Action<IDetectableComponent> onSee = this.OnSee;
          if (onSee != null)
            onSee(tmp);
        }
      }
      DetectorComponent.tmps2.Clear();
      foreach (IDetectableComponent detectableComponent in this.visible)
      {
        if (!DetectorComponent.tmps.Contains(detectableComponent))
        {
          DetectorComponent.tmps2.Add(detectableComponent);
          Action<IDetectableComponent> onStopSee = this.OnStopSee;
          if (onStopSee != null)
            onStopSee(detectableComponent);
        }
      }
      foreach (IDetectableComponent detectableComponent in DetectorComponent.tmps2)
        this.visible.Remove(detectableComponent);
    }

    private void ComputeHearing(List<DetectableCandidatInfo> candidates)
    {
      DetectorComponent.tmps.Clear();
      float hearingDistance = this.HearingDistance;
      for (int index = 0; index < candidates.Count; ++index)
      {
        DetectableCandidatInfo candidate = candidates[index];
        if (this.isIndoor)
        {
          if (!DetectorUtility.CanHear(this.gameObject, ((IEntityView) this.Owner).Position, candidate.GameObject, ((IEntityView) candidate.Detectable.Owner).Position, hearingDistance, candidate.Detectable.NoiseDistance))
            continue;
        }
        else if ((double) (((IEntityView) candidate.Detectable.Owner).Position - ((IEntityView) this.Owner).Position).magnitude > (double) hearingDistance + (double) candidate.Detectable.NoiseDistance)
          continue;
        DetectorComponent.tmps.Add((IDetectableComponent) candidate.Detectable);
      }
      foreach (IDetectableComponent tmp in DetectorComponent.tmps)
      {
        if (this.hearing.Add(tmp))
        {
          Action<IDetectableComponent> onHear = this.OnHear;
          if (onHear != null)
            onHear(tmp);
        }
      }
      DetectorComponent.tmps2.Clear();
      foreach (IDetectableComponent detectableComponent in this.hearing)
      {
        if (!DetectorComponent.tmps.Contains(detectableComponent))
          DetectorComponent.tmps2.Add(detectableComponent);
      }
      foreach (IDetectableComponent detectableComponent in DetectorComponent.tmps2)
      {
        Action<IDetectableComponent> onStopHear = this.OnStopHear;
        if (onStopHear != null)
          onStopHear(detectableComponent);
        this.hearing.Remove(detectableComponent);
      }
    }

    private void LocationItemOnChangeHibernation(ILocationItemComponent sender)
    {
      if (!this.locationItem.IsHibernation)
        return;
      this.Cleanup();
    }

    private void LocationItemOnLocationChanged(
      ILocationItemComponent locationItem,
      ILocationComponent location)
    {
      this.isIndoor = locationItem.IsIndoor;
    }

    private void Cleanup()
    {
      foreach (IDetectableComponent detectableComponent in this.visible)
      {
        Action<IDetectableComponent> onStopSee = this.OnStopSee;
        if (onStopSee != null)
          onStopSee(detectableComponent);
      }
      this.visible.Clear();
      this.hearing.Clear();
    }

    public void ComputeCandidats()
    {
      this.eyeCandidates.Clear();
      this.hearingCandidates.Clear();
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.DisableDetectors)
        return;
      List<DetectableCandidatInfo> detectablies = ServiceLocator.GetService<DetectorService>().Detectablies;
      DetectorComponent detectorComponent = this;
      ILocationItemComponent locationItem = this.locationItem;
      if (!detectorComponent.Owner.IsEnabledInHierarchy || !detectorComponent.IsEnabled || locationItem == null || locationItem.IsHibernation)
        return;
      IEntityView owner1 = (IEntityView) detectorComponent.Owner;
      if (!owner1.IsAttached)
        return;
      float eyeDistance = this.EyeDistance;
      float hearingDistance = this.HearingDistance;
      Vector3 position1 = owner1.Position;
      foreach (DetectableCandidatInfo detectableCandidatInfo1 in detectablies)
      {
        IEntityView owner2 = (IEntityView) detectableCandidatInfo1.Detectable.Owner;
        if (owner2.IsAttached)
        {
          DetectableCandidatInfo detectableCandidatInfo2 = detectableCandidatInfo1;
          Vector3 position2 = owner2.Position;
          bool flag1 = DetectorUtility.CheckDistance(position1, position2, eyeDistance + detectableCandidatInfo2.Detectable.BaseVisibleDistance);
          bool flag2 = DetectorUtility.CheckDistance(position1, position2, hearingDistance + detectableCandidatInfo2.Detectable.NoiseDistance);
          if ((flag1 || flag2) && detectableCandidatInfo1.Detectable.Owner.IsEnabledInHierarchy && detectableCandidatInfo1.Detectable.IsEnabled && !detectableCandidatInfo2.LocationItem.IsHibernation && LocationItemUtility.CheckLocation(locationItem, detectableCandidatInfo2.LocationItem) && detectorComponent.Owner != detectableCandidatInfo1.Detectable.Owner)
          {
            if (flag1)
              this.eyeCandidates.Add(detectableCandidatInfo2);
            if (flag2)
              this.hearingCandidates.Add(detectableCandidatInfo2);
          }
        }
      }
    }
  }
}
