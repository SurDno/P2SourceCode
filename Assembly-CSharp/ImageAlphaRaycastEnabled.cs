[RequireComponent(typeof (Image))]
public class ImageAlphaRaycastEnabled : MonoBehaviour
{
  private void Start() => this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.25f;
}
