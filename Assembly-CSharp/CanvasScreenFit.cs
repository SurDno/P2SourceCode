using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (CanvasScaler))]
public class CanvasScreenFit : MonoBehaviour
{
  private void OnEnable() => UpdateCanvasScale();

  private void Update() => UpdateCanvasScale();

  private void UpdateCanvasScale()
  {
    float num = (float) (Screen.width / 16.0 / (Screen.height / 9.0));
    GetComponent<CanvasScaler>().matchWidthOrHeight = num > 1.0 ? 1f : 0.0f;
  }
}
