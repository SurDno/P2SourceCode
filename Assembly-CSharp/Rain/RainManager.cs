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

    public Vector3 PlayerPosition => playerCamera != null ? playerCamera.transform.position : playerPosition;

    private void Awake() => Instance = this;

    private void OnDestroy() => Instance = null;

    private void Update()
    {
      actualRainIntensity = Mathf.MoveTowards(actualRainIntensity, rainIntensity, Time.deltaTime * 1f);
      if (terrainFillTime > 0.0)
        terrainWetness += actualRainIntensity * Time.deltaTime / terrainFillTime;
      if (terrainDryTime > 0.0)
        terrainWetness -= Time.deltaTime / terrainDryTime;
      terrainWetness = Mathf.Clamp01(terrainWetness);
      if (puddleFillTime > 0.0)
        puddleWetness += actualRainIntensity * Time.deltaTime / puddleFillTime;
      if (puddleDryTime > 0.0)
        puddleWetness -= Time.deltaTime / puddleDryTime;
      puddleWetness = Mathf.Clamp01(puddleWetness);
      actualWindVector = Vector2.MoveTowards(actualWindVector, windVector, Time.deltaTime * 0.2f);
      Shader.SetGlobalFloat("_Terrain_Wetness", terrainWetness);
      Shader.SetGlobalFloat("_PuddleWetness", puddleWetness);
      float num = terrainWetness * (float) (0.5 + actualRainIntensity * 0.5);
      Shader.SetGlobalFloat("_RainFlows", num * num);
      Shader.SetGlobalColor("_BlurBehindFallback", blurBehindFallback);
    }
  }
}
