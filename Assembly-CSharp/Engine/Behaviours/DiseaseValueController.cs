using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using StateSetters;
using UnityEngine;

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
      component = owner.GetComponent<DiseaseComponent>();
      if (component == null)
        return;
      component.OnCurrentDiseaseValueChanged += OnCurrentDiseaseValueChanged;
      OnCurrentDiseaseValueChanged(component.CurrentDiseaseValue);
    }

    public void Detach()
    {
      if (component == null)
        return;
      component.OnCurrentDiseaseValueChanged -= OnCurrentDiseaseValueChanged;
      component = null;
    }

    private void OnCurrentDiseaseValueChanged(float value) => UpdateValue();

    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    private float DiseaseValue
    {
      set
      {
        diseaseValue = Mathf.Clamp01(value);
        diseaseValueState.Apply(diseaseValue);
      }
      get => diseaseValue;
    }

    private void VisirEnableListener_OnVisibleChanged(bool visible) => UpdateValue();

    private void UpdateValue()
    {
      if (component == null)
        return;
      DiseaseValue = component.CurrentDiseaseValue;
    }
  }
}
