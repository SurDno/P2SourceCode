using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (ReflectionProbe))]
[ExecuteInEditMode]
public class EnvironmentProbe : MonoBehaviour
{
  [SerializeField]
  private float ambientIntensity = 1f;
  [SerializeField]
  private float ambientPower = 1f;

  public float AmbientIntensity
  {
    get => this.ambientIntensity;
    set => this.ambientIntensity = value;
  }

  private void OnDisable()
  {
    Shader.SetGlobalInt("Pathologic_AmbientCubemapLod", 0);
    Shader.SetGlobalTexture("Pathologic_AmbientCubemap", (Texture) null);
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", 0);
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", (Texture) null);
  }

  private void LateUpdate()
  {
    ReflectionProbe component = this.GetComponent<ReflectionProbe>();
    int resolution = component.resolution;
    int num = 5;
    if (resolution == 16 || resolution == 1024)
      num = 4;
    else if (resolution == 2048)
      num = 3;
    Shader.SetGlobalInt("Pathologic_AmbientCubemapLod", num);
    Shader.SetGlobalFloat("Pathologic_AmbientCubemapIntensity", this.ambientIntensity);
    Shader.SetGlobalFloat("Pathologic_AmbientCubemapPower", this.ambientPower);
    Shader.SetGlobalTexture("Pathologic_AmbientCubemap", component.texture);
    RenderSettings.ambientMode = AmbientMode.Flat;
    RenderSettings.ambientLight = Color.blue;
    RenderSettings.ambientIntensity = 1f;
  }
}
