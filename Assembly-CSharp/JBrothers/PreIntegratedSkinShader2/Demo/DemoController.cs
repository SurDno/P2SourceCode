// Decompiled with JetBrains decompiler
// Type: JBrothers.PreIntegratedSkinShader2.Demo.DemoController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace JBrothers.PreIntegratedSkinShader2.Demo
{
  public class DemoController : MonoBehaviour
  {
    private int _MainTex;
    public int cubemapResolution = 64;
    private Material materialCopy;
    private Material materialOrig;
    public Renderer meshRenderer;
    private DemoController.SkyboxSphere probeBakedWithSkybox;
    private Quaternion probeBakedWithSunRotation = Quaternion.identity;
    public PreIntegratedSkinProfile[] profiles;
    private Material profileSphereMaterial;
    public Shader profileSphereShader;
    public ReflectionProbe reflectionProbe;
    private DemoController.SkyboxSphere selectedSkybox;
    public Material[] skyboxes;
    private Material skyboxSphereMaterial;
    private DemoController.SkyboxSphere[] skyboxSpheres;
    public Shader skyboxSphereShader;
    public float sphereSize = 64f;
    public Light sun;

    private void Start()
    {
      this._MainTex = Shader.PropertyToID("_MainTex");
      if (!(bool) (UnityEngine.Object) this.skyboxSphereShader)
      {
        Debug.LogWarning((object) "no skybox preview shader");
        this.enabled = false;
      }
      else
      {
        this.skyboxSphereMaterial = new Material(this.skyboxSphereShader);
        if (!(bool) (UnityEngine.Object) this.profileSphereShader)
        {
          Debug.LogWarning((object) "no profile preview shader");
          this.enabled = false;
        }
        else
        {
          this.profileSphereMaterial = new Material(this.profileSphereShader);
          this.profileSphereMaterial.SetTexture("_LookupDirectSM2", (Texture) Resources.Load<Texture2D>("PSSLookupDirectSM2"));
          if (!(bool) (UnityEngine.Object) this.meshRenderer)
          {
            Debug.LogWarning((object) "no mesh renderer");
            this.enabled = false;
          }
          else
          {
            this.materialOrig = this.meshRenderer.sharedMaterial;
            this.materialCopy = this.meshRenderer.material;
            this.skyboxSpheres = new DemoController.SkyboxSphere[this.skyboxes.Length];
            for (int index = 0; index < this.skyboxes.Length; ++index)
            {
              Material skybox = this.skyboxes[index];
              DemoController.SkyboxSphere skyboxSphere = new DemoController.SkyboxSphere();
              if (!(bool) (UnityEngine.Object) skybox)
              {
                Debug.LogWarning((object) "no skybox material specified");
                this.enabled = false;
                return;
              }
              skyboxSphere.skybox = skybox;
              skyboxSphere.cube = this.bakeSkyboxMaterialToCube(this.cubemapResolution, skybox);
              this.skyboxSpheres[index] = skyboxSphere;
            }
            this.SelectSkybox(this.skyboxSpheres[0]);
            this.UpdateRelfectionProbeIfNecessary();
          }
        }
      }
    }

    private void OnDestroy()
    {
      if (this.skyboxSpheres != null)
      {
        foreach (DemoController.SkyboxSphere skyboxSphere in this.skyboxSpheres)
          UnityEngine.Object.Destroy((UnityEngine.Object) skyboxSphere.cube);
      }
      if ((bool) (UnityEngine.Object) this.skyboxSphereMaterial)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.skyboxSphereMaterial);
      if (!(bool) (UnityEngine.Object) this.materialCopy)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.materialCopy);
    }

    private void SelectSkybox(DemoController.SkyboxSphere sb)
    {
      this.selectedSkybox = sb;
      RenderSettings.skybox = sb.skybox;
    }

    private void Update() => this.UpdateRelfectionProbeIfNecessary();

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
      foreach (DemoController.SkyboxSphere skyboxSphere in this.skyboxSpheres)
      {
        Rect rect1 = GUILayoutUtility.GetRect(this.sphereSize, this.sphereSize, GUILayout.ExpandWidth(false));
        Rect rect2 = new Rect(rect1.x, (float) Screen.height - rect1.y - rect1.height, rect1.width, rect1.height);
        bool flag = false;
        if (rect2.Contains(Input.mousePosition))
        {
          float num = (float) ((double) rect1.width * (double) rect1.height / 4.0);
          flag = (double) ((Vector2) Input.mousePosition - rect2.center).sqrMagnitude < (double) num;
        }
        if (Event.current.type == EventType.Repaint)
        {
          float num = Mathf.Repeat(Time.time / 10f, 1f);
          this.skyboxSphereMaterial.SetFloat("_Alpha", flag ? 1f : 0.5f);
          this.skyboxSphereMaterial.SetFloat("_Radius", flag ? 0.5f : 0.4f);
          this.skyboxSphereMaterial.SetTexture("_Cube", (Texture) skyboxSphere.cube);
          this.skyboxSphereMaterial.SetFloat("_Rotation", num);
          Graphics.DrawTexture(rect1, (Texture) Texture2D.whiteTexture, this.skyboxSphereMaterial);
        }
        if (flag)
          GUI.Label(rect1, skyboxSphere.skybox.name, style1);
        if (flag && Input.GetMouseButtonDown(0))
          this.SelectSkybox(skyboxSphere);
      }
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
      foreach (PreIntegratedSkinProfile profile in this.profiles)
      {
        Rect rect3 = GUILayoutUtility.GetRect(this.sphereSize, this.sphereSize, GUILayout.ExpandWidth(false));
        Rect rect4 = new Rect(rect3.x, (float) Screen.height - rect3.y - rect3.height, rect3.width, rect3.height);
        bool flag = false;
        if (rect4.Contains(Input.mousePosition))
        {
          float num = (float) ((double) rect3.width * (double) rect3.height / 4.0);
          flag = (double) ((Vector2) Input.mousePosition - rect4.center).sqrMagnitude < (double) num;
        }
        if (Event.current.type.Equals((object) EventType.Repaint))
        {
          float num = Mathf.Repeat(Time.time / 10f, 1f);
          this.profileSphereMaterial.SetFloat("_Alpha", flag ? 1f : 0.5f);
          this.profileSphereMaterial.SetFloat("_Radius", flag ? 0.5f : 0.4f);
          this.profileSphereMaterial.SetFloat("_Rotation", num);
          profile.ApplyProfile(this.profileSphereMaterial);
          Graphics.DrawTexture(rect3, (Texture) Texture2D.whiteTexture, this.profileSphereMaterial);
        }
        if (flag)
          GUI.Label(rect3, profile.name, style1);
        if (flag && Input.GetMouseButtonDown(0))
          profile.ApplyProfile(this.materialCopy);
      }
      GUILayout.EndHorizontal();
      this.sun.enabled = GUILayout.Toggle(this.sun.enabled, "Direct light");
      GUILayout.BeginVertical();
      GUILayout.Label("Ambient intensity", style2);
      RenderSettings.ambientIntensity = GUILayout.HorizontalSlider(RenderSettings.ambientIntensity, 0.0f, 2f);
      GUILayout.EndVertical();
      GUILayout.BeginVertical();
      GUILayout.Label("Reflection intensity", style2);
      this.reflectionProbe.intensity = GUILayout.HorizontalSlider(this.reflectionProbe.intensity, 0.0f, 2f);
      GUILayout.EndVertical();
      bool flag1 = this.materialCopy.GetTexture(this._MainTex) != Texture2D.whiteTexture;
      bool flag2 = GUILayout.Toggle(flag1, "Use diffuse texture");
      if (flag2 != flag1)
      {
        if (flag2)
          this.materialCopy.SetTexture(this._MainTex, this.materialOrig.GetTexture(this._MainTex));
        else
          this.materialCopy.SetTexture(this._MainTex, (Texture) Texture2D.whiteTexture);
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
      if (!(bool) (UnityEngine.Object) this.sun)
        return;
      bool flag = false;
      if (this.probeBakedWithSkybox != this.selectedSkybox)
      {
        flag = true;
        this.probeBakedWithSkybox = this.selectedSkybox;
      }
      if (this.sun.transform.rotation != this.probeBakedWithSunRotation)
      {
        flag = true;
        this.probeBakedWithSunRotation = this.sun.transform.rotation;
      }
      if (flag && this.reflectionProbe.isActiveAndEnabled)
        this.reflectionProbe.RenderProbe();
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
