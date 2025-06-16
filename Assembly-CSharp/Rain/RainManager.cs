using UnityEngine;

namespace Rain
{
  [ExecuteInEditMode]
  public class RainManager : MonoBehaviour
  {
    private const float RainChangingSpeed = 1f;
    private const float WindChangingSpeed = 0.2f;
    [Header("Controls")]
    [Range(0.0f, 1f)]
    public float rainIntensity;
    public float terrainDryTime;
    public float puddleDryTime;
    public Vector2 windVector = Vector2.zero;
    public Color blurBehindFallback = Color.gray;
    public Vector3 playerPosition;
    [Header("State")]
    [Range(0.0f, 1f)]
    public float actualRainIntensity;
    [Range(0.0f, 1f)]
    public float terrainWetness;
    [Range(0.0f, 1f)]
    public float puddleWetness;
    public Vector2 actualWindVector;
    [Header("Setup")]
    public Camera playerCamera;
    [Tooltip("Слои с коллайдерами, об которые должны разбиваться капли")]
    public LayerMask rainColliders;
    [Tooltip("Слои с лужами, для поиска попаданий")]
    public LayerMask puddleLayers;
    public float terrainFillTime = 10f;
    public float puddleFillTime = 60f;

    public static RainManager Instance { get; private set; }

    public Vector3 PlayerPosition
    {
      get
      {
        return (Object) this.playerCamera != (Object) null ? this.playerCamera.transform.position : this.playerPosition;
      }
    }

    private void Awake() => RainManager.Instance = this;

    private void OnDestroy() => RainManager.Instance = (RainManager) null;

    private void Update()
    {
      this.actualRainIntensity = Mathf.MoveTowards(this.actualRainIntensity, this.rainIntensity, Time.deltaTime * 1f);
      if ((double) this.terrainFillTime > 0.0)
        this.terrainWetness += this.actualRainIntensity * Time.deltaTime / this.terrainFillTime;
      if ((double) this.terrainDryTime > 0.0)
        this.terrainWetness -= Time.deltaTime / this.terrainDryTime;
      this.terrainWetness = Mathf.Clamp01(this.terrainWetness);
      if ((double) this.puddleFillTime > 0.0)
        this.puddleWetness += this.actualRainIntensity * Time.deltaTime / this.puddleFillTime;
      if ((double) this.puddleDryTime > 0.0)
        this.puddleWetness -= Time.deltaTime / this.puddleDryTime;
      this.puddleWetness = Mathf.Clamp01(this.puddleWetness);
      this.actualWindVector = Vector2.MoveTowards(this.actualWindVector, this.windVector, Time.deltaTime * 0.2f);
      Shader.SetGlobalFloat("_Terrain_Wetness", this.terrainWetness);
      Shader.SetGlobalFloat("_PuddleWetness", this.puddleWetness);
      float num = this.terrainWetness * (float) (0.5 + (double) this.actualRainIntensity * 0.5);
      Shader.SetGlobalFloat("_RainFlows", num * num);
      Shader.SetGlobalColor("_BlurBehindFallback", this.blurBehindFallback);
    }
  }
}
