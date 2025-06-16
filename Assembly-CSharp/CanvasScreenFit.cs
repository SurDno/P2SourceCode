using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (CanvasScaler))]
public class CanvasScreenFit : MonoBehaviour
{
  private void OnEnable() => this.UpdateCanvasScale();

  private void Update() => this.UpdateCanvasScale();

  private void UpdateCanvasScale()
  {
    float num = (float) ((double) Screen.width / 16.0 / ((double) Screen.height / 9.0));
    this.GetComponent<CanvasScaler>().matchWidthOrHeight = (double) num > 1.0 ? 1f : 0.0f;
  }
}
