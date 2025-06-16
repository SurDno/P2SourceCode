using UnityEngine;

namespace Rain
{
  [ExecuteInEditMode]
  public class RippleGenerator : MonoBehaviour
  {
    private const float WindJitterTime = 2f;
    private const float WindJitterRadius = 0.25f;
    private const float WindThreshold = 0.25f;
    private const float WindJitterSmoothness = 0.5f;
    public Shader shader;
    public Texture rippleTex;
    public Texture wavesTex;
    public RenderTexture outputTexture;
    private Material _material;
    private Vector2 _jitteredWindVector = Vector2.zero;
    private Vector2 _windJitterVelocity = Vector2.zero;
    private Vector2 _targetWindJitter = Vector2.zero;
    private float _windJitterTimeLeft = 0.0f;
    private bool _isOutputFlat = false;

    private Material material
    {
      get
      {
        if ((Object) this._material == (Object) null)
        {
          this._material = new Material(this.shader);
          this._material.SetTexture("_WindTex", this.wavesTex);
        }
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
      this._windJitterTimeLeft -= Time.deltaTime;
      float num;
      if ((Object) instance == (Object) null)
      {
        num = 0.0f;
        if ((double) this._windJitterTimeLeft <= 0.0)
        {
          this._windJitterTimeLeft = 2f;
          this._targetWindJitter = Random.insideUnitCircle * 0.25f;
        }
      }
      else
      {
        num = instance.actualRainIntensity;
        if ((double) this._windJitterTimeLeft <= 0.0)
        {
          this._windJitterTimeLeft = 2f;
          this._targetWindJitter = instance.actualWindVector + Random.insideUnitCircle * 0.25f;
        }
      }
      this._jitteredWindVector = Vector2.SmoothDamp(this._jitteredWindVector, this._targetWindJitter, ref this._windJitterVelocity, 0.5f, float.PositiveInfinity, Time.deltaTime);
      Vector2 vector2 = Vector2.MoveTowards(this._jitteredWindVector, Vector2.zero, 0.25f);
      if ((double) num <= 0.0 && vector2 == Vector2.zero)
      {
        if (this._isOutputFlat)
        {
          if (this.outputTexture.IsCreated())
            return;
        }
        else
          this._isOutputFlat = true;
      }
      else
        this._isOutputFlat = false;
      this.material.SetFloat("_RainIntensity", num);
      this.material.SetVector("_WindVector", new Vector4(vector2.x, vector2.y, 0.0f, 0.0f));
      Graphics.Blit(this.rippleTex, this.outputTexture, this.material);
    }
  }
}
