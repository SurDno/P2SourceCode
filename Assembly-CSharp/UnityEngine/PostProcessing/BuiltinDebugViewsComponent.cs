using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
  public sealed class BuiltinDebugViewsComponent : 
    PostProcessingComponentCommandBuffer<BuiltinDebugViewsModel>
  {
    private const string k_ShaderString = "Hidden/Post FX/Builtin Debug Views";
    private ArrowArray m_Arrows;

    public override bool active => model.IsModeActive(BuiltinDebugViewsModel.Mode.Depth) || model.IsModeActive(BuiltinDebugViewsModel.Mode.Normals) || model.IsModeActive(BuiltinDebugViewsModel.Mode.MotionVectors);

    public override DepthTextureMode GetCameraFlags()
    {
      BuiltinDebugViewsModel.Mode mode = model.settings.mode;
      DepthTextureMode cameraFlags = DepthTextureMode.None;
      switch (mode)
      {
        case BuiltinDebugViewsModel.Mode.Depth:
          cameraFlags |= DepthTextureMode.Depth;
          break;
        case BuiltinDebugViewsModel.Mode.Normals:
          cameraFlags |= DepthTextureMode.DepthNormals;
          break;
        case BuiltinDebugViewsModel.Mode.MotionVectors:
          cameraFlags |= DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
          break;
      }
      return cameraFlags;
    }

    public override CameraEvent GetCameraEvent()
    {
      return model.settings.mode == BuiltinDebugViewsModel.Mode.MotionVectors ? CameraEvent.BeforeImageEffects : CameraEvent.BeforeImageEffectsOpaque;
    }

    public override string GetName() => "Builtin Debug Views";

    public override void PopulateCommandBuffer(CommandBuffer cb)
    {
      BuiltinDebugViewsModel.Settings settings = model.settings;
      Material material = context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      material.shaderKeywords = null;
      if (context.isGBufferAvailable)
        material.EnableKeyword("SOURCE_GBUFFER");
      switch (settings.mode)
      {
        case BuiltinDebugViewsModel.Mode.Depth:
          DepthPass(cb);
          break;
        case BuiltinDebugViewsModel.Mode.Normals:
          DepthNormalsPass(cb);
          break;
        case BuiltinDebugViewsModel.Mode.MotionVectors:
          MotionVectorsPass(cb);
          break;
      }
      context.Interrupt();
    }

    private void DepthPass(CommandBuffer cb)
    {
      Material mat = context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      BuiltinDebugViewsModel.DepthSettings depth = model.settings.depth;
      cb.SetGlobalFloat(Uniforms._DepthScale, 1f / depth.scale);
      cb.Blit(null, BuiltinRenderTextureType.CameraTarget, mat, 0);
    }

    private void DepthNormalsPass(CommandBuffer cb)
    {
      Material mat = context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      cb.Blit(null, BuiltinRenderTextureType.CameraTarget, mat, 1);
    }

    private void MotionVectorsPass(CommandBuffer cb)
    {
      Material material = context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
      BuiltinDebugViewsModel.MotionVectorsSettings motionVectors = model.settings.motionVectors;
      int num = Uniforms._TempRT;
      cb.GetTemporaryRT(num, context.width, context.height, 0, FilterMode.Bilinear);
      cb.SetGlobalFloat(Uniforms._Opacity, motionVectors.sourceOpacity);
      cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
      cb.Blit(BuiltinRenderTextureType.CameraTarget, num, material, 2);
      if (motionVectors.motionImageOpacity > 0.0 && motionVectors.motionImageAmplitude > 0.0)
      {
        int tempRt2 = Uniforms._TempRT2;
        cb.GetTemporaryRT(tempRt2, context.width, context.height, 0, FilterMode.Bilinear);
        cb.SetGlobalFloat(Uniforms._Opacity, motionVectors.motionImageOpacity);
        cb.SetGlobalFloat(Uniforms._Amplitude, motionVectors.motionImageAmplitude);
        cb.SetGlobalTexture(Uniforms._MainTex, num);
        cb.Blit(num, tempRt2, material, 3);
        cb.ReleaseTemporaryRT(num);
        num = tempRt2;
      }
      if (motionVectors.motionVectorsOpacity > 0.0 && motionVectors.motionVectorsAmplitude > 0.0)
      {
        PrepareArrows();
        float y = 1f / motionVectors.motionVectorsResolution;
        float x = y * context.height / context.width;
        cb.SetGlobalVector(Uniforms._Scale, new Vector2(x, y));
        cb.SetGlobalFloat(Uniforms._Opacity, motionVectors.motionVectorsOpacity);
        cb.SetGlobalFloat(Uniforms._Amplitude, motionVectors.motionVectorsAmplitude);
        cb.DrawMesh(m_Arrows.mesh, Matrix4x4.identity, material, 0, 4);
      }
      cb.SetGlobalTexture(Uniforms._MainTex, num);
      cb.Blit(num, BuiltinRenderTextureType.CameraTarget);
      cb.ReleaseTemporaryRT(num);
    }

    private void PrepareArrows()
    {
      int vectorsResolution = model.settings.motionVectors.motionVectorsResolution;
      int columns = vectorsResolution * Screen.width / Screen.height;
      if (m_Arrows == null)
        m_Arrows = new ArrowArray();
      if (m_Arrows.columnCount == columns && m_Arrows.rowCount == vectorsResolution)
        return;
      m_Arrows.Release();
      m_Arrows.BuildMesh(columns, vectorsResolution);
    }

    public override void OnDisable()
    {
      if (m_Arrows != null)
        m_Arrows.Release();
      m_Arrows = null;
    }

    private static class Uniforms
    {
      internal static readonly int _DepthScale = Shader.PropertyToID(nameof (_DepthScale));
      internal static readonly int _TempRT = Shader.PropertyToID(nameof (_TempRT));
      internal static readonly int _Opacity = Shader.PropertyToID(nameof (_Opacity));
      internal static readonly int _MainTex = Shader.PropertyToID(nameof (_MainTex));
      internal static readonly int _TempRT2 = Shader.PropertyToID(nameof (_TempRT2));
      internal static readonly int _Amplitude = Shader.PropertyToID(nameof (_Amplitude));
      internal static readonly int _Scale = Shader.PropertyToID(nameof (_Scale));
    }

    private enum Pass
    {
      Depth,
      Normals,
      MovecOpacity,
      MovecImaging,
      MovecArrows,
    }

    private class ArrowArray
    {
      public Mesh mesh { get; private set; }

      public int columnCount { get; private set; }

      public int rowCount { get; private set; }

      public void BuildMesh(int columns, int rows)
      {
        Vector3[] vector3Array = [
          new(0.0f, 0.0f, 0.0f),
          new(0.0f, 1f, 0.0f),
          new(0.0f, 1f, 0.0f),
          new(-1f, 1f, 0.0f),
          new(0.0f, 1f, 0.0f),
          new(1f, 1f, 0.0f)
        ];
        int capacity = 6 * columns * rows;
        List<Vector3> vector3List = new List<Vector3>(capacity);
        List<Vector2> vector2List = new List<Vector2>(capacity);
        for (int index1 = 0; index1 < rows; ++index1)
        {
          for (int index2 = 0; index2 < columns; ++index2)
          {
            Vector2 vector2 = new Vector2((0.5f + index2) / columns, (0.5f + index1) / rows);
            for (int index3 = 0; index3 < 6; ++index3)
            {
              vector3List.Add(vector3Array[index3]);
              vector2List.Add(vector2);
            }
          }
        }
        int[] indices = new int[capacity];
        for (int index = 0; index < capacity; ++index)
          indices[index] = index;
        Mesh mesh = new Mesh();
        mesh.hideFlags = HideFlags.DontSave;
        this.mesh = mesh;
        this.mesh.SetVertices(vector3List);
        this.mesh.SetUVs(0, vector2List);
        this.mesh.SetIndices(indices, MeshTopology.Lines, 0);
        this.mesh.UploadMeshData(true);
        columnCount = columns;
        rowCount = rows;
      }

      public void Release()
      {
        GraphicsUtils.Destroy(mesh);
        mesh = null;
      }
    }
  }
}
