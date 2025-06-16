// Decompiled with JetBrains decompiler
// Type: JerboaAnimationInstancing.JerboaInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
namespace JerboaAnimationInstancing
{
  [AddComponentMenu("JerboaInstance")]
  public class JerboaInstance : MonoBehaviour
  {
    public GameObject prototype;
    public float playSpeed = 1f;
    public ShadowCastingMode shadowCastingMode;
    public bool receiveShadow;
    [NonSerialized]
    public int layer;
    [Range(1f, 4f)]
    public int bonePerVertex = 4;
    public AnimationInfo[] aniInfo;
    private ComparerHash comparer;
    private AnimationInfo searchInfo;
    [NonSerialized]
    public JerboaInstance.LodInfo[] lodInfo;
    [NonSerialized]
    public int lodLevel;
    private Transform[] allTransforms;

    private void Awake()
    {
      this.layer = this.gameObject.layer;
      switch (QualitySettings.blendWeights)
      {
        case BlendWeights.OneBone:
          this.bonePerVertex = 1;
          break;
        case BlendWeights.TwoBones:
          this.bonePerVertex = this.bonePerVertex > 2 ? 2 : this.bonePerVertex;
          break;
      }
      LODGroup component = this.GetComponent<LODGroup>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        this.lodInfo = new JerboaInstance.LodInfo[component.lodCount];
        LOD[] loDs = component.GetLODs();
        for (int index1 = 0; index1 != loDs.Length; ++index1)
        {
          if (loDs[index1].renderers != null)
          {
            JerboaInstance.LodInfo lodInfo = new JerboaInstance.LodInfo()
            {
              lodLevel = index1,
              vertexCacheList = new JerboaInstancingManager.VertexCache[loDs[index1].renderers.Length]
            };
            lodInfo.materialBlockList = new JerboaInstancingManager.MaterialBlock[lodInfo.vertexCacheList.Length];
            List<SkinnedMeshRenderer> skinnedMeshRendererList = new List<SkinnedMeshRenderer>();
            List<MeshRenderer> meshRendererList = new List<MeshRenderer>();
            foreach (Renderer renderer in loDs[index1].renderers)
            {
              if (renderer is SkinnedMeshRenderer)
                skinnedMeshRendererList.Add((SkinnedMeshRenderer) renderer);
              if (renderer is MeshRenderer)
                meshRendererList.Add((MeshRenderer) renderer);
            }
            lodInfo.skinnedMeshRenderer = skinnedMeshRendererList.ToArray();
            lodInfo.meshRenderer = meshRendererList.ToArray();
            lodInfo.meshFilter = (MeshFilter[]) null;
            for (int index2 = 0; index2 != loDs[index1].renderers.Length; ++index2)
              loDs[index1].renderers[index2].enabled = false;
            this.lodInfo[index1] = lodInfo;
          }
        }
      }
      else
      {
        this.lodInfo = new JerboaInstance.LodInfo[1];
        JerboaInstance.LodInfo lodInfo = new JerboaInstance.LodInfo()
        {
          lodLevel = 0,
          skinnedMeshRenderer = this.GetComponentsInChildren<SkinnedMeshRenderer>(),
          meshRenderer = this.GetComponentsInChildren<MeshRenderer>(),
          meshFilter = this.GetComponentsInChildren<MeshFilter>()
        };
        lodInfo.vertexCacheList = new JerboaInstancingManager.VertexCache[lodInfo.skinnedMeshRenderer.Length + lodInfo.meshRenderer.Length];
        lodInfo.materialBlockList = new JerboaInstancingManager.MaterialBlock[lodInfo.vertexCacheList.Length];
        this.lodInfo[0] = lodInfo;
        for (int index = 0; index != lodInfo.meshRenderer.Length; ++index)
          lodInfo.meshRenderer[index].enabled = false;
        for (int index = 0; index != lodInfo.skinnedMeshRenderer.Length; ++index)
          lodInfo.skinnedMeshRenderer[index].enabled = false;
      }
    }

    private void OnDestroy()
    {
    }

    private void OnEnable() => this.playSpeed = 1f;

    private void OnDisable() => this.playSpeed = 0.0f;

    public bool InitializeAnimation(
      JerboaInstancingManager jerboaInstancingManager,
      JerboaAnimationManager jerboaAnimationManager,
      TextAsset textAsset)
    {
      if ((UnityEngine.Object) this.prototype == (UnityEngine.Object) null)
        Debug.LogError((object) "The prototype is NULL. Please select the prototype first.");
      Debug.Assert((UnityEngine.Object) this.prototype != (UnityEngine.Object) null);
      GameObject prototype = this.prototype;
      JerboaAnimationManager.InstanceAnimationInfo animationInfo = jerboaAnimationManager.FindAnimationInfo(jerboaInstancingManager, textAsset, this.prototype, this);
      if (animationInfo != null)
      {
        this.aniInfo = animationInfo.listAniInfo;
        this.Prepare(jerboaInstancingManager, this.aniInfo, animationInfo.extraBoneInfo);
      }
      this.searchInfo = new AnimationInfo();
      this.comparer = new ComparerHash();
      return true;
    }

    public void Prepare(
      JerboaInstancingManager jerboaInstancingManager,
      AnimationInfo[] infoList,
      ExtraBoneInfo extraBoneInfo)
    {
      this.aniInfo = infoList;
      List<Matrix4x4> bindPose = new List<Matrix4x4>(150);
      Transform[] collection = RuntimeHelper.MergeBone(this.lodInfo[0].skinnedMeshRenderer, bindPose);
      this.allTransforms = collection;
      if (extraBoneInfo != null)
      {
        List<Transform> transformList = new List<Transform>();
        transformList.AddRange((IEnumerable<Transform>) collection);
        Transform[] componentsInChildren = this.gameObject.GetComponentsInChildren<Transform>();
        for (int index1 = 0; index1 != extraBoneInfo.extraBone.Length; ++index1)
        {
          for (int index2 = 0; index2 != componentsInChildren.Length; ++index2)
          {
            if (extraBoneInfo.extraBone[index1] == componentsInChildren[index2].name)
              transformList.Add(componentsInChildren[index2]);
          }
          bindPose.Add(extraBoneInfo.extraBindPose[index1]);
        }
        this.allTransforms = transformList.ToArray();
      }
      jerboaInstancingManager.AddMeshVertex(this.prototype.name, this.lodInfo, this.allTransforms, bindPose, this.bonePerVertex);
      foreach (JerboaInstance.LodInfo lodInfo in this.lodInfo)
      {
        foreach (JerboaInstancingManager.VertexCache vertexCache in lodInfo.vertexCacheList)
        {
          vertexCache.shadowcastingMode = this.shadowCastingMode;
          vertexCache.receiveShadow = this.receiveShadow;
          vertexCache.layer = this.layer;
        }
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) this.GetComponent<Animator>());
    }

    public int FindAnimationInfo(int hash)
    {
      if (this.aniInfo == null)
        return -1;
      this.searchInfo.animationNameHash = hash;
      for (int animationInfo = 0; animationInfo < this.aniInfo.Length; ++animationInfo)
      {
        if (this.aniInfo[animationInfo].animationNameHash == hash)
          return animationInfo;
      }
      return -1;
    }

    public int GetAnimationCount() => this.aniInfo != null ? this.aniInfo.Length : 0;

    public class LodInfo
    {
      public int lodLevel;
      public SkinnedMeshRenderer[] skinnedMeshRenderer;
      public MeshRenderer[] meshRenderer;
      public MeshFilter[] meshFilter;
      public JerboaInstancingManager.VertexCache[] vertexCacheList;
      public JerboaInstancingManager.MaterialBlock[] materialBlockList;
    }
  }
}
