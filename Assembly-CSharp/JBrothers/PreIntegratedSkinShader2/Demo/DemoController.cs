using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JBrothers.PreIntegratedSkinShader2.Demo;

public class DemoController : MonoBehaviour {
	private int _MainTex;
	public int cubemapResolution = 64;
	private Material materialCopy;
	private Material materialOrig;
	public Renderer meshRenderer;
	private SkyboxSphere probeBakedWithSkybox;
	private Quaternion probeBakedWithSunRotation = Quaternion.identity;
	public PreIntegratedSkinProfile[] profiles;
	private Material profileSphereMaterial;
	public Shader profileSphereShader;
	public ReflectionProbe reflectionProbe;
	private SkyboxSphere selectedSkybox;
	public Material[] skyboxes;
	private Material skyboxSphereMaterial;
	private SkyboxSphere[] skyboxSpheres;
	public Shader skyboxSphereShader;
	public float sphereSize = 64f;
	public Light sun;

	private void Start() {
		_MainTex = Shader.PropertyToID("_MainTex");
		if (!(bool)(Object)skyboxSphereShader) {
			Debug.LogWarning("no skybox preview shader");
			enabled = false;
		} else {
			skyboxSphereMaterial = new Material(skyboxSphereShader);
			if (!(bool)(Object)profileSphereShader) {
				Debug.LogWarning("no profile preview shader");
				enabled = false;
			} else {
				profileSphereMaterial = new Material(profileSphereShader);
				profileSphereMaterial.SetTexture("_LookupDirectSM2", Resources.Load<Texture2D>("PSSLookupDirectSM2"));
				if (!(bool)(Object)meshRenderer) {
					Debug.LogWarning("no mesh renderer");
					enabled = false;
				} else {
					materialOrig = meshRenderer.sharedMaterial;
					materialCopy = meshRenderer.material;
					skyboxSpheres = new SkyboxSphere[skyboxes.Length];
					for (var index = 0; index < skyboxes.Length; ++index) {
						var skybox = skyboxes[index];
						var skyboxSphere = new SkyboxSphere();
						if (!(bool)(Object)skybox) {
							Debug.LogWarning("no skybox material specified");
							enabled = false;
							return;
						}

						skyboxSphere.skybox = skybox;
						skyboxSphere.cube = bakeSkyboxMaterialToCube(cubemapResolution, skybox);
						skyboxSpheres[index] = skyboxSphere;
					}

					SelectSkybox(skyboxSpheres[0]);
					UpdateRelfectionProbeIfNecessary();
				}
			}
		}
	}

	private void OnDestroy() {
		if (skyboxSpheres != null)
			foreach (var skyboxSphere in skyboxSpheres)
				Destroy(skyboxSphere.cube);
		if ((bool)(Object)skyboxSphereMaterial)
			Destroy(skyboxSphereMaterial);
		if (!(bool)(Object)materialCopy)
			return;
		Destroy(materialCopy);
	}

	private void SelectSkybox(SkyboxSphere sb) {
		selectedSkybox = sb;
		RenderSettings.skybox = sb.skybox;
	}

	private void Update() {
		UpdateRelfectionProbeIfNecessary();
	}

	private void OnGUI() {
		var style1 = new GUIStyle("label");
		style1.alignment = TextAnchor.MiddleCenter;
		style1.fontStyle = FontStyle.Bold;
		var style2 = new GUIStyle("label");
		style2.alignment = TextAnchor.UpperLeft;
		style2.fontStyle = FontStyle.Normal;
		var controlId = GUIUtility.GetControlID(FocusType.Passive);
		GUILayout.BeginVertical(GUI.skin.box);
		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
		foreach (var skyboxSphere in skyboxSpheres) {
			var rect1 = GUILayoutUtility.GetRect(sphereSize, sphereSize, GUILayout.ExpandWidth(false));
			var rect2 = new Rect(rect1.x, Screen.height - rect1.y - rect1.height, rect1.width, rect1.height);
			var flag = false;
			if (rect2.Contains(Input.mousePosition)) {
				var num = (float)(rect1.width * (double)rect1.height / 4.0);
				flag = ((Vector2)Input.mousePosition - rect2.center).sqrMagnitude < (double)num;
			}

			if (Event.current.type == EventType.Repaint) {
				var num = Mathf.Repeat(Time.time / 10f, 1f);
				skyboxSphereMaterial.SetFloat("_Alpha", flag ? 1f : 0.5f);
				skyboxSphereMaterial.SetFloat("_Radius", flag ? 0.5f : 0.4f);
				skyboxSphereMaterial.SetTexture("_Cube", skyboxSphere.cube);
				skyboxSphereMaterial.SetFloat("_Rotation", num);
				Graphics.DrawTexture(rect1, Texture2D.whiteTexture, skyboxSphereMaterial);
			}

			if (flag)
				GUI.Label(rect1, skyboxSphere.skybox.name, style1);
			if (flag && Input.GetMouseButtonDown(0))
				SelectSkybox(skyboxSphere);
		}

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
		foreach (var profile in profiles) {
			var rect3 = GUILayoutUtility.GetRect(sphereSize, sphereSize, GUILayout.ExpandWidth(false));
			var rect4 = new Rect(rect3.x, Screen.height - rect3.y - rect3.height, rect3.width, rect3.height);
			var flag = false;
			if (rect4.Contains(Input.mousePosition)) {
				var num = (float)(rect3.width * (double)rect3.height / 4.0);
				flag = ((Vector2)Input.mousePosition - rect4.center).sqrMagnitude < (double)num;
			}

			if (Event.current.type.Equals(EventType.Repaint)) {
				var num = Mathf.Repeat(Time.time / 10f, 1f);
				profileSphereMaterial.SetFloat("_Alpha", flag ? 1f : 0.5f);
				profileSphereMaterial.SetFloat("_Radius", flag ? 0.5f : 0.4f);
				profileSphereMaterial.SetFloat("_Rotation", num);
				profile.ApplyProfile(profileSphereMaterial);
				Graphics.DrawTexture(rect3, Texture2D.whiteTexture, profileSphereMaterial);
			}

			if (flag)
				GUI.Label(rect3, profile.name, style1);
			if (flag && Input.GetMouseButtonDown(0))
				profile.ApplyProfile(materialCopy);
		}

		GUILayout.EndHorizontal();
		sun.enabled = GUILayout.Toggle(sun.enabled, "Direct light");
		GUILayout.BeginVertical();
		GUILayout.Label("Ambient intensity", style2);
		RenderSettings.ambientIntensity = GUILayout.HorizontalSlider(RenderSettings.ambientIntensity, 0.0f, 2f);
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.Label("Reflection intensity", style2);
		reflectionProbe.intensity = GUILayout.HorizontalSlider(reflectionProbe.intensity, 0.0f, 2f);
		GUILayout.EndVertical();
		var flag1 = materialCopy.GetTexture(_MainTex) != Texture2D.whiteTexture;
		var flag2 = GUILayout.Toggle(flag1, "Use diffuse texture");
		if (flag2 != flag1) {
			if (flag2)
				materialCopy.SetTexture(_MainTex, materialOrig.GetTexture(_MainTex));
			else
				materialCopy.SetTexture(_MainTex, Texture2D.whiteTexture);
		}

		GUILayout.EndVertical();
		var lastRect = GUILayoutUtility.GetLastRect();
		switch (Event.current.GetTypeForControl(controlId)) {
			case EventType.MouseDown:
				if (!lastRect.Contains(Event.current.mousePosition))
					break;
				GUIUtility.hotControl = controlId;
				Event.current.Use();
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl != controlId)
					break;
				GUIUtility.hotControl = 0;
				Event.current.Use();
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl != controlId)
					break;
				Event.current.Use();
				break;
			case EventType.ScrollWheel:
				if (!lastRect.Contains(Event.current.mousePosition))
					break;
				Event.current.Use();
				break;
		}
	}

	private void UpdateRelfectionProbeIfNecessary() {
		if (!(bool)(Object)sun)
			return;
		var flag = false;
		if (probeBakedWithSkybox != selectedSkybox) {
			flag = true;
			probeBakedWithSkybox = selectedSkybox;
		}

		if (sun.transform.rotation != probeBakedWithSunRotation) {
			flag = true;
			probeBakedWithSunRotation = sun.transform.rotation;
		}

		if (flag && reflectionProbe.isActiveAndEnabled)
			reflectionProbe.RenderProbe();
	}

	private Cubemap bakeSkyboxMaterialToCube(int size, Material skybox) {
		var gameObject = new GameObject();
		try {
			gameObject.SetActive(false);
			gameObject.AddComponent<Skybox>().material = skybox;
			var cubemap = new Cubemap(size, TextureFormat.RGB24, false);
			var camera = gameObject.AddComponent<Camera>();
			camera.enabled = false;
			camera.clearFlags = CameraClearFlags.Skybox;
			camera.renderingPath = RenderingPath.Forward;
			camera.cullingMask = 0;
			camera.RenderToCubemap(cubemap);
			cubemap.Apply(false, true);
			return cubemap;
		} finally {
			Destroy(gameObject);
		}
	}

	[Serializable]
	private class SkyboxSphere {
		public Cubemap cube;
		public Material skybox;
	}
}