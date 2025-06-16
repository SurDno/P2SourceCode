// Decompiled with JetBrains decompiler
// Type: PlagueCloudParticles
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class PlagueCloudParticles : MonoBehaviour
{
  private static Dictionary<int, Mesh> meshes;
  private static MaterialPropertyBlock mpb;
  public Shader KernelShader = (Shader) null;
  [FormerlySerializedAs("Radius")]
  public float EmissionRadius = 2f;
  [Range(1f, 65000f)]
  public int MaxPointCount = 1000;
  [Range(0.0f, 1f)]
  public float Emission = 1f;
  public float LifeTime = 10f;
  public Vector3 Acceleration = Vector3.zero;
  public float Drag = 1f;
  [Space]
  public float NoiseFrequency = 1f;
  public float NoiseAmplitude = 1f;
  public float NoiseMotion = 1f;
  [Space]
  public float Gravity = 0.0f;
  public float GravitySphereRadius = 0.5f;
  public float GravityFadeRadius = 10f;
  public float GravityMovementPrediction = 2f;
  [Space]
  public Texture2D Pattern;
  public Vector2 PatternSize;
  public float PatternPlaneForce;
  public float PatternOrthogonalForce;
  public float PatternRandomForce;
  private RenderTexture positionBuffer;
  private RenderTexture velocityBuffer;
  private RenderTexture targetBuffer;
  private Material kernelMaterial;
  private Vector3 noiseOffset;
  private Vector3 delayedPosition;
  private Vector3 velocity;

  private void Start()
  {
    int height = Mathf.CeilToInt((float) this.MaxPointCount / 256f);
    Mesh mesh = (Mesh) null;
    if (PlagueCloudParticles.meshes == null)
      PlagueCloudParticles.meshes = new Dictionary<int, Mesh>();
    else
      PlagueCloudParticles.meshes.TryGetValue(this.MaxPointCount, out mesh);
    if ((Object) mesh == (Object) null)
    {
      mesh = new Mesh();
      Vector3[] vector3Array = new Vector3[this.MaxPointCount];
      int[] indices = new int[this.MaxPointCount];
      int index = 0;
      int num1 = 0;
      int num2 = 0;
      while (index < this.MaxPointCount)
      {
        if (num1 == 256)
        {
          num1 = 0;
          ++num2;
        }
        vector3Array[index] = new Vector3((float) (((double) num1 + 0.5) / 256.0), ((float) num2 + 0.5f) / (float) height, Random.value);
        indices[index] = index;
        ++index;
        ++num1;
      }
      mesh.vertices = vector3Array;
      mesh.SetIndices(indices, MeshTopology.Points, 0);
      float num3 = 100f;
      mesh.bounds = new Bounds(Vector3.zero, new Vector3(num3, num3, num3));
      mesh.name = "Plague Cloud Particles (" + this.MaxPointCount.ToString() + ")";
      PlagueCloudParticles.meshes.Add(this.MaxPointCount, mesh);
    }
    this.GetComponent<MeshFilter>().sharedMesh = mesh;
    this.positionBuffer = new RenderTexture(256, height, 0, RenderTextureFormat.ARGBFloat);
    this.velocityBuffer = new RenderTexture(256, height, 0, RenderTextureFormat.ARGBFloat);
    this.targetBuffer = new RenderTexture(256, height, 0, RenderTextureFormat.ARGBFloat);
    if (!this.UpdateKernelMaterial())
      return;
    Graphics.Blit((Texture) null, this.positionBuffer, this.kernelMaterial, 0);
    this.ApplyPositionBuffer();
  }

  private void ApplyPositionBuffer()
  {
    if (PlagueCloudParticles.mpb == null)
      PlagueCloudParticles.mpb = new MaterialPropertyBlock();
    PlagueCloudParticles.mpb.SetTexture("_PositionBuffer", (Texture) this.positionBuffer);
    this.GetComponent<MeshRenderer>().SetPropertyBlock(PlagueCloudParticles.mpb);
  }

  private void SwapBuffers(ref RenderTexture rt0, ref RenderTexture rt1)
  {
    RenderTexture renderTexture = rt0;
    rt0 = rt1;
    rt1 = renderTexture;
  }

  private bool UpdateKernelMaterial()
  {
    if ((Object) this.kernelMaterial == (Object) null)
    {
      if (!((Object) this.KernelShader != (Object) null))
        return false;
      this.kernelMaterial = new Material(this.KernelShader);
    }
    float z = Mathf.Min(Time.deltaTime, 0.1f);
    Vector3 position = this.transform.position;
    this.delayedPosition = Vector3.SmoothDamp(this.delayedPosition, position, ref this.velocity, 0.1f);
    Vector3 vector3 = position + this.velocity * this.GravityMovementPrediction;
    this.kernelMaterial.SetVector("_Acceleration", new Vector4(this.Acceleration.x, this.Acceleration.y, this.Acceleration.z, Mathf.Exp(-this.Drag * z)));
    this.kernelMaterial.SetVector("_NoiseParams", (Vector4) new Vector2(this.NoiseFrequency, this.NoiseAmplitude));
    if (this.Acceleration == Vector3.zero)
      this.noiseOffset += Vector3.up * this.NoiseMotion * z;
    else
      this.noiseOffset += this.Acceleration.normalized * this.NoiseMotion * z;
    this.kernelMaterial.SetVector("_NoiseOffset", (Vector4) this.noiseOffset);
    this.kernelMaterial.SetFloat("_LifeTime", 1f / this.LifeTime);
    this.kernelMaterial.SetVector("_Emitter", new Vector4(position.x, position.y, position.z, this.EmissionRadius));
    this.kernelMaterial.SetVector("_GravityPosition", new Vector4(vector3.x, vector3.y, vector3.z, 0.0f));
    this.kernelMaterial.SetVector("_GravityConfig", new Vector4(this.Gravity, this.GravitySphereRadius, this.GravityFadeRadius, 0.0f));
    this.kernelMaterial.SetVector("_Config", new Vector4(this.Emission, Random.value, z, 0.0f));
    this.kernelMaterial.SetTexture("_PatternTex", (Texture) this.Pattern);
    this.kernelMaterial.SetVector("_PatternConfig", new Vector4(this.PatternPlaneForce, this.PatternOrthogonalForce, this.PatternRandomForce, 0.0f));
    Quaternion rotation = this.transform.rotation;
    this.kernelMaterial.SetMatrix("_ToPattern", Matrix4x4.TRS(position, rotation, new Vector3(this.PatternSize.x, this.PatternSize.y, 1f)).inverse);
    this.kernelMaterial.SetMatrix("_FromPatternRotation", Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));
    return true;
  }

  private void Update()
  {
    if (!this.UpdateKernelMaterial())
      return;
    this.kernelMaterial.SetTexture("_PositionBuffer", (Texture) this.positionBuffer);
    this.kernelMaterial.SetTexture("_VelocityBuffer", (Texture) this.velocityBuffer);
    Graphics.Blit((Texture) null, this.targetBuffer, this.kernelMaterial, 1);
    this.SwapBuffers(ref this.velocityBuffer, ref this.targetBuffer);
    this.kernelMaterial.SetTexture("_PositionBuffer", (Texture) this.positionBuffer);
    this.kernelMaterial.SetTexture("_VelocityBuffer", (Texture) this.velocityBuffer);
    Graphics.Blit((Texture) null, this.targetBuffer, this.kernelMaterial, 2);
    this.SwapBuffers(ref this.positionBuffer, ref this.targetBuffer);
    this.ApplyPositionBuffer();
  }
}
