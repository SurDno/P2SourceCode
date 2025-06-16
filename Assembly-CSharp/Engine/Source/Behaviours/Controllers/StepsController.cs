using System;
using System.Collections.Generic;
using Engine.Assets.Objects;
using Engine.Source.Audio;
using Inspectors;
using Rain;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Behaviours.Controllers;

public abstract class StepsController : MonoBehaviour {
	[SerializeField] private GameObject footEffectAudioModel;
	[SerializeField] private GameObject footAudioModel;
	private Dictionary<string, float> actions = new();
	private Dictionary<string, float> reactions = new();
	[SerializeField] private PhysicMaterial puddlePhysicMaterial;
	[SerializeField] private PhysicMaterial rainPhysicMaterial;
	protected const string footLeftEventName = "Skeleton.Humanoid.Foot_Left";
	protected const string footRightEventName = "Skeleton.Humanoid.Foot_Right";
	[SerializeField] private StepsData stepsData;
	private Dictionary<StepsEvent, Info> footActions = new();
	private Dictionary<StepsReaction, float> footReactions = new();
	[Inspected] private List<AudioSource> footLeftAudioSources = new();
	[Inspected] private List<AudioSource> footRightAudioSources = new();
	[Inspected] private List<AudioSource> footEffectAudioSources = new();
	private static List<KeyValuePair<StepsEvent, Info>> tmp = new();

	protected abstract AudioMixerGroup FootAudioMixer { get; }

	protected abstract AudioMixerGroup FootEffectsAudioMixer { get; }

	protected void RefreshMixers() {
		footLeftAudioSources.ForEach(audioSource => audioSource.outputAudioMixerGroup = FootAudioMixer);
		footRightAudioSources.ForEach(audioSource => audioSource.outputAudioMixerGroup = FootAudioMixer);
		footEffectAudioSources.ForEach(audioSource => audioSource.outputAudioMixerGroup = FootEffectsAudioMixer);
	}

	protected virtual void Awake() { }

	protected void OnStep(string data, bool isIndoor) {
		if (stepsData == null || string.IsNullOrEmpty(data))
			return;
		UpdateMaterial(data, transform.position, isIndoor);
		var val1 = 0.0f;
		footActions.Clear();
		footReactions.Clear();
		for (var index1 = 0; index1 < stepsData.Actions.Length; ++index1) {
			var action = stepsData.Actions[index1];
			if (action != null && actions.ContainsKey(action.Material.name))
				for (var index2 = 0; index2 < action.Events.Length; ++index2) {
					var key = action.Events[index2];
					if (key != null && key.Name == data) {
						val1 = Math.Max(val1, key.ReactionIntensity);
						var info = new Info {
							Action = action,
							Intensity = actions[action.Material.name]
						};
						footActions.Add(key, info);
					}
				}
		}

		for (var index = 0; index < stepsData.Reactions.Length; ++index) {
			var reaction = stepsData.Reactions[index];
			if (reaction != null && reactions.ContainsKey(reaction.Material.name))
				footReactions.Add(reaction, reactions[reaction.Material.name]);
		}

		var index3 = 0;
		var flag = false;
		tmp.Clear();
		foreach (var footAction in footActions)
			tmp.Add(footAction);
		foreach (var keyValuePair in tmp)
			if (!(keyValuePair.Value.Action.Material != puddlePhysicMaterial)) {
				var num = !float.IsNaN(keyValuePair.Value.Intensity)
					? keyValuePair.Value.Intensity
					: keyValuePair.Key.ActionIntensity;
				if (num < (double)keyValuePair.Value.Action.MinThesholdIntensity ||
				    num > (double)keyValuePair.Value.Action.MaxThesholdIntensity) {
					footActions.Remove(keyValuePair.Key);
					break;
				}

				footActions.Clear();
				footReactions.Clear();
				footActions.Add(keyValuePair.Key, keyValuePair.Value);
				break;
			}

		switch (data) {
			case "Skeleton.Humanoid.Foot_Left":
				if (footAudioModel != null) {
					FillAudioModelInstance(gameObject, footAudioModel, footLeftAudioSources, FootAudioMixer,
						footActions.Count);
					using (var enumerator = footActions.GetEnumerator()) {
						while (enumerator.MoveNext()) {
							var current = enumerator.Current;
							var num = !float.IsNaN(current.Value.Intensity)
								? current.Value.Intensity
								: current.Key.ActionIntensity;
							if (num >= (double)current.Value.Action.MinThesholdIntensity &&
							    num <= (double)current.Value.Action.MaxThesholdIntensity) {
								flag |= current.Key.HaveReaction;
								var footLeftAudioSource = footLeftAudioSources[index3];
								var audioClip = current.Key.Clips.Random();
								footLeftAudioSource.volume = num;
								footLeftAudioSource.clip = audioClip;
								footLeftAudioSource.PlayAndCheck();
								++index3;
							}
						}
					}
				}

				break;
			case "Skeleton.Humanoid.Foot_Right":
				if (footAudioModel != null) {
					FillAudioModelInstance(gameObject, footAudioModel, footRightAudioSources, FootAudioMixer,
						footActions.Count);
					using (var enumerator = footActions.GetEnumerator()) {
						while (enumerator.MoveNext()) {
							var current = enumerator.Current;
							var num = !float.IsNaN(current.Value.Intensity)
								? current.Value.Intensity
								: current.Key.ActionIntensity;
							if (num >= (double)current.Value.Action.MinThesholdIntensity &&
							    num <= (double)current.Value.Action.MaxThesholdIntensity) {
								flag |= current.Key.HaveReaction;
								var rightAudioSource = footRightAudioSources[index3];
								var audioClip = current.Key.Clips.Random();
								rightAudioSource.volume = num;
								rightAudioSource.clip = audioClip;
								rightAudioSource.PlayAndCheck();
								++index3;
							}
						}
					}
				}

				break;
			default:
				return;
		}

		var index4 = 0;
		if (!(footEffectAudioModel != null))
			return;
		FillAudioModelInstance(gameObject, footEffectAudioModel, footEffectAudioSources, FootEffectsAudioMixer,
			footReactions.Count);
		foreach (var footReaction in footReactions) {
			var num = (!float.IsNaN(footReaction.Value) ? footReaction.Value : footReaction.Key.Intensity) * val1;
			if (num >= (double)footReaction.Key.MinThesholdIntensity &&
			    num <= (double)footReaction.Key.MaxThesholdIntensity) {
				var effectAudioSource = footEffectAudioSources[index4];
				var audioClip = footReaction.Key.Clips.Random();
				effectAudioSource.volume = num;
				effectAudioSource.clip = audioClip;
				effectAudioSource.PlayAndCheck();
				++index4;
			}
		}
	}

	private void FillAudioModelInstance(
		GameObject parent,
		GameObject prefab,
		IList<AudioSource> collection,
		AudioMixerGroup mixer,
		int count) {
		if (prefab == null || prefab.GetComponent<AudioSource>() == null)
			return;
		while (collection.Count < count) {
			var gameObject = Instantiate(prefab);
			gameObject.transform.SetParent(parent.transform, false);
			var component = gameObject.GetComponent<AudioSource>();
			if (component == null)
				throw new Exception();
			component.loop = false;
			component.outputAudioMixerGroup = mixer;
			collection.Add(component);
		}
	}

	private void UpdateTerrainMaterial(GameObject go, Vector3 position) {
		if (go == null)
			return;
		var component = go.GetComponent<Terrain>();
		if (component == null)
			return;
		var terrainData = component.terrainData;
		var vector3_1 = position - go.transform.position;
		if (stepsData == null)
			return;
		var vector3_2 = vector3_1 / component.terrainData.size.x * component.terrainData.alphamapResolution;
		var alphamaps = terrainData.GetAlphamaps((int)vector3_2.x, (int)vector3_2.z, 1, 1);
		var index1 = -1;
		var f1 = float.NaN;
		for (var index2 = 0; index2 < stepsData.TileLayers.Length; ++index2) {
			var num = alphamaps[0, 0, index2];
			if (float.IsNaN(f1) || f1 <= (double)num) {
				f1 = num;
				index1 = index2;
			}
		}

		if (!float.IsNaN(f1) && stepsData.TileLayers.Length != 0 && f1 > 0.0)
			actions.Add(stepsData.TileLayers[index1].name, float.NaN);
		var vector3_3 = vector3_1 / component.terrainData.size.x * component.terrainData.detailResolution;
		var index3 = -1;
		var f2 = float.NaN;
		for (var layer = 0; layer < stepsData.DetailLayers.Length; ++layer) {
			float num = terrainData.GetDetailLayer((int)vector3_3.x, (int)vector3_3.z, 1, 1, layer)[0, 0];
			if (float.IsNaN(f2) || f2 <= (double)num) {
				f2 = num;
				index3 = layer;
			}
		}

		if (!float.IsNaN(f2) && stepsData.DetailLayers.Length != 0 && f2 > 0.0)
			reactions.Add(stepsData.DetailLayers[index3].name, float.NaN);
	}

	private void UpdateMaterial(string data, Vector3 position, bool isIndoor) {
		actions.Clear();
		reactions.Clear();
		var maxDistance = 1f;
		var puddlesLayer = ScriptableObjectInstance<GameSettingsData>.Instance.PuddlesLayer;
		RaycastHit hitInfo1;
		if (Physics.Raycast(position + new Vector3(0.0f, maxDistance / 2f, 0.0f), new Vector3(0.0f, -maxDistance, 0.0f),
			    out hitInfo1, maxDistance, puddlesLayer, QueryTriggerInteraction.Ignore) && hitInfo1.collider != null) {
			var gameObject = hitInfo1.collider.gameObject;
			if (gameObject != null) {
				var component = gameObject.GetComponent<Puddle>();
				float Wetness;
				if (component != null && component.GetWetness(hitInfo1, out Wetness)) {
					actions.Add(puddlePhysicMaterial.name, Wetness);
					component.AddRipple(hitInfo1.point, 0.1f, 1f);
				}
			}
		}

		var instance = RainManager.Instance;
		if (!(instance != null))
			return;
		var stepsLayer = ScriptableObjectInstance<GameSettingsData>.Instance.StepsLayer;
		RaycastHit hitInfo2;
		if (Physics.Raycast(position + new Vector3(0.0f, maxDistance / 2f, 0.0f), new Vector3(0.0f, -maxDistance, 0.0f),
			    out hitInfo2, maxDistance, stepsLayer, QueryTriggerInteraction.Ignore)) {
			var collider = hitInfo2.collider;
			if (collider != null) {
				var rainPhysicMaterial = this.rainPhysicMaterial;
				if (instance.rainIntensity > 1.4012984643248171E-45 && rainPhysicMaterial != null && !isIndoor)
					reactions.Add(rainPhysicMaterial.name, instance.rainIntensity);
				if (collider as TerrainCollider != null)
					UpdateTerrainMaterial(collider.gameObject, position);
				else
					UpdateMeshMaterial(collider);
			}
		}
	}

	private void UpdateMeshMaterial(Collider collider) {
		if (collider == null)
			return;
		if (collider.sharedMaterial == null)
			actions.Add(collider.material.name, float.NaN);
		else
			actions.Add(collider.sharedMaterial.name, float.NaN);
	}

	public struct Info {
		public StepsAction Action;
		public float Intensity;
	}
}