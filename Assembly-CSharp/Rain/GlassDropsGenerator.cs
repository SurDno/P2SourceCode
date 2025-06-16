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
    private float _phase = 0.0f;
    private bool _isOutputFlat = false;

    private Material material
    {
      get
      {
        if ((Object) this._material == (Object) null)
          this._material = new Material(this.shader);
        return this._material;
      }
      set
      {
        if (!((Object) value != (Object) this._material))
          return;
        if ((Object) this._material != (Object) null)
          Object.Destroy((Object) this._material);
        this._material = value;
      }
    }

    private void OnDisable() => this.material = (Material) null;

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      float num = 0.0f;
      if ((Object) instance != (Object) null && (double) instance.actualRainIntensity > 0.0)
      {
        this._phase += Time.deltaTime * this.speed * instance.actualRainIntensity * instance.actualRainIntensity;
        if ((double) this._phase >= 1.0)
          --this._phase;
        num = Mathf.Sqrt(instance.actualRainIntensity);
      }
      if ((double) num <= 0.0)
      {
        if (this._isOutputFlat)
        {
          if (this.normalOutput.IsCreated())
            return;
        }
        else
          this._isOutputFlat = true;
      }
      else
        this._isOutputFlat = false;
      this.material.SetFloat("_Intensity", num);
      this.material.SetFloat("_Phase", this._phase);
      Graphics.Blit(this.inputTexture, this.heightOutput, this.material, 0);
      Graphics.Blit((Texture) this.heightOutput, this.normalOutput, this.material, 1);
    }
  }
}
