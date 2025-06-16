using System.Linq;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayerPolyhedron : MonoBehaviour {
	[SerializeField] private bool playInstantly;
	[SerializeField] private AudioClip[] clips = new AudioClip[0];
	[SerializeField] private float minRadius;
	[SerializeField] private float maxRadius;
	[SerializeField] private float minAngle;
	[SerializeField] private float maxAngle;
	[SerializeField] private float baseVolume;
	[SerializeField] private float soundFadeSpeed = 0.2f;
	private AudioSource audioSource;
	private float wait = 0.1f;
	private float waitingForUpdate;

	private void Start() {
		audioSource = GetComponent<AudioSource>();
		clips = clips.Where(o => o != null).ToArray();
		if (clips.Length == 0)
			return;
		var audioClip = clips.Random();
		audioSource.loop = true;
		audioSource.clip = audioClip;
		audioSource.PlayAndCheck();
		audioSource.volume = 0.0f;
	}

	private void Update() {
		if (clips.Length == 0)
			return;
		waitingForUpdate += Time.deltaTime;
		if (waitingForUpdate < (double)wait)
			return;
		UpdateVolume();
		waitingForUpdate = 0.0f;
	}

	private void UpdateVolume() {
		var service = ServiceLocator.GetService<ISimulation>();
		if (service == null)
			audioSource.enabled = false;
		else {
			var player = service.Player;
			if (player == null || ((IEntityView)player).GameObject == null)
				audioSource.enabled = false;
			else {
				var vector3 = (player != null ? ((IEntityView)player).Position : Vector3.zero) with {
					y = transform.position.y
				};
				var maxRadius = this.maxRadius;
				var magnitude = (transform.position - vector3).magnitude;
				var flag = magnitude < (double)maxRadius;
				if (audioSource.enabled != flag)
					audioSource.enabled = flag;
				if (magnitude < (double)minRadius)
					audioSource.volume = baseVolume;
				else {
					var num = Vector3.Angle(((IEntityView)player).GameObject.transform.forward with {
						y = 0.0f
					}, (transform.position - vector3) with {
						y = 0.0f
					});
					audioSource.volume = Mathf.MoveTowards(audioSource.volume,
						baseVolume * (num < (double)minAngle ? 1f : 1f - Mathf.InverseLerp(minAngle, maxAngle, num)) *
						(1f - Mathf.InverseLerp(minRadius, this.maxRadius, magnitude)), wait * soundFadeSpeed);
				}
			}
		}
	}
}