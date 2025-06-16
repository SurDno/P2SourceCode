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
    private float _windJitterTimeLeft;
    private bool _isOutputFlat;

    private Material material
    {
      get
      {
        if ((Object) _material == (Object) null)
        {
          _material = new Material(shader);
          _material.SetTexture("_WindTex", wavesTex);
        }
        return _material;
      }
      set
      {
        if (!((Object) value != (Object) _material))
          return;
        if ((Object) _material != (Object) null)
          Object.Destroy((Object) _material);
        _material = value;
      }
    }

    private void OnDisable() => material = (Material) null;

    private void Update()
    {
      RainManager instance = RainManager.Instance;
      _windJitterTimeLeft -= Time.deltaTime;
      float num;
      if ((Object) instance == (Object) null)
      {
        num = 0.0f;
        if (_windJitterTimeLeft <= 0.0)
        {
          _windJitterTimeLeft = 2f;
          _targetWindJitter = Random.insideUnitCircle * 0.25f;
        }
      }
      else
      {
        num = instance.actualRainIntensity;
        if (_windJitterTimeLeft <= 0.0)
        {
          _windJitterTimeLeft = 2f;
          _targetWindJitter = instance.actualWindVector + Random.insideUnitCircle * 0.25f;
        }
      }
      _jitteredWindVector = Vector2.SmoothDamp(_jitteredWindVector, _targetWindJitter, ref _windJitterVelocity, 0.5f, float.PositiveInfinity, Time.deltaTime);
      Vector2 vector2 = Vector2.MoveTowards(_jitteredWindVector, Vector2.zero, 0.25f);
      if (num <= 0.0 && vector2 == Vector2.zero)
      {
        if (_isOutputFlat)
        {
          if (outputTexture.IsCreated())
            return;
        }
        else
          _isOutputFlat = true;
      }
      else
        _isOutputFlat = false;
      material.SetFloat("_RainIntensity", num);
      material.SetVector("_WindVector", new Vector4(vector2.x, vector2.y, 0.0f, 0.0f));
      Graphics.Blit(rippleTex, outputTexture, material);
    }
  }
}
