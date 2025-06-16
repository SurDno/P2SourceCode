using UnityEngine;

public class LightFlicker2 : MonoBehaviour {
	private static MaterialPropertyBlock propertyBlock;
	private static int propertyId;
	[SerializeField] private Light additionalLight;
	[SerializeField] private GameObject bulbObject;
	[SerializeField] private float flickerIntensity = 0.5f;
	[SerializeField] private float flickerFrequency = float.PositiveInfinity;
	private Light light;
	private float baseIntensity;
	private float baseAdditionalIntensity;
	private Renderer bulbRenderer;
	private Color[] baseEmissionColors;
	private float lastChangeTime = float.MinValue;

	private void Awake() {
		light = GetComponent<Light>();
		if (light != null)
			baseIntensity = light.intensity;
		if (additionalLight != null)
			baseAdditionalIntensity = additionalLight.intensity;
		if (bulbObject != null)
			bulbRenderer = bulbObject.GetComponent<Renderer>();
		if (!(bulbRenderer != null))
			return;
		if (propertyId == 0)
			propertyId = Shader.PropertyToID("_EmissionColor");
		var sharedMaterials = bulbRenderer.sharedMaterials;
		if (sharedMaterials.Length != 0)
			baseEmissionColors = new Color[sharedMaterials.Length];
		for (var index = 0; index < sharedMaterials.Length; ++index)
			if (sharedMaterials[index] != null)
				baseEmissionColors[index] = sharedMaterials[index].GetColor(propertyId);
	}

	private void Update() {
		if (flickerFrequency <= 0.0 || Time.time < (double)(lastChangeTime + 1f / flickerFrequency))
			return;
		lastChangeTime = Time.time;
		var num = (float)(1.0 - Mathf.PerlinNoise(Random.Range(0.0f, 1000f), Time.time) * (double)flickerIntensity);
		if (light != null)
			light.intensity = baseIntensity * num;
		if (additionalLight != null)
			additionalLight.intensity = baseAdditionalIntensity * num;
		if (baseEmissionColors == null)
			return;
		if (propertyBlock == null)
			propertyBlock = new MaterialPropertyBlock();
		for (var materialIndex = 0; materialIndex < baseEmissionColors.Length; ++materialIndex) {
			propertyBlock.SetColor(propertyId, baseEmissionColors[materialIndex] * num);
			bulbRenderer.SetPropertyBlock(propertyBlock, materialIndex);
		}
	}

	private void OnDisable() {
		if (light != null)
			light.intensity = baseIntensity;
		if (additionalLight != null)
			additionalLight.intensity = baseAdditionalIntensity;
		if (baseEmissionColors == null)
			return;
		for (var materialIndex = 0; materialIndex < baseEmissionColors.Length; ++materialIndex)
			bulbRenderer.SetPropertyBlock(null, materialIndex);
	}
}