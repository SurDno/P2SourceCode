using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class CompareMeter : MonoBehaviour
  {
    private float currentValue = float.NaN;
    private float factor;
    private float targetValue = float.NaN;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Allowed")]
    private Button unityAllowed;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Forbidden")]
    private Button unityForbidden;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Marker")]
    private Image unityMarker;
    [SerializeField]
    [FormerlySerializedAs("_Unity_Separator")]
    private Image unitySeparator;

    public float TargetValue
    {
      get => this.targetValue;
      set
      {
        this.targetValue = value;
        this.Calculate();
      }
    }

    public float CurrentValue
    {
      get => this.currentValue;
      set
      {
        this.currentValue = value;
        this.Calculate();
      }
    }

    public float Factor => this.factor;

    protected void Start() => this.Calculate();

    protected void Calculate()
    {
      bool flag = false;
      if (Mathf.Approximately(this.currentValue, this.targetValue))
      {
        this.factor = 1f;
        flag = true;
      }
      else if ((double) this.currentValue < (double) this.targetValue)
      {
        this.factor = this.currentValue / this.targetValue;
      }
      else
      {
        this.factor = this.targetValue / this.currentValue;
        this.factor = (float) (1.0 - (double) this.factor + 1.0);
      }
      if (float.IsNaN(this.factor) || float.IsPositiveInfinity(this.factor) || float.IsNegativeInfinity(this.factor))
      {
        this.unityAllowed.interactable = false;
        this.unityForbidden.interactable = false;
        this.unityMarker.gameObject.SetActive(false);
      }
      else if (flag)
      {
        this.unityAllowed.interactable = true;
        this.unityForbidden.interactable = false;
        this.unityMarker.gameObject.SetActive(true);
        Vector3 position = this.unityMarker.gameObject.transform.position;
        Rect rect = this.unitySeparator.GetComponent<RectTransform>().rect;
        position.x = this.unitySeparator.gameObject.transform.position.x + rect.size.x * 0.5f;
        this.unityMarker.gameObject.transform.position = position;
      }
      else if ((double) this.factor < 1.0)
      {
        this.unityAllowed.interactable = false;
        this.unityForbidden.interactable = true;
        this.unityMarker.gameObject.SetActive(true);
        Vector3 position = this.unityMarker.gameObject.transform.position;
        Rect rect = this.unityForbidden.GetComponent<RectTransform>().rect;
        position.x = this.unityForbidden.gameObject.transform.position.x + (this.unitySeparator.gameObject.transform.position.x - this.unityForbidden.gameObject.transform.position.x) * Mathf.Clamp(this.factor, 0.0f, 1f);
        this.unityMarker.gameObject.transform.position = position;
      }
      else
      {
        this.unityAllowed.interactable = true;
        this.unityForbidden.interactable = false;
        this.unityMarker.gameObject.SetActive(true);
        Vector3 position = this.unityMarker.gameObject.transform.position;
        Rect rect = this.unityAllowed.GetComponent<RectTransform>().rect;
        position.x = this.unitySeparator.gameObject.transform.position.x + (this.unityAllowed.gameObject.transform.position.x - this.unitySeparator.gameObject.transform.position.x) * Mathf.Clamp(this.factor - 1f, 0.0f, 1f);
        this.unityMarker.gameObject.transform.position = position;
      }
    }
  }
}
