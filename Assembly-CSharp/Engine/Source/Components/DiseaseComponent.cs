// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.DiseaseComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory(typeof (IDiseaseComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class DiseaseComponent : 
    EngineComponent,
    IDiseaseComponent,
    IComponent,
    IUpdatable,
    INeedSave
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected float diseaseValue;
    [Inspected]
    private float startDiseaseValue;
    [Inspected]
    private float currentDiseaseValue;
    [Inspected]
    private TimeSpan startTime;
    [Inspected]
    private TimeSpan endTime;
    [Inspected]
    private bool update;

    public event Action<float> OnCurrentDiseaseValueChanged;

    public override void OnRemoved()
    {
      base.OnRemoved();
      if (!this.update)
        return;
      this.update = false;
      InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.RemoveUpdatable((IUpdatable) this);
    }

    public void SetDiseaseValue(float value, TimeSpan deltaTime)
    {
      this.startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      if (deltaTime.Ticks <= 0L)
      {
        this.endTime = this.startTime;
        this.currentDiseaseValue = value;
        this.startDiseaseValue = value;
        if (this.update)
        {
          this.update = false;
          InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.RemoveUpdatable((IUpdatable) this);
        }
      }
      else
      {
        this.endTime = this.startTime + deltaTime;
        this.currentDiseaseValue = this.diseaseValue;
        this.startDiseaseValue = this.diseaseValue;
        if (!this.update)
        {
          this.update = true;
          InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.AddUpdatable((IUpdatable) this);
        }
      }
      this.diseaseValue = value;
      Action<float> diseaseValueChanged = this.OnCurrentDiseaseValueChanged;
      if (diseaseValueChanged == null)
        return;
      diseaseValueChanged(this.currentDiseaseValue);
    }

    public float DiseaseValue => this.diseaseValue;

    public float CurrentDiseaseValue => this.currentDiseaseValue;

    public bool NeedSave
    {
      get
      {
        if (!(this.Owner.Template is IEntity template))
        {
          Debug.LogError((object) ("Template not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        DiseaseComponent component = template.GetComponent<DiseaseComponent>();
        if (component == null)
        {
          Debug.LogError((object) (this.GetType().Name + " not found, owner : " + this.Owner.GetInfo()));
          return true;
        }
        return (double) this.diseaseValue != (double) component.diseaseValue;
      }
    }

    public void ComputeUpdate()
    {
      if (!this.update)
        return;
      TimeSpan absoluteGameTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
      if (absoluteGameTime >= this.endTime)
      {
        this.currentDiseaseValue = this.diseaseValue;
        this.update = false;
        InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.RemoveUpdatable((IUpdatable) this);
        Action<float> diseaseValueChanged = this.OnCurrentDiseaseValueChanged;
        if (diseaseValueChanged == null)
          return;
        diseaseValueChanged(this.currentDiseaseValue);
      }
      else
      {
        TimeSpan timeSpan = absoluteGameTime - this.startTime;
        double ticks1 = (double) timeSpan.Ticks;
        timeSpan = this.endTime - this.startTime;
        double ticks2 = (double) timeSpan.Ticks;
        this.currentDiseaseValue = Mathf.Lerp(this.startDiseaseValue, this.diseaseValue, Mathf.Clamp01((float) (ticks1 / ticks2)));
        Action<float> diseaseValueChanged = this.OnCurrentDiseaseValueChanged;
        if (diseaseValueChanged == null)
          return;
        diseaseValueChanged(this.currentDiseaseValue);
      }
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded()
    {
      this.currentDiseaseValue = this.diseaseValue;
      Action<float> diseaseValueChanged = this.OnCurrentDiseaseValueChanged;
      if (diseaseValueChanged == null)
        return;
      diseaseValueChanged(this.currentDiseaseValue);
    }
  }
}
