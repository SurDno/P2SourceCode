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
      get => targetValue;
      set
      {
        targetValue = value;
        Calculate();
      }
    }

    public float CurrentValue
    {
      get => currentValue;
      set
      {
        currentValue = value;
        Calculate();
      }
    }

    public float Factor => factor;

    protected void Start() => Calculate();

    protected void Calculate()
    {
      bool flag = false;
      if (Mathf.Approximately(currentValue, targetValue))
      {
        factor = 1f;
        flag = true;
      }
      else if (currentValue < (double) targetValue)
      {
        factor = currentValue / targetValue;
      }
      else
      {
        factor = targetValue / currentValue;
        factor = (float) (1.0 - factor + 1.0);
      }
      if (float.IsNaN(factor) || float.IsPositiveInfinity(factor) || float.IsNegativeInfinity(factor))
      {
        unityAllowed.interactable = false;
        unityForbidden.interactable = false;
        unityMarker.gameObject.SetActive(false);
      }
      else if (flag)
      {
        unityAllowed.interactable = true;
        unityForbidden.interactable = false;
        unityMarker.gameObject.SetActive(true);
        Vector3 position = unityMarker.gameObject.transform.position;
        Rect rect = unitySeparator.GetComponent<RectTransform>().rect;
        position.x = unitySeparator.gameObject.transform.position.x + rect.size.x * 0.5f;
        unityMarker.gameObject.transform.position = position;
      }
      else if (factor < 1.0)
      {
        unityAllowed.interactable = false;
        unityForbidden.interactable = true;
        unityMarker.gameObject.SetActive(true);
        Vector3 position = unityMarker.gameObject.transform.position;
        Rect rect = unityForbidden.GetComponent<RectTransform>().rect;
        position.x = unityForbidden.gameObject.transform.position.x + (unitySeparator.gameObject.transform.position.x - unityForbidden.gameObject.transform.position.x) * Mathf.Clamp(factor, 0.0f, 1f);
        unityMarker.gameObject.transform.position = position;
      }
      else
      {
        unityAllowed.interactable = true;
        unityForbidden.interactable = false;
        unityMarker.gameObject.SetActive(true);
        Vector3 position = unityMarker.gameObject.transform.position;
        Rect rect = unityAllowed.GetComponent<RectTransform>().rect;
        position.x = unitySeparator.gameObject.transform.position.x + (unityAllowed.gameObject.transform.position.x - unitySeparator.gameObject.transform.position.x) * Mathf.Clamp(factor - 1f, 0.0f, 1f);
        unityMarker.gameObject.transform.position = position;
      }
    }
  }
}
