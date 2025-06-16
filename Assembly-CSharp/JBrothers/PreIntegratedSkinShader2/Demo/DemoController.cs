using System;

namespace JBrothers.PreIntegratedSkinShader2.Demo
{
  public class DemoController : MonoBehaviour
  {
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

    private void Start()
    {
      _MainTex = Shader.PropertyToID("_MainTex");
      if (!(bool) (UnityEngine.Object) skyboxSphereShader)
      {
        Debug.LogWarning((object) "no skybox preview shader");
        this.enabled = false;
      }
      else
      {
        skyboxSphereMaterial = new Material(skyboxSphereShader);
        if (!(bool) (UnityEngine.Object) profileSphereShader)
        {
          Debug.LogWarning((object) "no profile preview shader");
          this.enabled = false;
        }
        else
        {
          profileSphereMaterial = new Material(profileSphereShader);
          profileSphereMaterial.SetTexture("_LookupDirectSM2", (Texture) Resources.Load<Texture2D>("PSSLookupDirectSM2"));
          if (!(bool) (UnityEngine.Object) meshRenderer)
          {
            Debug.LogWarning((object) "no mesh renderer");
            this.enabled = false;
          }
          else
          {
            materialOrig = meshRenderer.sharedMaterial;
            materialCopy = meshRenderer.material;
            skyboxSpheres = new SkyboxSphere[skyboxes.Length];
            for (int index = 0; index < skyboxes.Length; ++index)
            {
              Material skybox = skyboxes[index];
              SkyboxSphere skyboxSphere = new SkyboxSphere();
              if (!(bool) (UnityEngine.Object) skybox)
              {
                Debug.LogWarning((object) "no skybox material specified");
                this.enabled = false;
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

    private void OnDestroy()
    {
      if (skyboxSpheres != null)
      {
        foreach (SkyboxSphere skyboxSphere in skyboxSpheres)
          UnityEngine.Object.Destroy((UnityEngine.Object) skyboxSphere.cube);
      }
      if ((bool) (UnityEngine.Object) skyboxSphereMaterial)
        UnityEngine.Object.Destroy((UnityEngine.Object) skyboxSphereMaterial);
      if (!(bool) (UnityEngine.Object) materialCopy)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) materialCopy);
    }

    private void SelectSkybox(SkyboxSphere sb)
    {
      selectedSkybox = sb;
      RenderSettings.skybox = sb.skybox;
    }

    private void Update() => UpdateRelfectionProbeIfNecessary();

    private void OnGUI()
    {
      GUIStyle style1 = new GUIStyle((GUIStyle) "label");
      style1.alignment = TextAnchor.MiddleCenter;
      style1.fontStyle = FontStyle.Bold;
      GUIStyle style2 = new GUIStyle((GUIStyle) "label");
      style2.alignment = TextAnchor.UpperLeft;
      style2.fontStyle = FontStyle.Normal;
      int controlId = GUIUtility.GetControlID(FocusType.Passive);
      GUILayout.BeginVertical(GUI.skin.box);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
      foreach (SkyboxSphere skyboxSphere in skyboxSpheres)
      {
        Rect rect1 = GUILayoutUtility.GetRect(sphereSize, sphereSize, GUILayout.ExpandWidth(false));
        Rect rect2 = new Rect(rect1.x, (float) Screen.height - rect1.y - rect1.height, rect1.width, rect1.height);
        bool flag = false;
        if (rect2.Contains(Input.mousePosition))
        {
          float num = (float) ((double) rect1.width * (double) rect1.height / 4.0);
          flag = (double) ((Vector2) Input.mousePosition - rect2.center).sqrMagnitude < num;
        }
        if (Event.current.type == EventType.Repaint)
        {
          float num = Mathf.Repeat(Time.time / 10f, 1f);
          skyboxSphereMaterial.SetFloat("_Alpha", flag ? 1f : 0.5f);
          skyboxSphereMaterial.SetFloat("_Radius", flag ? 0.5f : 0.4f);
          skyboxSphereMaterial.SetTexture("_Cube", (Texture) skyboxSphere.cube);
          skyboxSphereMaterial.SetFloat("_Rotation", num);
          Graphics.DrawTexture(rect1, (Texture) Texture2D.whiteTexture, skyboxSphereMaterial);
        }
        if (flag)
          GUI.Label(rect1, skyboxSphere.skybox.name, style1);
        if (flag && Input.GetMouseButtonDown(0))
          SelectSkybox(skyboxSphere);
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
      foreach (PreIntegratedSkinProfile profile in profiles)
      {
        Rect rect3 = GUILayoutUtility.GetRect(sphereSize, sphereSize, GUILayout.ExpandWidth(false));
        Rect rect4 = new Rect(rect3.x, (float) Screen.height - rect3.y - rect3.height, rect3.width, rect3.height);
        bool flag = false;
        if (rect4.Contains(Input.mousePosition))
        {
          float num = (float) ((double) rect3.width * (double) rect3.height / 4.0);
          flag = (double) ((Vector2) Input.mousePosition - rect4.center).sqrMagnitude < num;
        }
        if (Event.current.type.Equals((object) EventType.Repaint))
        {
          float num = Mathf.Repeat(Time.time / 10f, 1f);
          profileSphereMaterial.SetFloat("_Alpha", flag ? 1f : 0.5f);
          profileSphereMaterial.SetFloat("_Radius", flag ? 0.5f : 0.4f);
          profileSphereMaterial.SetFloat("_Rotation", num);
          profile.ApplyProfile(profileSphereMaterial);
          Graphics.DrawTexture(rect3, (Texture) Texture2D.whiteTexture, profileSphereMaterial);
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
      bool flag1 = materialCopy.GetTexture(_MainTex) != Texture2D.whiteTexture;
      bool flag2 = GUILayout.Toggle(flag1, "Use diffuse texture");
      if (flag2 != flag1)
      {
        if (flag2)
          materialCopy.SetTexture(_MainTex, materialOrig.GetTexture(_MainTex));
        else
          materialCopy.SetTexture(_MainTex, (Texture) Texture2D.whiteTexture);
      }
      GUILayout.EndVertical();
      Rect lastRect = GUILayoutUtility.GetLastRect();
      switch (Event.current.GetTypeForControl(controlId))
      {
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

    private void UpdateRelfectionProbeIfNecessary()
    {
      if (!(bool) (UnityEngine.Object) sun)
        return;
      bool flag = false;
      if (probeBakedWithSkybox != selectedSkybox)
      {
        flag = true;
        probeBakedWithSkybox = selectedSkybox;
      }
      if (sun.transform.rotation != probeBakedWithSunRotation)
      {
        flag = true;
        probeBakedWithSunRotation = sun.transform.rotation;
      }
      if (flag && reflectionProbe.isActiveAndEnabled)
        reflectionProbe.RenderProbe();
    }

    private Cubemap bakeSkyboxMaterialToCube(int size, Material skybox)
    {
      GameObject gameObject = new GameObject();
      try
      {
        gameObject.SetActive(false);
        gameObject.AddComponent<Skybox>().material = skybox;
        Cubemap cubemap = new Cubemap(size, TextureFormat.RGB24, false);
        Camera camera = gameObject.AddComponent<Camera>();
        camera.enabled = false;
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.renderingPath = RenderingPath.Forward;
        camera.cullingMask = 0;
        camera.RenderToCubemap(cubemap);
        cubemap.Apply(false, true);
        return cubemap;
      }
      finally
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
      }
    }

    [Serializable]
    private class SkyboxSphere
    {
      public Cubemap cube;
      public Material skybox;
    }
  }
}
