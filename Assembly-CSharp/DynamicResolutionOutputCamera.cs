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
    view.texture = targetTexture;
    view.gameObject.SetActive(targetTexture != null);
  }
}
