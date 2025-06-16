// Decompiled with JetBrains decompiler
// Type: LightFlicker2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LightFlicker2 : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  private static int propertyId = 0;
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
    this.light = this.GetComponent<Light>();
    if ((Object) this.light != (Object) null)
      this.baseIntensity = this.light.intensity;
    if ((Object) this.additionalLight != (Object) null)
      this.baseAdditionalIntensity = this.additionalLight.intensity;
    if ((Object) this.bulbObject != (Object) null)
      this.bulbRenderer = this.bulbObject.GetComponent<Renderer>();
    if (!((Object) this.bulbRenderer != (Object) null))
      return;
    if (LightFlicker2.propertyId == 0)
      LightFlicker2.propertyId = Shader.PropertyToID("_EmissionColor");
    Material[] sharedMaterials = this.bulbRenderer.sharedMaterials;
    if (sharedMaterials.Length != 0)
      this.baseEmissionColors = new Color[sharedMaterials.Length];
    for (int index = 0; index < sharedMaterials.Length; ++index)
    {
      if ((Object) sharedMaterials[index] != (Object) null)
        this.baseEmissionColors[index] = sharedMaterials[index].GetColor(LightFlicker2.propertyId);
    }
  }

  private void Update()
  {
    if ((double) this.flickerFrequency <= 0.0 || (double) Time.time < (double) (this.lastChangeTime + 1f / this.flickerFrequency))
      return;
    this.lastChangeTime = Time.time;
    float num = (float) (1.0 - (double) Mathf.PerlinNoise(Random.Range(0.0f, 1000f), Time.time) * (double) this.flickerIntensity);
    if ((Object) this.light != (Object) null)
      this.light.intensity = this.baseIntensity * num;
    if ((Object) this.additionalLight != (Object) null)
      this.additionalLight.intensity = this.baseAdditionalIntensity * num;
    if (this.baseEmissionColors == null)
      return;
    if (LightFlicker2.propertyBlock == null)
      LightFlicker2.propertyBlock = new MaterialPropertyBlock();
    for (int materialIndex = 0; materialIndex < this.baseEmissionColors.Length; ++materialIndex)
    {
      LightFlicker2.propertyBlock.SetColor(LightFlicker2.propertyId, this.baseEmissionColors[materialIndex] * num);
      this.bulbRenderer.SetPropertyBlock(LightFlicker2.propertyBlock, materialIndex);
    }
  }

  private void OnDisable()
  {
    if ((Object) this.light != (Object) null)
      this.light.intensity = this.baseIntensity;
    if ((Object) this.additionalLight != (Object) null)
      this.additionalLight.intensity = this.baseAdditionalIntensity;
    if (this.baseEmissionColors == null)
      return;
    for (int materialIndex = 0; materialIndex < this.baseEmissionColors.Length; ++materialIndex)
      this.bulbRenderer.SetPropertyBlock((MaterialPropertyBlock) null, materialIndex);
  }
}
