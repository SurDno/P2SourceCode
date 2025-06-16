// Decompiled with JetBrains decompiler
// Type: DeferredProjector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

#nullable disable
[ExecuteInEditMode]
public class DeferredProjector : MonoBehaviour
{
  private const CameraEvent RenderTime = CameraEvent.BeforeReflections;
  private const int MaxActiveInctances = 4096;
  [SerializeField]
  [FormerlySerializedAs("Material")]
  private Material material;
  [SerializeField]
  private DeferredProjector.Property[] properties;
  private int index = -1;
  private Material actualMaterial;
  private Vector3 lossyScale;
  private static DeferredProjector[] instances = new DeferredProjector[4096];
  private static BoundingSphere[] boundingSpheres = new BoundingSphere[4096];
  private static int[] cullingResults = new int[4096];
  private static int count = 0;
  private static CullingGroup cullingGroup;
  private static CommandBuffer commandBuffer;
  private static Camera bufferCamera;
  private static RenderTargetIdentifier[] mrt;
  private static Mesh boxMesh;
  private static int normalsID;
  private static int specularID;
  private static int cullId;
  private static int zTestId;

  public DeferredProjector.Property[] Properties => this.properties;

  public void PopulateBuffer(CommandBuffer buffer, bool inside)
  {
    if ((UnityEngine.Object) this.actualMaterial == (UnityEngine.Object) null)
      return;
    bool flag = (double) this.lossyScale.x < 0.0 ^ (double) this.lossyScale.y < 0.0 ^ (double) this.lossyScale.z < 0.0;
    if (this.properties != null && this.properties.Length != 0)
    {
      for (int index = 0; index < this.properties.Length; ++index)
        this.actualMaterial.SetFloat(this.properties[index].Name, this.properties[index].Value);
    }
    this.actualMaterial.SetInt(DeferredProjector.cullId, flag ^ inside ? 1 : 2);
    this.actualMaterial.SetInt(DeferredProjector.zTestId, inside ? 5 : 2);
    buffer.DrawMesh(DeferredProjector.BoxMesh, this.transform.localToWorldMatrix, this.actualMaterial, 0, -1);
  }

  private void DrawGizmo(bool selected)
  {
    Gizmos.matrix = this.transform.localToWorldMatrix;
    Color color = new Color(0.0f, 0.7f, 1f, 1f);
    if (selected)
    {
      color.a = 0.25f;
      Gizmos.color = color;
      Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
    color.a = selected ? 0.5f : 0.1f;
    Gizmos.color = color;
    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
  }

  private void OnEnable()
  {
    if (DeferredProjector.count == 4096)
      return;
    this.index = DeferredProjector.count;
    DeferredProjector.instances[this.index] = this;
    this.UpdateBoundingSphere();
    if ((UnityEngine.Object) this.material != (UnityEngine.Object) null)
      this.actualMaterial = new Material(this.material);
    ++DeferredProjector.count;
    if (DeferredProjector.count == 1)
    {
      Camera.onPreCull += new Camera.CameraCallback(DeferredProjector.OnPreCullEvent);
      Camera.onPreRender += new Camera.CameraCallback(DeferredProjector.OnPreRenderEvent);
      Camera.onPostRender += new Camera.CameraCallback(DeferredProjector.OnPostRenderEvent);
      DeferredProjector.cullingGroup = new CullingGroup();
      DeferredProjector.cullingGroup.SetBoundingSpheres(DeferredProjector.boundingSpheres);
      DeferredProjector.commandBuffer = new CommandBuffer();
      DeferredProjector.commandBuffer.name = "Decals";
    }
    DeferredProjector.cullingGroup.SetBoundingSphereCount(DeferredProjector.count);
  }

  private void OnDisable()
  {
    if (this.index == -1)
      return;
    int index = DeferredProjector.count - 1;
    if ((UnityEngine.Object) this.actualMaterial != (UnityEngine.Object) null)
    {
      if (Application.isPlaying)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.actualMaterial);
      else
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.actualMaterial);
      this.actualMaterial = (Material) null;
    }
    if (this.index != index)
    {
      DeferredProjector instance = DeferredProjector.instances[index];
      instance.index = this.index;
      DeferredProjector.instances[this.index] = instance;
      DeferredProjector.boundingSpheres[this.index] = DeferredProjector.boundingSpheres[index];
    }
    DeferredProjector.instances[index] = (DeferredProjector) null;
    --DeferredProjector.count;
    if (DeferredProjector.count == 0)
    {
      Camera.onPreCull -= new Camera.CameraCallback(DeferredProjector.OnPreCullEvent);
      Camera.onPreRender -= new Camera.CameraCallback(DeferredProjector.OnPreRenderEvent);
      Camera.onPostRender -= new Camera.CameraCallback(DeferredProjector.OnPostRenderEvent);
      DeferredProjector.cullingGroup.Dispose();
      DeferredProjector.cullingGroup = (CullingGroup) null;
      DeferredProjector.commandBuffer = (CommandBuffer) null;
    }
    else
      DeferredProjector.cullingGroup.SetBoundingSphereCount(DeferredProjector.count);
    this.index = -1;
  }

  private void OnDrawGizmos() => this.DrawGizmo(false);

  private void OnDrawGizmosSelected()
  {
    this.DrawGizmo(true);
    this.UpdateBoundingSphere();
  }

  private void UpdateBoundingSphere()
  {
    if (this.index == -1)
      return;
    this.lossyScale = this.transform.lossyScale;
    DeferredProjector.boundingSpheres[this.index] = new BoundingSphere(this.transform.position, this.lossyScale.magnitude * 0.5f);
  }

  private void OnValidate() => this.UpdateBoundingSphere();

  private static Mesh BoxMesh
  {
    get
    {
      if ((UnityEngine.Object) DeferredProjector.boxMesh == (UnityEngine.Object) null)
      {
        DeferredProjector.boxMesh = new Mesh();
        DeferredProjector.boxMesh.hideFlags = HideFlags.HideAndDontSave;
        DeferredProjector.boxMesh.vertices = new Vector3[8]
        {
          new Vector3(-0.5f, -0.5f, -0.5f),
          new Vector3(-0.5f, 0.5f, -0.5f),
          new Vector3(0.5f, 0.5f, -0.5f),
          new Vector3(0.5f, -0.5f, -0.5f),
          new Vector3(0.5f, -0.5f, 0.5f),
          new Vector3(0.5f, 0.5f, 0.5f),
          new Vector3(-0.5f, 0.5f, 0.5f),
          new Vector3(-0.5f, -0.5f, 0.5f)
        };
        DeferredProjector.boxMesh.triangles = new int[36]
        {
          0,
          1,
          2,
          2,
          3,
          0,
          3,
          2,
          5,
          5,
          4,
          3,
          4,
          5,
          6,
          6,
          7,
          4,
          7,
          6,
          1,
          1,
          0,
          7,
          1,
          6,
          5,
          5,
          2,
          1,
          7,
          0,
          3,
          3,
          4,
          7
        };
      }
      return DeferredProjector.boxMesh;
    }
  }

  private static void OnPreCullEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (DeferredProjector));
    DeferredProjector.OnPreCullEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private static void OnPreCullEvent2(Camera camera)
  {
    if ((UnityEngine.Object) DeferredProjector.cullingGroup.targetCamera != (UnityEngine.Object) null || camera.actualRenderingPath != RenderingPath.DeferredShading)
      return;
    DeferredProjector.cullingGroup.targetCamera = camera;
  }

  private static void OnPreRenderEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (DeferredProjector));
    DeferredProjector.OnPreRenderEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private static void OnPreRenderEvent2(Camera camera)
  {
    if ((UnityEngine.Object) DeferredProjector.cullingGroup.targetCamera != (UnityEngine.Object) camera)
      return;
    int num1 = DeferredProjector.cullingGroup.QueryIndices(true, DeferredProjector.cullingResults, 0);
    Vector3 position = camera.transform.position;
    float magnitude = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(-1f, -1f, -1f)).magnitude;
    if (num1 <= 0)
      return;
    int cullingMask = camera.cullingMask;
    int num2 = 0;
    for (int index = 0; index < num1; ++index)
    {
      if ((cullingMask & 1 << DeferredProjector.instances[DeferredProjector.cullingResults[index]].gameObject.layer) != 0)
      {
        if (num2 > 0)
          DeferredProjector.cullingResults[index - num2] = DeferredProjector.cullingResults[index];
      }
      else
        ++num2;
    }
    int num3 = num1 - num2;
    if (num3 > 0)
    {
      if (DeferredProjector.normalsID == 0)
        DeferredProjector.normalsID = Shader.PropertyToID("_CameraGBufferTexture2Copy");
      DeferredProjector.commandBuffer.GetTemporaryRT(DeferredProjector.normalsID, -1, -1);
      DeferredProjector.commandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer2, (RenderTargetIdentifier) DeferredProjector.normalsID);
      if (DeferredProjector.specularID == 0)
        DeferredProjector.specularID = Shader.PropertyToID("_CameraGBufferTexture1Copy");
      DeferredProjector.commandBuffer.GetTemporaryRT(DeferredProjector.specularID, -1, -1);
      DeferredProjector.commandBuffer.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer1, (RenderTargetIdentifier) DeferredProjector.specularID);
      if (DeferredProjector.mrt == null)
        DeferredProjector.mrt = new RenderTargetIdentifier[4]
        {
          (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0,
          (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer1,
          (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer2,
          (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget
        };
      DeferredProjector.commandBuffer.SetRenderTarget(DeferredProjector.mrt, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
      if (DeferredProjector.cullId == 0)
        DeferredProjector.cullId = Shader.PropertyToID("_Cull");
      if (DeferredProjector.zTestId == 0)
        DeferredProjector.zTestId = Shader.PropertyToID("_ZTest");
      for (int index = 0; index < num3; ++index)
      {
        int cullingResult = DeferredProjector.cullingResults[index];
        BoundingSphere boundingSphere = DeferredProjector.boundingSpheres[cullingResult];
        float num4 = magnitude + boundingSphere.radius;
        bool inside = (double) (boundingSphere.position - position).sqrMagnitude < (double) num4 * (double) num4;
        DeferredProjector.instances[cullingResult].PopulateBuffer(DeferredProjector.commandBuffer, inside);
      }
      DeferredProjector.commandBuffer.ReleaseTemporaryRT(DeferredProjector.normalsID);
      DeferredProjector.commandBuffer.ReleaseTemporaryRT(DeferredProjector.specularID);
      camera.AddCommandBuffer(CameraEvent.BeforeReflections, DeferredProjector.commandBuffer);
      DeferredProjector.bufferCamera = camera;
    }
  }

  private static void OnPostRenderEvent(Camera camera)
  {
    if (Profiler.enabled)
      Profiler.BeginSample(nameof (DeferredProjector));
    DeferredProjector.OnPostRenderEvent2(camera);
    if (!Profiler.enabled)
      return;
    Profiler.EndSample();
  }

  private static void OnPostRenderEvent2(Camera camera)
  {
    if ((UnityEngine.Object) DeferredProjector.cullingGroup.targetCamera != (UnityEngine.Object) camera)
      return;
    DeferredProjector.cullingGroup.targetCamera = (Camera) null;
    if (!((UnityEngine.Object) DeferredProjector.bufferCamera == (UnityEngine.Object) camera))
      return;
    camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, DeferredProjector.commandBuffer);
    DeferredProjector.commandBuffer.Clear();
    DeferredProjector.bufferCamera = (Camera) null;
  }

  [Serializable]
  public struct Property
  {
    public string Name;
    public float Value;
  }
}
