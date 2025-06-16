using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Light))]
public class LightShaftsSourceCapture : MonoBehaviour
{
  public LayerMask shadowCastingLayers;

  private void OnDisable()
  {
    LightShafts.isLightActive = false;
    Shader.SetGlobalColor("_SunlightColor", Color.black);
  }

  private void Update()
  {
    Light component = this.GetComponent<Light>();
    LightShafts.isLightActive = (double) component.intensity > 0.0;
    LightShafts.LightDirection = this.transform.forward;
    LightShafts.shadowCastingColliders = this.shadowCastingLayers;
    Shader.SetGlobalColor("_SunlightColor", (component.color * component.intensity) with
    {
      a = 1f
    });
  }
}
