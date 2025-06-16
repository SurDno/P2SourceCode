[RequireComponent(typeof (CanvasScaler))]
public class CanvasScreenFit : MonoBehaviour
{
  private void OnEnable() => UpdateCanvasScale();

  private void Update() => UpdateCanvasScale();

  private void UpdateCanvasScale()
  {
    float num = (float) ((double) Screen.width / 16.0 / ((double) Screen.height / 9.0));
    this.GetComponent<CanvasScaler>().matchWidthOrHeight = num > 1.0 ? 1f : 0.0f;
  }
}
