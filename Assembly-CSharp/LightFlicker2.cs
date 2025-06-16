public class LightFlicker2 : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  private static int propertyId;
  [SerializeField]
  private Light additionalLight;
  [SerializeField]
  private GameObject bulbObject;
  [SerializeField]
  private float flickerIntensity = 0.5f;
  [SerializeField]
  private float flickerFrequency = float.PositiveInfinity;
  private Light light;
  private float baseIntensity;
  private float baseAdditionalIntensity;
  private Renderer bulbRenderer;
  private Color[] baseEmissionColors;
  private float lastChangeTime = float.MinValue;

  private void Awake()
  {
    light = this.GetComponent<Light>();
    if ((Object) light != (Object) null)
      baseIntensity = light.intensity;
    if ((Object) additionalLight != (Object) null)
      baseAdditionalIntensity = additionalLight.intensity;
    if ((Object) bulbObject != (Object) null)
      bulbRenderer = bulbObject.GetComponent<Renderer>();
    if (!((Object) bulbRenderer != (Object) null))
      return;
    if (propertyId == 0)
      propertyId = Shader.PropertyToID("_EmissionColor");
    Material[] sharedMaterials = bulbRenderer.sharedMaterials;
    if (sharedMaterials.Length != 0)
      baseEmissionColors = new Color[sharedMaterials.Length];
    for (int index = 0; index < sharedMaterials.Length; ++index)
    {
      if ((Object) sharedMaterials[index] != (Object) null)
        baseEmissionColors[index] = sharedMaterials[index].GetColor(propertyId);
    }
  }

  private void Update()
  {
    if (flickerFrequency <= 0.0 || (double) Time.time < lastChangeTime + 1f / flickerFrequency)
      return;
    lastChangeTime = Time.time;
    float num = (float) (1.0 - (double) Mathf.PerlinNoise(Random.Range(0.0f, 1000f), Time.time) * flickerIntensity);
    if ((Object) light != (Object) null)
      light.intensity = baseIntensity * num;
    if ((Object) additionalLight != (Object) null)
      additionalLight.intensity = baseAdditionalIntensity * num;
    if (baseEmissionColors == null)
      return;
    if (propertyBlock == null)
      propertyBlock = new MaterialPropertyBlock();
    for (int materialIndex = 0; materialIndex < baseEmissionColors.Length; ++materialIndex)
    {
      propertyBlock.SetColor(propertyId, baseEmissionColors[materialIndex] * num);
      bulbRenderer.SetPropertyBlock(propertyBlock, materialIndex);
    }
  }

  private void OnDisable()
  {
    if ((Object) light != (Object) null)
      light.intensity = baseIntensity;
    if ((Object) additionalLight != (Object) null)
      additionalLight.intensity = baseAdditionalIntensity;
    if (baseEmissionColors == null)
      return;
    for (int materialIndex = 0; materialIndex < baseEmissionColors.Length; ++materialIndex)
      bulbRenderer.SetPropertyBlock((MaterialPropertyBlock) null, materialIndex);
  }
}
