﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace JerboaAnimationInstancing
{
  public class JerboaInstancingManager : IDisposable
  {
    private List<JerboaInstanceDescription> aniInstancingList;
    private Dictionary<int, VertexCache> vertexCachePool;
    private Dictionary<int, InstanceData> instanceDataPool;
    private const int InstancingSizePerPackage = 1001;
    private int instancingPackageSize = 1001;
    private List<AnimationTexture> animationTextureList = [];
    private BoundingSphere[] boundingSphere;
    private int usedBoundingSphereCount;
    private CullingGroup cullingGroup;
    private JerboaManager jerboaManager;
    private int jerboaCountPrevFrame = -1;
    private int currentMoveBoundingSpheresIndex;

    public int InstancingPackageSize
    {
      get => instancingPackageSize;
      set => instancingPackageSize = value;
    }

    public JerboaInstancingManager(JerboaManager jerboaManager)
    {
      this.jerboaManager = jerboaManager;
      boundingSphere = new BoundingSphere[5000];
      InitializeCullingGroup();
      aniInstancingList = new List<JerboaInstanceDescription>(1000);
      vertexCachePool = new Dictionary<int, VertexCache>();
      instanceDataPool = new Dictionary<int, InstanceData>();
    }

    public void Dispose()
    {
      ReleaseBuffer();
      cullingGroup.Dispose();
      cullingGroup = null;
    }

    private void OnApplicationFocus(bool focus)
    {
      if (!focus)
        return;
      RefreshMaterial();
    }

    private void Start()
    {
    }

    private void InitializeCullingGroup()
    {
      cullingGroup = new CullingGroup();
      cullingGroup.targetCamera = Camera.main;
      cullingGroup.onStateChanged = CullingStateChanged;
      cullingGroup.SetBoundingSpheres(boundingSphere);
      usedBoundingSphereCount = 0;
      cullingGroup.SetBoundingSphereCount(usedBoundingSphereCount);
    }

    public void Update(Vector3 playerPosition)
    {
      if (!jerboaManager.Visible)
        return;
      int jerboaCountThisFrame = (int) (aniInstancingList.Count * (double) jerboaManager.SmoothedJerboaWeight);
      if (jerboaCountPrevFrame == -1)
        jerboaCountPrevFrame = jerboaCountThisFrame;
      ApplyBoneMatrix(playerPosition, jerboaCountPrevFrame, jerboaCountThisFrame);
      Render();
      jerboaCountPrevFrame = jerboaCountThisFrame;
    }

    private void Render()
    {
      foreach (KeyValuePair<int, VertexCache> keyValuePair in vertexCachePool)
      {
        VertexCache vertexCache = keyValuePair.Value;
        foreach (KeyValuePair<int, MaterialBlock> instanceBlock in vertexCache.instanceBlockList)
        {
          List<InstancingPackage>[] packageList = instanceBlock.Value.packageList;
          for (int aniTextureIndex = 0; aniTextureIndex != packageList.Length; ++aniTextureIndex)
          {
            for (int index = 0; index != packageList[aniTextureIndex].Count; ++index)
            {
              InstancingPackage package = packageList[aniTextureIndex][index];
              if (package.instancingCount != 0)
              {
                for (int submeshIndex = 0; submeshIndex != package.subMeshCount; ++submeshIndex)
                {
                  InstanceData instanceData = instanceBlock.Value.instanceData;
                  PreparePackageMaterial(package, vertexCache, aniTextureIndex);
                  package.propertyBlock.SetFloatArray("frameIndex", instanceData.frameIndex[aniTextureIndex][index]);
                  package.propertyBlock.SetFloatArray("preFrameIndex", instanceData.preFrameIndex[aniTextureIndex][index]);
                  package.propertyBlock.SetFloatArray("transitionProgress", instanceData.transitionProgress[aniTextureIndex][index]);
                  Graphics.DrawMeshInstanced(vertexCache.mesh, submeshIndex, package.material[submeshIndex], instanceData.worldMatrix[aniTextureIndex][index], package.instancingCount, package.propertyBlock, vertexCache.shadowcastingMode, vertexCache.receiveShadow, vertexCache.layer);
                }
                package.instancingCount = 0;
              }
            }
            instanceBlock.Value.runtimePackageIndex[aniTextureIndex] = 0;
          }
        }
      }
    }

    public void Clear()
    {
      aniInstancingList.Clear();
      cullingGroup.Dispose();
      vertexCachePool.Clear();
      instanceDataPool.Clear();
      InitializeCullingGroup();
    }

    public void AddInstance(JerboaInstanceDescription obj) => aniInstancingList.Add(obj);

    public void RemoveInstance(JerboaInstanceDescription instance)
    {
      Debug.Assert(aniInstancingList != null);
      if (!aniInstancingList.Remove(instance))
        return;
      --usedBoundingSphereCount;
      cullingGroup.SetBoundingSphereCount(usedBoundingSphereCount);
      Debug.Assert(usedBoundingSphereCount >= 0);
      if (usedBoundingSphereCount < 0)
        Debug.DebugBreak();
    }

    private void RefreshMaterial()
    {
      if (vertexCachePool == null)
        return;
      foreach (KeyValuePair<int, VertexCache> keyValuePair in vertexCachePool)
      {
        VertexCache vertexCache = keyValuePair.Value;
        foreach (KeyValuePair<int, MaterialBlock> instanceBlock in vertexCache.instanceBlockList)
        {
          for (int aniTextureIndex = 0; aniTextureIndex != instanceBlock.Value.packageList.Length; ++aniTextureIndex)
          {
            for (int index = 0; index != instanceBlock.Value.packageList[aniTextureIndex].Count; ++index)
              PreparePackageMaterial(instanceBlock.Value.packageList[aniTextureIndex][index], vertexCache, aniTextureIndex);
          }
        }
      }
    }

    private void ApplyBoneMatrix(
      Vector3 playerPosition,
      int jerboaCountPrevFrame,
      int jerboaCountThisFrame)
    {
      if (jerboaCountThisFrame == 0)
        return;
      ApplyRootMotion(playerPosition, jerboaCountPrevFrame, jerboaCountThisFrame);
      MoveBoundingSpheres(jerboaCountThisFrame);
      for (int index1 = 0; index1 < jerboaCountThisFrame; ++index1)
      {
        JerboaInstanceDescription aniInstancing = aniInstancingList[index1];
        if (aniInstancing.visible)
        {
          JerboaInstance.LodInfo lodInfo = aniInstancing.Source.lodInfo[0];
          int aniTextureIndex = aniInstancing.aniTextureIndex;
          for (int index2 = 0; index2 != lodInfo.vertexCacheList.Length; ++index2)
          {
            VertexCache vertexCache = lodInfo.vertexCacheList[index2];
            MaterialBlock materialBlock = lodInfo.materialBlockList[index2];
            int index3 = materialBlock.runtimePackageIndex[aniTextureIndex];
            InstancingPackage instancingPackage = materialBlock.packageList[aniTextureIndex][index3];
            if (instancingPackage.instancingCount + 1 > instancingPackageSize)
            {
              ++materialBlock.runtimePackageIndex[aniTextureIndex];
              int index4 = materialBlock.runtimePackageIndex[aniTextureIndex];
              if (index4 >= materialBlock.packageList[aniTextureIndex].Count)
              {
                InstancingPackage package = CreatePackage(materialBlock.instanceData, vertexCache.mesh, vertexCache.materials, aniTextureIndex);
                materialBlock.packageList[aniTextureIndex].Add(package);
                PreparePackageMaterial(package, vertexCache, aniTextureIndex);
                package.instancingCount = 1;
              }
              materialBlock.packageList[aniTextureIndex][index4].instancingCount = 1;
            }
            else
              ++instancingPackage.instancingCount;
            InstanceData instanceData = materialBlock.instanceData;
            int index5 = materialBlock.runtimePackageIndex[aniTextureIndex];
            int index6 = materialBlock.packageList[aniTextureIndex][index5].instancingCount - 1;
            if (index6 >= 0)
            {
              instanceData.worldMatrix[aniTextureIndex][index5][index6] = Matrix4x4.TRS(aniInstancing.Position, aniInstancing.Rotation, Vector3.one);
              float num1 = -1f;
              float num2 = aniInstancing.Source.aniInfo[aniInstancing.aniIndex].animationIndex + aniInstancing.curFrame;
              if (aniInstancing.preAniIndex >= 0)
                num1 = aniInstancing.Source.aniInfo[aniInstancing.preAniIndex].animationIndex + aniInstancing.preAniFrame;
              float transitionProgress = aniInstancing.transitionProgress;
              instanceData.frameIndex[aniTextureIndex][index5][index6] = num2;
              instanceData.preFrameIndex[aniTextureIndex][index5][index6] = num1;
              instanceData.transitionProgress[aniTextureIndex][index5][index6] = transitionProgress;
            }
          }
        }
      }
    }

    private void ApplyRootMotion(
      Vector3 playerPosition,
      int jerboaCountPrevFrame,
      int jerboaCountThisFrame)
    {
      float deltaTime = Time.deltaTime;
      for (int index = jerboaCountPrevFrame; index < jerboaCountThisFrame; ++index)
        aniInstancingList[index].Respawn();
      int layerMask = jerboaManager.RaycasLayerMask.value;
      for (int index = 0; index < jerboaCountThisFrame; ++index)
      {
        JerboaInstanceDescription aniInstancing = aniInstancingList[index];
        AnimationInfo animationInfoUnsafe = aniInstancing.GetCurrentAnimationInfoUnsafe();
        int curFrame = (int) aniInstancing.curFrame;
        aniInstancing.Position += aniInstancing.Rotation * animationInfoUnsafe.velocity[curFrame] * deltaTime;
        if (aniInstancing.UpdateAnimationUnsafeNoEventsNoTransitions(deltaTime) && Physics.Raycast(aniInstancing.Position + new Vector3(0.0f, 5f, 0.0f), new Vector3(0.0f, -1f, 0.0f), out RaycastHit hitInfo, 10f, layerMask, QueryTriggerInteraction.Ignore))
          aniInstancing.Position.y = hitInfo.point.y;
      }
    }

    private void MoveBoundingSpheres(int jerboaCount)
    {
      int num = jerboaCount / 20 + 1;
      for (int boundingSpheresIndex = currentMoveBoundingSpheresIndex; boundingSpheresIndex < currentMoveBoundingSpheresIndex + num; ++boundingSpheresIndex)
      {
        int index = boundingSpheresIndex % jerboaCount;
        JerboaInstanceDescription aniInstancing = aniInstancingList[index];
        aniInstancing.BoundingSphere.position = aniInstancing.Position;
        boundingSphere[index] = aniInstancing.BoundingSphere;
      }
      currentMoveBoundingSpheresIndex = (currentMoveBoundingSpheresIndex + num) % jerboaCount;
    }

    private int FindTexture_internal(string name)
    {
      for (int index = 0; index != animationTextureList.Count; ++index)
      {
        if (animationTextureList[index].name == name)
          return index;
      }
      return -1;
    }

    public AnimationTexture FindTexture(string name)
    {
      int textureInternal = FindTexture_internal(name);
      return textureInternal >= 0 ? animationTextureList[textureInternal] : null;
    }

    public AnimationTexture FindTexture(int index)
    {
      return 0 <= index && index < animationTextureList.Count ? animationTextureList[index] : null;
    }

    public VertexCache FindVertexCache(int renderName)
    {
      vertexCachePool.TryGetValue(renderName, out VertexCache vertexCache);
      return vertexCache;
    }

    private void ReadTexture(BinaryReader reader, string prefabName)
    {
      TextureFormat textureFormat = TextureFormat.RGBAHalf;
      if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
        textureFormat = TextureFormat.RGBA32;
      int length = reader.ReadInt32();
      int num1 = reader.ReadInt32();
      int num2 = reader.ReadInt32();
      AnimationTexture animationTexture = new AnimationTexture();
      animationTexture.boneTexture = new Texture2D[length];
      animationTexture.name = prefabName;
      animationTexture.blockWidth = num1;
      animationTexture.blockHeight = num2;
      animationTextureList.Add(animationTexture);
      for (int index = 0; index != length; ++index)
      {
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int count = reader.ReadInt32();
        byte[] numArray = new byte[count];
        byte[] data = reader.ReadBytes(count);
        Texture2D texture2D = new Texture2D(width, height, textureFormat, false);
        texture2D.LoadRawTextureData(data);
        texture2D.filterMode = FilterMode.Point;
        texture2D.Apply();
        animationTexture.boneTexture[index] = texture2D;
      }
    }

    public bool ImportAnimationTexture(string prefabName, BinaryReader reader)
    {
      if (FindTexture_internal(prefabName) >= 0)
        return true;
      ReadTexture(reader, prefabName);
      return true;
    }

    private void ReleaseBuffer()
    {
      if (vertexCachePool == null)
        return;
      vertexCachePool.Clear();
    }

    public InstancingPackage CreatePackage(
      InstanceData data,
      Mesh mesh,
      Material[] originalMaterial,
      int animationIndex)
    {
      InstancingPackage package = new InstancingPackage();
      package.material = new Material[mesh.subMeshCount];
      package.subMeshCount = mesh.subMeshCount;
      package.size = 1;
      for (int index = 0; index != mesh.subMeshCount; ++index)
      {
        package.material[index] = new Material(originalMaterial[index]);
        package.material[index].enableInstancing = true;
        package.material[index].EnableKeyword("INSTANCING_ON");
        package.propertyBlock = new MaterialPropertyBlock();
        package.material[index].EnableKeyword("USE_CONSTANT_BUFFER");
        package.material[index].DisableKeyword("USE_COMPUTE_BUFFER");
      }
      Matrix4x4[] matrix4x4Array = new Matrix4x4[instancingPackageSize];
      float[] numArray1 = new float[instancingPackageSize];
      float[] numArray2 = new float[instancingPackageSize];
      float[] numArray3 = new float[instancingPackageSize];
      data.worldMatrix[animationIndex].Add(matrix4x4Array);
      data.frameIndex[animationIndex].Add(numArray1);
      data.preFrameIndex[animationIndex].Add(numArray2);
      data.transitionProgress[animationIndex].Add(numArray3);
      return package;
    }

    private int GetIdentify(Material[] mat)
    {
      int identify = 0;
      for (int index = 0; index != mat.Length; ++index)
        identify += mat[index].name.GetHashCode();
      return identify;
    }

    private InstanceData CreateInstanceData(int packageCount)
    {
      InstanceData instanceData = new InstanceData();
      instanceData.worldMatrix = new List<Matrix4x4[]>[packageCount];
      instanceData.frameIndex = new List<float[]>[packageCount];
      instanceData.preFrameIndex = new List<float[]>[packageCount];
      instanceData.transitionProgress = new List<float[]>[packageCount];
      for (int index = 0; index != packageCount; ++index)
      {
        instanceData.worldMatrix[index] = [];
        instanceData.frameIndex[index] = [];
        instanceData.preFrameIndex[index] = [];
        instanceData.transitionProgress[index] = [];
      }
      return instanceData;
    }

    public void AddMeshVertex(
      string prefabName,
      JerboaInstance.LodInfo[] lodInfo,
      Transform[] bones,
      List<Matrix4x4> bindPose,
      int bonePerVertex,
      string alias = null)
    {
      if (Profiler.enabled)
        Profiler.BeginSample("AddMeshVertex()");
      for (int index1 = 0; index1 != lodInfo.Length; ++index1)
      {
        JerboaInstance.LodInfo lodInfo1 = lodInfo[index1];
        for (int index2 = 0; index2 != lodInfo1.skinnedMeshRenderer.Length; ++index2)
        {
          Mesh sharedMesh = lodInfo1.skinnedMeshRenderer[index2].sharedMesh;
          if (!(sharedMesh == null))
          {
            int hashCode = lodInfo1.skinnedMeshRenderer[index2].name.GetHashCode();
            int identify = GetIdentify(lodInfo1.skinnedMeshRenderer[index2].sharedMaterials);
            if (vertexCachePool.TryGetValue(hashCode, out VertexCache cache))
            {
              if (!cache.instanceBlockList.TryGetValue(identify, out MaterialBlock materialBlock))
              {
                materialBlock = CreateBlock(cache, lodInfo1.skinnedMeshRenderer[index2].sharedMaterials);
                cache.instanceBlockList.Add(identify, materialBlock);
              }
              lodInfo1.vertexCacheList[index2] = cache;
              lodInfo1.materialBlockList[index2] = materialBlock;
            }
            else
            {
              VertexCache vertexCache = CreateVertexCache(prefabName, hashCode, 0, sharedMesh);
              vertexCache.bindPose = bindPose.ToArray();
              MaterialBlock block = CreateBlock(vertexCache, lodInfo1.skinnedMeshRenderer[index2].sharedMaterials);
              vertexCache.instanceBlockList.Add(identify, block);
              SetupVertexCache(vertexCache, block, lodInfo1.skinnedMeshRenderer[index2], bones, bonePerVertex);
              lodInfo1.vertexCacheList[index2] = vertexCache;
              lodInfo1.materialBlockList[index2] = block;
            }
          }
        }
        int index3 = 0;
        int length = lodInfo1.skinnedMeshRenderer.Length;
        while (index3 != lodInfo1.meshRenderer.Length)
        {
          Mesh sharedMesh = lodInfo1.meshFilter[index3].sharedMesh;
          if (!(sharedMesh == null))
          {
            int hashCode1 = lodInfo1.meshRenderer[index3].name.GetHashCode();
            int hashCode2 = alias != null ? alias.GetHashCode() : 0;
            int identify = GetIdentify(lodInfo1.meshRenderer[index3].sharedMaterials);
            if (vertexCachePool.TryGetValue(hashCode1 + hashCode2, out VertexCache cache))
            {
              if (!cache.instanceBlockList.TryGetValue(identify, out MaterialBlock materialBlock))
              {
                materialBlock = CreateBlock(cache, lodInfo1.meshRenderer[index3].sharedMaterials);
                cache.instanceBlockList.Add(identify, materialBlock);
              }
              lodInfo1.vertexCacheList[length] = cache;
              lodInfo1.materialBlockList[length] = materialBlock;
            }
            else
            {
              VertexCache vertexCache = CreateVertexCache(prefabName, hashCode1, hashCode2, sharedMesh);
              if (bindPose != null)
                vertexCache.bindPose = bindPose.ToArray();
              MaterialBlock block = CreateBlock(vertexCache, lodInfo1.meshRenderer[index3].sharedMaterials);
              vertexCache.instanceBlockList.Add(identify, block);
              SetupVertexCache(vertexCache, block, lodInfo1.meshRenderer[index3], sharedMesh, bones, bonePerVertex);
              lodInfo1.vertexCacheList[lodInfo1.skinnedMeshRenderer.Length + index3] = vertexCache;
              lodInfo1.materialBlockList[lodInfo1.skinnedMeshRenderer.Length + index3] = block;
            }
          }
          ++index3;
          ++length;
        }
      }
      if (!Profiler.enabled)
        return;
      Profiler.EndSample();
    }

    private int GetPackageCount(VertexCache vertexCache)
    {
      int packageCount = 1;
      if (vertexCache.boneTextureIndex >= 0)
        packageCount = animationTextureList[vertexCache.boneTextureIndex].boneTexture.Length;
      return packageCount;
    }

    private MaterialBlock CreateBlock(
      VertexCache cache,
      Material[] materials)
    {
      MaterialBlock block = new MaterialBlock();
      int packageCount = GetPackageCount(cache);
      block.instanceData = CreateInstanceData(packageCount);
      block.packageList = new List<InstancingPackage>[packageCount];
      for (int index = 0; index != block.packageList.Length; ++index)
      {
        block.packageList[index] = [];
        InstancingPackage package = CreatePackage(block.instanceData, cache.mesh, materials, index);
        block.packageList[index].Add(package);
        PreparePackageMaterial(package, cache, index);
        package.instancingCount = 1;
      }
      block.runtimePackageIndex = new int[packageCount];
      return block;
    }

    private VertexCache CreateVertexCache(
      string prefabName,
      int renderName,
      int alias,
      Mesh mesh)
    {
      VertexCache vertexCache = new VertexCache();
      int key1 = renderName + alias;
      vertexCachePool[key1] = vertexCache;
      vertexCache.nameCode = key1;
      vertexCache.mesh = mesh;
      vertexCache.boneTextureIndex = FindTexture_internal(prefabName);
      vertexCache.weight = new Vector4[mesh.vertexCount];
      vertexCache.boneIndex = new Vector4[mesh.vertexCount];
      int packageCount = GetPackageCount(vertexCache);
      InstanceData instanceData1 = null;
      int key2 = prefabName.GetHashCode() + alias;
      if (!instanceDataPool.TryGetValue(key2, out instanceData1))
      {
        InstanceData instanceData2 = CreateInstanceData(packageCount);
        instanceDataPool.Add(key2, instanceData2);
      }
      vertexCache.instanceBlockList = new Dictionary<int, MaterialBlock>();
      return vertexCache;
    }

    private void SetupVertexCache(
      VertexCache vertexCache,
      MaterialBlock block,
      SkinnedMeshRenderer render,
      Transform[] boneTransform,
      int bonePerVertex)
    {
      int[] numArray = null;
      if (render.bones.Length != boneTransform.Length)
      {
        if (render.bones.Length == 0)
        {
          numArray = new int[1];
          int hashCode = render.transform.parent.name.GetHashCode();
          for (int index = 0; index != boneTransform.Length; ++index)
          {
            if (hashCode == boneTransform[index].name.GetHashCode())
            {
              numArray[0] = index;
              break;
            }
          }
        }
        else
        {
          numArray = new int[render.bones.Length];
          for (int index1 = 0; index1 != render.bones.Length; ++index1)
          {
            numArray[index1] = -1;
            int hashCode = render.bones[index1].name.GetHashCode();
            for (int index2 = 0; index2 != boneTransform.Length; ++index2)
            {
              if (hashCode == boneTransform[index2].name.GetHashCode())
              {
                numArray[index1] = index2;
                break;
              }
            }
          }
          if (numArray.Length == 0)
            numArray = null;
        }
      }
      if (Profiler.enabled)
        Profiler.BeginSample("Copy the vertex data in SetupVertexCache()");
      Mesh sharedMesh = render.sharedMesh;
      BoneWeight[] boneWeights = sharedMesh.boneWeights;
      Debug.Assert(boneWeights.Length != 0);
      for (int index = 0; index != sharedMesh.vertexCount; ++index)
      {
        vertexCache.weight[index].x = boneWeights[index].weight0;
        Debug.Assert(vertexCache.weight[index].x > 0.0);
        vertexCache.weight[index].y = boneWeights[index].weight1;
        vertexCache.weight[index].z = boneWeights[index].weight2;
        vertexCache.weight[index].w = boneWeights[index].weight3;
        vertexCache.boneIndex[index].x = numArray == null ? boneWeights[index].boneIndex0 : (float) numArray[boneWeights[index].boneIndex0];
        vertexCache.boneIndex[index].y = numArray == null ? boneWeights[index].boneIndex1 : (float) numArray[boneWeights[index].boneIndex1];
        vertexCache.boneIndex[index].z = numArray == null ? boneWeights[index].boneIndex2 : (float) numArray[boneWeights[index].boneIndex2];
        vertexCache.boneIndex[index].w = numArray == null ? boneWeights[index].boneIndex3 : (float) numArray[boneWeights[index].boneIndex3];
        Debug.Assert(vertexCache.boneIndex[index].x >= 0.0);
        switch (bonePerVertex)
        {
          case 1:
            vertexCache.weight[index].x = 1f;
            vertexCache.weight[index].y = -0.1f;
            vertexCache.weight[index].z = -0.1f;
            vertexCache.weight[index].w = -0.1f;
            break;
          case 2:
            float num1 = (float) (1.0 / (vertexCache.weight[index].x + (double) vertexCache.weight[index].y));
            vertexCache.weight[index].x *= num1;
            vertexCache.weight[index].y *= num1;
            vertexCache.weight[index].z = -0.1f;
            vertexCache.weight[index].w = -0.1f;
            break;
          case 3:
            float num2 = (float) (1.0 / (vertexCache.weight[index].x + (double) vertexCache.weight[index].y + vertexCache.weight[index].z));
            vertexCache.weight[index].x *= num2;
            vertexCache.weight[index].y *= num2;
            vertexCache.weight[index].z *= num2;
            vertexCache.weight[index].w = -0.1f;
            break;
        }
      }
      if (Profiler.enabled)
        Profiler.EndSample();
      if (vertexCache.materials == null)
        vertexCache.materials = render.sharedMaterials;
      SetupAdditionalData(vertexCache);
      for (int index = 0; index != block.packageList.Length; ++index)
      {
        InstancingPackage package = CreatePackage(block.instanceData, vertexCache.mesh, render.sharedMaterials, index);
        block.packageList[index].Add(package);
        PreparePackageMaterial(package, vertexCache, index);
      }
    }

    private void SetupVertexCache(
      VertexCache vertexCache,
      MaterialBlock block,
      MeshRenderer render,
      Mesh mesh,
      Transform[] boneTransform,
      int bonePerVertex)
    {
      int boneIndex = -1;
      if (boneTransform != null)
      {
        for (int index = 0; index != boneTransform.Length; ++index)
        {
          if (render.transform.parent.name.GetHashCode() == boneTransform[index].name.GetHashCode())
          {
            boneIndex = index;
            break;
          }
        }
      }
      if (boneIndex >= 0)
        BindAttachment(vertexCache, vertexCache, vertexCache.mesh, boneIndex);
      if (vertexCache.materials == null)
        vertexCache.materials = render.sharedMaterials;
      SetupAdditionalData(vertexCache);
      for (int index = 0; index != block.packageList.Length; ++index)
      {
        InstancingPackage package = CreatePackage(block.instanceData, vertexCache.mesh, render.sharedMaterials, index);
        block.packageList[index].Add(package);
        PreparePackageMaterial(package, vertexCache, index);
      }
    }

    public void SetupAdditionalData(VertexCache vertexCache)
    {
      Color[] colorArray = new Color[vertexCache.weight.Length];
      for (int index = 0; index != colorArray.Length; ++index)
      {
        colorArray[index].r = vertexCache.weight[index].x;
        colorArray[index].g = vertexCache.weight[index].y;
        colorArray[index].b = vertexCache.weight[index].z;
        colorArray[index].a = vertexCache.weight[index].w;
      }
      vertexCache.mesh.colors = colorArray;
      List<Vector4> vector4List = new List<Vector4>(vertexCache.boneIndex.Length);
      for (int index = 0; index != vertexCache.boneIndex.Length; ++index)
        vector4List.Add(vertexCache.boneIndex[index]);
      vertexCache.mesh.SetUVs(2, vector4List);
      vertexCache.mesh.UploadMeshData(false);
    }

    public void PreparePackageMaterial(
      InstancingPackage package,
      VertexCache vertexCache,
      int aniTextureIndex)
    {
      if (vertexCache.boneTextureIndex < 0)
        return;
      for (int index = 0; index != package.subMeshCount; ++index)
      {
        AnimationTexture animationTexture = animationTextureList[vertexCache.boneTextureIndex];
        package.material[index].SetTexture("_boneTexture", animationTexture.boneTexture[aniTextureIndex]);
        package.material[index].SetInt("_boneTextureWidth", animationTexture.boneTexture[aniTextureIndex].width);
        package.material[index].SetInt("_boneTextureHeight", animationTexture.boneTexture[aniTextureIndex].height);
        package.material[index].SetInt("_boneTextureBlockWidth", animationTexture.blockWidth);
        package.material[index].SetInt("_boneTextureBlockHeight", animationTexture.blockHeight);
        package.material[index].color = jerboaManager.Color;
      }
    }

    public void AddBoundingSphere(JerboaInstanceDescription instance)
    {
      boundingSphere[usedBoundingSphereCount++] = instance.BoundingSphere;
      cullingGroup.SetBoundingSphereCount(usedBoundingSphereCount);
      instance.visible = cullingGroup.IsVisible(usedBoundingSphereCount - 1);
    }

    private void CullingStateChanged(CullingGroupEvent evt)
    {
      Debug.Assert(evt.index < usedBoundingSphereCount);
      if (evt.hasBecomeVisible)
      {
        Debug.Assert(evt.index < aniInstancingList.Count);
        aniInstancingList[evt.index].visible = true;
      }
      if (!evt.hasBecomeInvisible)
        return;
      Debug.Assert(evt.index < aniInstancingList.Count);
      aniInstancingList[evt.index].visible = false;
    }

    public void BindAttachment(
      VertexCache parentCache,
      VertexCache attachmentCache,
      Mesh sharedMesh,
      int boneIndex)
    {
      Matrix4x4 inverse = parentCache.bindPose[boneIndex].inverse;
      attachmentCache.mesh = Object.Instantiate(sharedMesh);
      Vector3 column = inverse.GetColumn(3);
      Quaternion quaternion = RuntimeHelper.QuaternionFromMatrix(inverse);
      Vector3[] vertices = attachmentCache.mesh.vertices;
      for (int index = 0; index != attachmentCache.mesh.vertexCount; ++index)
      {
        vertices[index] = quaternion * vertices[index];
        vertices[index] = vertices[index] + column;
      }
      attachmentCache.mesh.vertices = vertices;
      for (int index = 0; index != attachmentCache.mesh.vertexCount; ++index)
      {
        attachmentCache.weight[index].x = 1f;
        attachmentCache.weight[index].y = -0.1f;
        attachmentCache.weight[index].z = -0.1f;
        attachmentCache.weight[index].w = -0.1f;
        attachmentCache.boneIndex[index].x = boneIndex;
      }
    }

    public class InstanceData
    {
      public List<Matrix4x4[]>[] worldMatrix;
      public List<float[]>[] frameIndex;
      public List<float[]>[] preFrameIndex;
      public List<float[]>[] transitionProgress;
    }

    public class InstancingPackage
    {
      public Material[] material;
      public int animationTextureIndex = 0;
      public int subMeshCount = 1;
      public int instancingCount;
      public int size;
      public MaterialPropertyBlock propertyBlock;
    }

    public class MaterialBlock
    {
      public InstanceData instanceData;
      public int[] runtimePackageIndex;
      public List<InstancingPackage>[] packageList;
    }

    public class VertexCache
    {
      public int nameCode;
      public Mesh mesh;
      public Dictionary<int, MaterialBlock> instanceBlockList;
      public Vector4[] weight;
      public Vector4[] boneIndex;
      public Material[] materials;
      public Matrix4x4[] bindPose;
      public Transform[] bonePose;
      public int boneTextureIndex = -1;
      public ShadowCastingMode shadowcastingMode;
      public bool receiveShadow;
      public int layer;
    }

    public class AnimationTexture
    {
      public string name { get; set; }

      public Texture2D[] boneTexture { get; set; }

      public int blockWidth { get; set; }

      public int blockHeight { get; set; }
    }
  }
}
