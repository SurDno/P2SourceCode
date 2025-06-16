using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Camera))]
public class DynamicResolutionOutputCamera : MonoBehaviour
{
  [SerializeField]
  private RawImage view;

  private void OnPreCull()
  {
    RenderTexture targetTexture = DynamicResolution.Instance.GetTargetTexture();
    this.view.texture = (Texture) targetTexture;
    this.view.gameObject.SetActive((Object) targetTexture != (Object) null);
  }
}
