[RequireComponent(typeof (Camera))]
public class DynamicResolutionOutputCamera : MonoBehaviour
{
  [SerializeField]
  private RawImage view;

  private void OnPreCull()
  {
    RenderTexture targetTexture = DynamicResolution.Instance.GetTargetTexture();
    view.texture = (Texture) targetTexture;
    view.gameObject.SetActive((Object) targetTexture != (Object) null);
  }
}
