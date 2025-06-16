using UnityEngine;

namespace Rain
{
  [ExecuteInEditMode]
  public class GlassDropsGenerator : MonoBehaviour
  {
    public Shader shader;
    public Texture inputTexture;
    public RenderTexture heightOutput;
    public RenderTexture normalOutput;
    public float speed = 1f;
    private Material _material;
    private float _phase;
    private bool _isOutputFlat;

    private Material material
    {
      get
      {
        if (_material == null)
          _material = new Material(shader);
        return _material;
      }
      set
      {
        if (!(value != _material))
          return;
        if (_material != null)
          Destroy(_material);
        _material = value;
      }
    }

    private void OnDisable() => material = null;

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      float num = 0.0f;
      if (instance != null && instance.actualRainIntensity > 0.0)
      {
        _phase += Time.deltaTime * speed * instance.actualRainIntensity * instance.actualRainIntensity;
        if (_phase >= 1.0)
          --_phase;
        num = Mathf.Sqrt(instance.actualRainIntensity);
      }
      if (num <= 0.0)
      {
        if (_isOutputFlat)
        {
          if (normalOutput.IsCreated())
            return;
        }
        else
          _isOutputFlat = true;
      }
      else
        _isOutputFlat = false;
      material.SetFloat("_Intensity", num);
      material.SetFloat("_Phase", _phase);
      Graphics.Blit(inputTexture, heightOutput, material, 0);
      Graphics.Blit(heightOutput, normalOutput, material, 1);
    }
  }
}
