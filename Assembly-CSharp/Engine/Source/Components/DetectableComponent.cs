// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.DetectableComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Detectors;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Services;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Required(typeof (ParametersComponent))]
  [Factory(typeof (IDetectableComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DetectableComponent : EngineComponent, IDetectableComponent, IComponent, INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    [CopyableProxy(MemberEnum.None)]
    protected bool isEnabled = true;
    private IParameter<float> visibleDistanceParameter;
    private IParameter<DetectType> visibleDetectTypeParameter;
    private IParameter<float> noiseDistanceParameter;
    private IParameter<DetectType> noiseDetectTypeParameter;
    [FromThis]
    private ParametersComponent parametersComponent;
    [FromThis]
    private LocationItemComponent locationItemComponent;

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
    public float BaseVisibleDistance
    {
      get => this.visibleDistanceParameter != null ? this.visibleDistanceParameter.BaseValue : 0.0f;
    }

    [Inspected]
    public float VisibleDistance
    {
      get => this.visibleDistanceParameter != null ? this.visibleDistanceParameter.Value : 0.0f;
    }

    [Inspected]
    public DetectType VisibleDetectType
    {
      get
      {
        return this.visibleDetectTypeParameter != null ? this.visibleDetectTypeParameter.Value : DetectType.None;
      }
    }

    [Inspected]
    public float NoiseDistance
    {
      get => this.noiseDistanceParameter != null ? this.noiseDistanceParameter.Value : 0.0f;
    }

    [Inspected]
    public DetectType NoiseDetectType
    {
      get
      {
        return this.noiseDetectTypeParameter != null ? this.noiseDetectTypeParameter.Value : DetectType.None;
      }
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
        DetectableComponent component = template.GetComponent<DetectableComponent>();
        if (component == null)
        {
          Debug.LogError((object) (this.GetType().Name + " not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        return this.isEnabled != component.isEnabled;
      }
    }

    public override void OnAdded()
    {
      base.OnAdded();
      if (this.parametersComponent != null)
      {
        this.visibleDistanceParameter = this.parametersComponent.GetByName<float>(ParameterNameEnum.VisibleDistance);
        this.visibleDetectTypeParameter = this.parametersComponent.GetByName<DetectType>(ParameterNameEnum.VisibleDetectType);
        this.noiseDistanceParameter = this.parametersComponent.GetByName<float>(ParameterNameEnum.NoiseDistance);
        this.noiseDetectTypeParameter = this.parametersComponent.GetByName<DetectType>(ParameterNameEnum.NoiseDetectType);
      }
      ((IEntityView) this.Owner).OnGameObjectChangedEvent += new Action(this.OnGameObjectChangedEvent);
      this.locationItemComponent.OnHibernationChanged += new Action<ILocationItemComponent>(this.OnHibernationChanged);
      this.UpdateSunscribe();
    }

    private void OnHibernationChanged(ILocationItemComponent sender) => this.UpdateSunscribe();

    private void OnGameObjectChangedEvent() => this.UpdateSunscribe();

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      if (this.locationItemComponent == null)
        return;
      this.UpdateSunscribe();
    }

    public override void OnRemoved()
    {
      this.visibleDistanceParameter = (IParameter<float>) null;
      this.noiseDistanceParameter = (IParameter<float>) null;
      this.noiseDistanceParameter = (IParameter<float>) null;
      this.noiseDetectTypeParameter = (IParameter<DetectType>) null;
      ((IEntityView) this.Owner).OnGameObjectChangedEvent -= new Action(this.OnGameObjectChangedEvent);
      this.locationItemComponent.OnHibernationChanged -= new Action<ILocationItemComponent>(this.OnHibernationChanged);
      ServiceLocator.GetService<DetectorService>().RemoveDetectable(this);
      base.OnRemoved();
    }

    private void UpdateSunscribe()
    {
      if (!this.locationItemComponent.IsHibernation && ((IEntityView) this.Owner).IsAttached && this.Owner.IsEnabledInHierarchy && this.IsEnabled)
        ServiceLocator.GetService<DetectorService>().AddDetectable(this);
      else
        ServiceLocator.GetService<DetectorService>().RemoveDetectable(this);
    }
  }
}
