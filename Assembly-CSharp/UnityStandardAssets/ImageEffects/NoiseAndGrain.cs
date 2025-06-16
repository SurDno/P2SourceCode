// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.NoiseAndGrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Noise/Noise And Grain (Filmic)")]
  public class NoiseAndGrain : PostEffectsBase
  {
    public float intensityMultiplier = 0.25f;
    public float generalIntensity = 0.5f;
    public float blackIntensity = 1f;
    public float whiteIntensity = 1f;
    public float midGrey = 0.2f;
    public bool dx11Grain = false;
    public float softness = 0.0f;
    public bool monochrome = false;
    public Vector3 intensities = new Vector3(1f, 1f, 1f);
    public Vector3 tiling = new Vector3(64f, 64f, 64f);
    public float monochromeTiling = 64f;
    public FilterMode filterMode = FilterMode.Bilinear;
    public Texture2D noiseTexture;
    public Shader noiseShader;
    private Material noiseMaterial = (Material) null;
    public Shader dx11NoiseShader;
    private Material dx11NoiseMaterial = (Material) null;
    private static float TILE_AMOUNT = 64f;
    private Mesh mesh;

    private void Awake() => this.mesh = new Mesh();

    public override bool CheckResources()
    {
      this.CheckSupport(false);
      this.noiseMaterial = this.CheckShaderAndCreateMaterial(this.noiseShader, this.noiseMaterial);
      if (this.dx11Grain && this.supportDX11)
        this.dx11NoiseMaterial = this.CheckShaderAndCreateMaterial(this.dx11NoiseShader, this.dx11NoiseMaterial);
      if (!this.isSupported)
        this.ReportAutoDisable();
      return this.isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckResources() || (Object) null == (Object) this.noiseTexture)
      {
        Graphics.Blit((Texture) source, destination);
        if (!((Object) null == (Object) this.noiseTexture))
          return;
        Debug.LogWarning((object) "Noise & Grain effect failing as noise texture is not assigned. please assign.", (Object) this.transform);
      }
      else
      {
        this.softness = Mathf.Clamp(this.softness, 0.0f, 0.99f);
        if (this.dx11Grain && this.supportDX11)
        {
          this.dx11NoiseMaterial.SetFloat("_DX11NoiseTime", (float) Time.frameCount);
          this.dx11NoiseMaterial.SetTexture("_NoiseTex", (Texture) this.noiseTexture);
          this.dx11NoiseMaterial.SetVector("_NoisePerChannel", (Vector4) (this.monochrome ? Vector3.one : this.intensities));
          this.dx11NoiseMaterial.SetVector("_MidGrey", (Vector4) new Vector3(this.midGrey, (float) (1.0 / (1.0 - (double) this.midGrey)), -1f / this.midGrey));
          this.dx11NoiseMaterial.SetVector("_NoiseAmount", (Vector4) (new Vector3(this.generalIntensity, this.blackIntensity, this.whiteIntensity) * this.intensityMultiplier));
          if ((double) this.softness > (double) Mathf.Epsilon)
          {
            RenderTexture temporary = RenderTexture.GetTemporary((int) ((double) source.width * (1.0 - (double) this.softness)), (int) ((double) source.height * (1.0 - (double) this.softness)));
            NoiseAndGrain.DrawNoiseQuadGrid(source, temporary, this.dx11NoiseMaterial, this.noiseTexture, this.mesh, this.monochrome ? 3 : 2);
            this.dx11NoiseMaterial.SetTexture("_NoiseTex", (Texture) temporary);
            Graphics.Blit((Texture) source, destination, this.dx11NoiseMaterial, 4);
            RenderTexture.ReleaseTemporary(temporary);
          }
          else
            NoiseAndGrain.DrawNoiseQuadGrid(source, destination, this.dx11NoiseMaterial, this.noiseTexture, this.mesh, this.monochrome ? 1 : 0);
        }
        else
        {
          if ((bool) (Object) this.noiseTexture)
          {
            this.noiseTexture.wrapMode = TextureWrapMode.Repeat;
            this.noiseTexture.filterMode = this.filterMode;
          }
          this.noiseMaterial.SetTexture("_NoiseTex", (Texture) this.noiseTexture);
          this.noiseMaterial.SetVector("_NoisePerChannel", (Vector4) (this.monochrome ? Vector3.one : this.intensities));
          this.noiseMaterial.SetVector("_NoiseTilingPerChannel", (Vector4) (this.monochrome ? Vector3.one * this.monochromeTiling : this.tiling));
          this.noiseMaterial.SetVector("_MidGrey", (Vector4) new Vector3(this.midGrey, (float) (1.0 / (1.0 - (double) this.midGrey)), -1f / this.midGrey));
          this.noiseMaterial.SetVector("_NoiseAmount", (Vector4) (new Vector3(this.generalIntensity, this.blackIntensity, this.whiteIntensity) * this.intensityMultiplier));
          if ((double) this.softness > (double) Mathf.Epsilon)
          {
            RenderTexture temporary = RenderTexture.GetTemporary((int) ((double) source.width * (1.0 - (double) this.softness)), (int) ((double) source.height * (1.0 - (double) this.softness)));
            NoiseAndGrain.DrawNoiseQuadGrid(source, temporary, this.noiseMaterial, this.noiseTexture, this.mesh, 2);
            this.noiseMaterial.SetTexture("_NoiseTex", (Texture) temporary);
            Graphics.Blit((Texture) source, destination, this.noiseMaterial, 1);
            RenderTexture.ReleaseTemporary(temporary);
          }
          else
            NoiseAndGrain.DrawNoiseQuadGrid(source, destination, this.noiseMaterial, this.noiseTexture, this.mesh, 0);
        }
      }
    }

    private static void DrawNoiseQuadGrid(
      RenderTexture source,
      RenderTexture dest,
      Material fxMaterial,
      Texture2D noise,
      Mesh mesh,
      int passNr)
    {
      RenderTexture.active = dest;
      fxMaterial.SetTexture("_MainTex", (Texture) source);
      GL.PushMatrix();
      GL.LoadOrtho();
      fxMaterial.SetPass(passNr);
      NoiseAndGrain.BuildMesh(mesh, source, noise);
      Transform transform = Camera.main.transform;
      Vector3 position = transform.position;
      Quaternion rotation = transform.rotation;
      transform.position = Vector3.zero;
      transform.rotation = Quaternion.identity;
      Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
      transform.position = position;
      transform.rotation = rotation;
      GL.PopMatrix();
    }

    private static void BuildMesh(Mesh mesh, RenderTexture source, Texture2D noise)
    {
      float noiseSize = (float) noise.width * 1f;
      float f = 1f * (float) source.width / NoiseAndGrain.TILE_AMOUNT;
      float num1 = (float) (1.0 * (double) source.width / (1.0 * (double) source.height));
      float num2 = 1f / f;
      float num3 = num2 * num1;
      int width = (int) Mathf.Ceil(f);
      int height = (int) Mathf.Ceil(1f / num3);
      if (mesh.vertices.Length != width * height * 4)
      {
        Vector3[] vector3Array = new Vector3[width * height * 4];
        Vector2[] vector2Array = new Vector2[width * height * 4];
        int[] numArray = new int[width * height * 6];
        int index1 = 0;
        int index2 = 0;
        for (float x = 0.0f; (double) x < 1.0; x += num2)
        {
          for (float y = 0.0f; (double) y < 1.0; y += num3)
          {
            vector3Array[index1] = new Vector3(x, y, 0.1f);
            vector3Array[index1 + 1] = new Vector3(x + num2, y, 0.1f);
            vector3Array[index1 + 2] = new Vector3(x + num2, y + num3, 0.1f);
            vector3Array[index1 + 3] = new Vector3(x, y + num3, 0.1f);
            vector2Array[index1] = new Vector2(0.0f, 0.0f);
            vector2Array[index1 + 1] = new Vector2(1f, 0.0f);
            vector2Array[index1 + 2] = new Vector2(1f, 1f);
            vector2Array[index1 + 3] = new Vector2(0.0f, 1f);
            numArray[index2] = index1;
            numArray[index2 + 1] = index1 + 1;
            numArray[index2 + 2] = index1 + 2;
            numArray[index2 + 3] = index1;
            numArray[index2 + 4] = index1 + 2;
            numArray[index2 + 5] = index1 + 3;
            index1 += 4;
            index2 += 6;
          }
        }
        mesh.vertices = vector3Array;
        mesh.uv2 = vector2Array;
        mesh.triangles = numArray;
      }
      NoiseAndGrain.BuildMeshUV0(mesh, width, height, noiseSize, noise.width);
    }

    private static void BuildMeshUV0(
      Mesh mesh,
      int width,
      int height,
      float noiseSize,
      int noiseWidth)
    {
      float num1 = noiseSize / ((float) noiseWidth * 1f);
      float num2 = 1f / noiseSize;
      Vector2[] vector2Array = new Vector2[width * height * 4];
      int index1 = 0;
      for (int index2 = 0; index2 < width * height; ++index2)
      {
        float f1 = Random.Range(0.0f, noiseSize);
        float f2 = Random.Range(0.0f, noiseSize);
        float x = Mathf.Floor(f1) * num2;
        float y = Mathf.Floor(f2) * num2;
        vector2Array[index1] = new Vector2(x, y);
        vector2Array[index1 + 1] = new Vector2(x + num1 * num2, y);
        vector2Array[index1 + 2] = new Vector2(x + num1 * num2, y + num1 * num2);
        vector2Array[index1 + 3] = new Vector2(x, y + num1 * num2);
        index1 += 4;
      }
      mesh.uv = vector2Array;
    }
  }
}
