using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  public class GlobalFogNonopaque : GlobalFogBase
  {
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.OnRenderImage_Internal(source, destination);
    }
  }
}
