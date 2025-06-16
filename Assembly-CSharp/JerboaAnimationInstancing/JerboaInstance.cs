using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace JerboaAnimationInstancing;

[AddComponentMenu("JerboaInstance")]
public class JerboaInstance : MonoBehaviour {
	public GameObject prototype;
	public float playSpeed = 1f;
	public ShadowCastingMode shadowCastingMode;
	public bool receiveShadow;
	[NonSerialized] public int layer;
	[Range(1f, 4f)] public int bonePerVertex = 4;
	public AnimationInfo[] aniInfo;
	private ComparerHash comparer;
	private AnimationInfo searchInfo;
	[NonSerialized] public LodInfo[] lodInfo;
	[NonSerialized] public int lodLevel;
	private Transform[] allTransforms;

	private void Awake() {
		layer = gameObject.layer;
		switch (QualitySettings.blendWeights) {
			case BlendWeights.OneBone:
				bonePerVertex = 1;
				break;
			case BlendWeights.TwoBones:
				bonePerVertex = bonePerVertex > 2 ? 2 : bonePerVertex;
				break;
		}

		var component = GetComponent<LODGroup>();
		if (component != null) {
			this.lodInfo = new LodInfo[component.lodCount];
			var loDs = component.GetLODs();
			for (var index1 = 0; index1 != loDs.Length; ++index1)
				if (loDs[index1].renderers != null) {
					var lodInfo = new LodInfo {
						lodLevel = index1,
						vertexCacheList = new JerboaInstancingManager.VertexCache[loDs[index1].renderers.Length]
					};
					lodInfo.materialBlockList =
						new JerboaInstancingManager.MaterialBlock[lodInfo.vertexCacheList.Length];
					var skinnedMeshRendererList = new List<SkinnedMeshRenderer>();
					var meshRendererList = new List<MeshRenderer>();
					foreach (var renderer in loDs[index1].renderers) {
						if (renderer is SkinnedMeshRenderer)
							skinnedMeshRendererList.Add((SkinnedMeshRenderer)renderer);
						if (renderer is MeshRenderer)
							meshRendererList.Add((MeshRenderer)renderer);
					}

					lodInfo.skinnedMeshRenderer = skinnedMeshRendererList.ToArray();
					lodInfo.meshRenderer = meshRendererList.ToArray();
					lodInfo.meshFilter = null;
					for (var index2 = 0; index2 != loDs[index1].renderers.Length; ++index2)
						loDs[index1].renderers[index2].enabled = false;
					this.lodInfo[index1] = lodInfo;
				}
		} else {
			this.lodInfo = new LodInfo[1];
			var lodInfo = new LodInfo {
				lodLevel = 0,
				skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>(),
				meshRenderer = GetComponentsInChildren<MeshRenderer>(),
				meshFilter = GetComponentsInChildren<MeshFilter>()
			};
			lodInfo.vertexCacheList =
				new JerboaInstancingManager.VertexCache
					[lodInfo.skinnedMeshRenderer.Length + lodInfo.meshRenderer.Length];
			lodInfo.materialBlockList = new JerboaInstancingManager.MaterialBlock[lodInfo.vertexCacheList.Length];
			this.lodInfo[0] = lodInfo;
			for (var index = 0; index != lodInfo.meshRenderer.Length; ++index)
				lodInfo.meshRenderer[index].enabled = false;
			for (var index = 0; index != lodInfo.skinnedMeshRenderer.Length; ++index)
				lodInfo.skinnedMeshRenderer[index].enabled = false;
		}
	}

	private void OnDestroy() { }

	private void OnEnable() {
		playSpeed = 1f;
	}

	private void OnDisable() {
		playSpeed = 0.0f;
	}

	public bool InitializeAnimation(
		JerboaInstancingManager jerboaInstancingManager,
		JerboaAnimationManager jerboaAnimationManager,
		TextAsset textAsset) {
		if (this.prototype == null)
			Debug.LogError("The prototype is NULL. Please select the prototype first.");
		Debug.Assert(this.prototype != null);
		var prototype = this.prototype;
		var animationInfo =
			jerboaAnimationManager.FindAnimationInfo(jerboaInstancingManager, textAsset, this.prototype, this);
		if (animationInfo != null) {
			aniInfo = animationInfo.listAniInfo;
			Prepare(jerboaInstancingManager, aniInfo, animationInfo.extraBoneInfo);
		}

		searchInfo = new AnimationInfo();
		comparer = new ComparerHash();
		return true;
	}

	public void Prepare(
		JerboaInstancingManager jerboaInstancingManager,
		AnimationInfo[] infoList,
		ExtraBoneInfo extraBoneInfo) {
		aniInfo = infoList;
		var bindPose = new List<Matrix4x4>(150);
		var collection = RuntimeHelper.MergeBone(this.lodInfo[0].skinnedMeshRenderer, bindPose);
		allTransforms = collection;
		if (extraBoneInfo != null) {
			var transformList = new List<Transform>();
			transformList.AddRange(collection);
			var componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			for (var index1 = 0; index1 != extraBoneInfo.extraBone.Length; ++index1) {
				for (var index2 = 0; index2 != componentsInChildren.Length; ++index2)
					if (extraBoneInfo.extraBone[index1] == componentsInChildren[index2].name)
						transformList.Add(componentsInChildren[index2]);
				bindPose.Add(extraBoneInfo.extraBindPose[index1]);
			}

			allTransforms = transformList.ToArray();
		}

		jerboaInstancingManager.AddMeshVertex(prototype.name, this.lodInfo, allTransforms, bindPose, bonePerVertex);
		foreach (var lodInfo in this.lodInfo) {
			foreach (var vertexCache in lodInfo.vertexCacheList) {
				vertexCache.shadowcastingMode = shadowCastingMode;
				vertexCache.receiveShadow = receiveShadow;
				vertexCache.layer = layer;
			}
		}

		Destroy(GetComponent<Animator>());
	}

	public int FindAnimationInfo(int hash) {
		if (aniInfo == null)
			return -1;
		searchInfo.animationNameHash = hash;
		for (var animationInfo = 0; animationInfo < aniInfo.Length; ++animationInfo)
			if (aniInfo[animationInfo].animationNameHash == hash)
				return animationInfo;
		return -1;
	}

	public int GetAnimationCount() {
		return aniInfo != null ? aniInfo.Length : 0;
	}

	public class LodInfo {
		public int lodLevel;
		public SkinnedMeshRenderer[] skinnedMeshRenderer;
		public MeshRenderer[] meshRenderer;
		public MeshFilter[] meshFilter;
		public JerboaInstancingManager.VertexCache[] vertexCacheList;
		public JerboaInstancingManager.MaterialBlock[] materialBlockList;
	}
}