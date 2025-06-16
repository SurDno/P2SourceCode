using Engine.Common;
using Engine.Source.Audio;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using SoundPropagation;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Source.Blueprints.Sounds;

[Category("Sounds")]
public class AudioTrack3dPrefabNode : FlowControlNode, IUpdatable {
	[Port("Value")] private ValueInput<bool> valueInput;
	[Port("Prefab")] private ValueInput<GameObject> sourcePrefab;
	[Port("Volume", 1f)] private ValueInput<float> volumeInput;
	[Port("FadeTime", 0.5f)] private ValueInput<float> fadeTimeInput;
	[Port("Mixer")] private ValueInput<AudioMixerGroup> mixerInput;
	[Port("Clip")] private ValueInput<AudioClip> clipInput;
	[Port("Target")] private ValueInput<Transform> targetInput;
	[Port("Propagation")] private ValueInput<bool> propagationInput;
	private AudioSource source;
	private bool run;
	private float progress;
	private float sleep;
	private float currentVolume;
	private float lastTime;
	private const float delayDestroy = 10f;

	public bool Play => valueInput.value;

	private void ComputeDestroy(float deltaTime) {
		if (!(source != null))
			return;
		sleep += deltaTime;
		if (sleep > 10.0) {
			sleep = 0.0f;
			Object.Destroy(source.gameObject);
			source = null;
		}
	}

	public override void OnGraphStarted() {
		base.OnGraphStarted();
		lastTime = Time.time;
		InstanceByRequest<UpdateService>.Instance.BlueprintSoundsUpdater.AddUpdatable(this);
	}

	public override void OnGraphStoped() {
		InstanceByRequest<UpdateService>.Instance.BlueprintSoundsUpdater.RemoveUpdatable(this);
		base.OnGraphStoped();
		if (!(source != null))
			return;
		Object.Destroy(source.gameObject);
		source = null;
	}

	public bool Complete { get; private set; }

	public void Reset() {
		if (source == null || !Complete)
			return;
		Complete = false;
		progress = 0.0f;
	}

	public void ComputeUpdate() {
		var deltaTime = Time.time - lastTime;
		lastTime = Time.time;
		Update(deltaTime);
	}

	private void Update(float deltaTime) {
		if (Complete)
			ComputeDestroy(deltaTime);
		else {
			var fade = fadeTimeInput.value;
			if (Play) {
				if (source == null) {
					var clip = clipInput.value;
					if (clip == null)
						return;
					var mixer = mixerInput.value;
					if (mixer == null)
						return;
					currentVolume = 0.0f;
					source = CreateAudioSource(sourcePrefab.value, clip, mixer, targetInput.value);
					if (propagationInput.value && targetInput.value != null)
						source.gameObject.AddComponent<SPAudioSource>().Origin = targetInput.value;
					source.PlayAndCheck();
					Debug.Log(ObjectInfoUtility.GetStream().Append("[Sounds]").Append(" Play sound, name : ")
						.Append(clip.name).Append(" , context : ").Append("(blueprint) ").Append(graph.agent.name));
				} else if (targetInput.value != null) {
					source.transform.position = targetInput.value.position;
					source.transform.rotation = targetInput.value.rotation;
				}
			}

			if (source == null)
				return;
			if (run) {
				progress += deltaTime;
				if (source.loop)
					progress %= source.clip.length;
				else {
					if (progress >= (double)source.clip.length) {
						Complete = true;
						run = false;
						source.Stop();
						return;
					}

					SoundUtility.ComputeFade(progress, source.clip.length, fade);
					source.volume = currentVolume * volumeInput.value;
				}
			}

			if (Play) {
				if (run) {
					if (currentVolume != 1.0) {
						currentVolume = Mathf.Clamp01(currentVolume + deltaTime / fade);
						source.volume = currentVolume * volumeInput.value;
					}
				} else {
					run = true;
					currentVolume = 0.0f;
					source.volume = currentVolume * volumeInput.value;
					source.PlayAndCheck();
					SoundUtility.SetTime(source, progress);
				}
			} else if (run) {
				if (currentVolume != 0.0) {
					currentVolume = Mathf.Clamp01(currentVolume - deltaTime / fade);
					source.volume = currentVolume * volumeInput.value;
				} else {
					run = false;
					source.Stop();
				}
			}

			if (run)
				sleep = 0.0f;
			else
				ComputeDestroy(deltaTime);
		}
	}

	private static AudioSource CreateAudioSource(
		GameObject prefab,
		AudioClip clip,
		AudioMixerGroup mixer,
		Transform transform) {
		var gameObject = UnityFactory.Instantiate(prefab, "[Sounds]");
		gameObject.name = clip.name;
		if (transform != null)
			gameObject.transform.position = transform.position;
		var length = clip.length;
		var component = gameObject.GetComponent<AudioSource>();
		component.clip = clip;
		component.outputAudioMixerGroup = mixer;
		return component;
	}
}