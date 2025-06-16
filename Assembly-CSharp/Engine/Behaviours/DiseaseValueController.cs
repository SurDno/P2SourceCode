// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.DiseaseValueController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using StateSetters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours
{
  public class DiseaseValueController : MonoBehaviour, IEntityAttachable
  {
    private DiseaseComponent component;
    private float diseaseValue;
    [SerializeField]
    private StateSetterItem[] diseaseValueState;

    public void Attach(IEntity owner)
    {
      this.component = owner.GetComponent<DiseaseComponent>();
      if (this.component == null)
        return;
      this.component.OnCurrentDiseaseValueChanged += new Action<float>(this.OnCurrentDiseaseValueChanged);
      this.OnCurrentDiseaseValueChanged(this.component.CurrentDiseaseValue);
    }

    public void Detach()
    {
      if (this.component == null)
        return;
      this.component.OnCurrentDiseaseValueChanged -= new Action<float>(this.OnCurrentDiseaseValueChanged);
      this.component = (DiseaseComponent) null;
    }

    private void OnCurrentDiseaseValueChanged(float value) => this.UpdateValue();

    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    private float DiseaseValue
    {
      set
      {
        this.diseaseValue = Mathf.Clamp01(value);
        this.diseaseValueState.Apply(this.diseaseValue);
      }
      get => this.diseaseValue;
    }

    private void VisirEnableListener_OnVisibleChanged(bool visible) => this.UpdateValue();

    private void UpdateValue()
    {
      if (this.component == null)
        return;
      this.DiseaseValue = this.component.CurrentDiseaseValue;
    }
  }
}
