using System;
using System.Collections.Generic;
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
    [StateSaveProxy]
    [StateLoadProxy]
    [DataReadProxy]
    [DataWriteProxy]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy()]
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
      get => isEnabled;
      set
      {
        isEnabled = value;
        OnChangeEnabled();
      }
    }

    [Inspected]
    public HashSet<IDetectableComponent> Visible => visible;

    [Inspected]
    public HashSet<IDetectableComponent> Hearing => hearing;

    [Inspected]
    public float EyeDistance
    {
      get => eyeDistanceParameter != null ? eyeDistanceParameter.Value : 0.0f;
    }

    [Inspected]
    public float BaseEyeDistance
    {
      get => eyeDistanceParameter != null ? eyeDistanceParameter.BaseValue : 0.0f;
    }

    [Inspected]
    public float EyeAngle => eyeAngleParameter != null ? eyeAngleParameter.Value : 0.0f;

    [Inspected]
    public float HearingDistance
    {
      get => hearingDistanceParameter != null ? hearingDistanceParameter.Value : 0.0f;
    }

    [Inspected]
    public float BaseHearingDistance
    {
      get => hearingDistanceParameter != null ? hearingDistanceParameter.BaseValue : 0.0f;
    }

    public bool NeedSave
    {
      get
      {
        if (!(Owner.Template is IEntity template))
        {
          Debug.LogError("Template not found, owner : " + Owner.GetInfo());
          return true;
        }
        DetectorComponent component = template.GetComponent<DetectorComponent>();
        if (component == null)
        {
          Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
          return true;
        }
        return isEnabled != component.isEnabled;
      }
    }

    public event Action<IDetectableComponent> OnSee;

    public event Action<IDetectableComponent> OnStopSee;

    public event Action<IDetectableComponent> OnHear;

    public event Action<IDetectableComponent> OnStopHear;

    public override void OnAdded()
    {
      base.OnAdded();
      if (parametersComponent != null)
      {
        eyeDistanceParameter = parametersComponent.GetByName<float>(ParameterNameEnum.EyeDistance);
        eyeAngleParameter = parametersComponent.GetByName<float>(ParameterNameEnum.EyeAngle);
        hearingDistanceParameter = parametersComponent.GetByName<float>(ParameterNameEnum.HearingDistance);
      }
      locationItem.OnHibernationChanged += LocationItemOnChangeHibernation;
      locationItem.OnChangeLocation += LocationItemOnLocationChanged;
      isIndoor = locationItem.IsIndoor;
      InstanceByRequest<UpdateService>.Instance.DetectorUpdater.AddUpdatable(this);
      ((IEntityView) Owner).OnGameObjectChangedEvent += Owner_OnGameObjectChangedEvent;
    }

    public override void OnRemoved()
    {
      ((IEntityView) Owner).OnGameObjectChangedEvent -= Owner_OnGameObjectChangedEvent;
      locationItem.OnHibernationChanged -= LocationItemOnChangeHibernation;
      locationItem.OnChangeLocation -= LocationItemOnLocationChanged;
      locationItem = null;
      InstanceByRequest<UpdateService>.Instance.DetectorUpdater.RemoveUpdatable(this);
      base.OnRemoved();
    }

    private void Owner_OnGameObjectChangedEvent()
    {
      gameObject = ((IEntityView) Owner).GameObject;
      pivot = gameObject != null ? gameObject.GetComponent<Pivot>() : null;
    }

    public void ComputeUpdate()
    {
      if (InstanceByRequest<EngineApplication>.Instance.IsPaused || !Owner.IsEnabledInHierarchy || !IsEnabled || locationItem.IsHibernation)
        return;
      IEntityView owner = (IEntityView) Owner;
      if (!owner.IsAttached)
        return;
      if (gameObject == null)
        throw new Exception("gameObject == null , owner : " + Owner.GetInfo());
      if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjects && !DetectorUtility.CheckDistance(owner.Position, EngineApplication.PlayerPosition, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.ReduceUpdateFarObjectsDistance))
      {
        updateSkipped = !updateSkipped;
        if (updateSkipped)
          return;
      }
      ComputeCandidats();
      ComputeEye();
      ComputeHearing();
    }

    private void ComputeHearing() => ComputeHearing(hearingCandidates);

    private void ComputeEye() => ComputeEye(eyeCandidates);

    private void ComputeEye(List<DetectableCandidatInfo> candidates)
    {
      if (pivot == null || pivot.Head == null)
        return;
      tmps.Clear();
      float eyeAngle = EyeAngle;
      float eyeDistance = EyeDistance;
      for (int index1 = 0; index1 < candidates.Count; ++index1)
      {
        DetectableCandidatInfo candidate = candidates[index1];
        Vector3 position = ((IEntityView) candidate.Detectable.Owner).Position;
        Vector3 forward = position - ((IEntityView) Owner).Position;
        float magnitude1 = forward.magnitude;
        Quaternion rotation = ((IEntityView) Owner).Rotation;
        Quaternion quaternion = Quaternion.identity;
        if (!Mathf.Approximately(magnitude1, 0.0f))
          quaternion = rotation * Quaternion.Inverse(Quaternion.LookRotation(forward));
        float f = Mathf.DeltaAngle(quaternion.eulerAngles.y, 0.0f);
        if (Mathf.Abs(f) < eyeAngle * 0.5)
        {
          float num1 = FunctionUtility.EyeFunction(f + eyeAngle * 0.5f, eyeAngle);
          float num2 = candidate.Detectable.VisibleDistance + eyeDistance * num1;
          if (magnitude1 <= (double) num2)
          {
            Vector3 direction = position + candidate.Offset - pivot.Head.transform.position;
            float magnitude2 = direction.magnitude;
            RaycastHit raycastHit1 = new RaycastHit();
            float num3 = float.MaxValue;
            LayerMask triggerInteractLayer = ScriptableObjectInstance<GameSettingsData>.Instance.TriggerInteractLayer;
            PhysicsUtility.Raycast(raycastBuffer, pivot.Head.transform.position, direction, magnitude2, -1 ^ triggerInteractLayer);
            for (int index2 = 0; index2 < raycastBuffer.Count; ++index2)
            {
              RaycastHit raycastHit2 = raycastBuffer[index2];
              if (!raycastHit2.collider.isTrigger && raycastHit2.distance < (double) num3)
              {
                raycastHit1 = raycastHit2;
                num3 = raycastHit2.distance;
              }
            }
            if (num3 <= (double) magnitude2 && !(raycastHit1.collider == null) && !(raycastHit1.collider.gameObject != candidate.GameObject))
              tmps.Add(candidate.Detectable);
          }
        }
      }
      foreach (IDetectableComponent tmp in tmps)
      {
        if (visible.Add(tmp))
        {
          Action<IDetectableComponent> onSee = OnSee;
          if (onSee != null)
            onSee(tmp);
        }
      }
      tmps2.Clear();
      foreach (IDetectableComponent detectableComponent in visible)
      {
        if (!tmps.Contains(detectableComponent))
        {
          tmps2.Add(detectableComponent);
          Action<IDetectableComponent> onStopSee = OnStopSee;
          if (onStopSee != null)
            onStopSee(detectableComponent);
        }
      }
      foreach (IDetectableComponent detectableComponent in tmps2)
        visible.Remove(detectableComponent);
    }

    private void ComputeHearing(List<DetectableCandidatInfo> candidates)
    {
      tmps.Clear();
      float hearingDistance = HearingDistance;
      for (int index = 0; index < candidates.Count; ++index)
      {
        DetectableCandidatInfo candidate = candidates[index];
        if (isIndoor)
        {
          if (!DetectorUtility.CanHear(gameObject, ((IEntityView) Owner).Position, candidate.GameObject, ((IEntityView) candidate.Detectable.Owner).Position, hearingDistance, candidate.Detectable.NoiseDistance))
            continue;
        }
        else if ((((IEntityView) candidate.Detectable.Owner).Position - ((IEntityView) Owner).Position).magnitude > hearingDistance + (double) candidate.Detectable.NoiseDistance)
          continue;
        tmps.Add(candidate.Detectable);
      }
      foreach (IDetectableComponent tmp in tmps)
      {
        if (hearing.Add(tmp))
        {
          Action<IDetectableComponent> onHear = OnHear;
          if (onHear != null)
            onHear(tmp);
        }
      }
      tmps2.Clear();
      foreach (IDetectableComponent detectableComponent in hearing)
      {
        if (!tmps.Contains(detectableComponent))
          tmps2.Add(detectableComponent);
      }
      foreach (IDetectableComponent detectableComponent in tmps2)
      {
        Action<IDetectableComponent> onStopHear = OnStopHear;
        if (onStopHear != null)
          onStopHear(detectableComponent);
        hearing.Remove(detectableComponent);
      }
    }

    private void LocationItemOnChangeHibernation(ILocationItemComponent sender)
    {
      if (!locationItem.IsHibernation)
        return;
      Cleanup();
    }

    private void LocationItemOnLocationChanged(
      ILocationItemComponent locationItem,
      ILocationComponent location)
    {
      isIndoor = locationItem.IsIndoor;
    }

    private void Cleanup()
    {
      foreach (IDetectableComponent detectableComponent in visible)
      {
        Action<IDetectableComponent> onStopSee = OnStopSee;
        if (onStopSee != null)
          onStopSee(detectableComponent);
      }
      visible.Clear();
      hearing.Clear();
    }

    public void ComputeCandidats()
    {
      eyeCandidates.Clear();
      hearingCandidates.Clear();
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
      float eyeDistance = EyeDistance;
      float hearingDistance = HearingDistance;
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
              eyeCandidates.Add(detectableCandidatInfo2);
            if (flag2)
              hearingCandidates.Add(detectableCandidatInfo2);
          }
        }
      }
    }
  }
}
