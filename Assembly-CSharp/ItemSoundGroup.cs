using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Sound Group")]
public class ItemSoundGroup : ScriptableObject {
	[SerializeField] private ClipSource put;
	[SerializeField] private ClipSource take;
	[SerializeField] private ClipSource use;
	[SerializeField] private ClipSource pourOut;

	public AudioClip GetPutClip() {
		return put.GetClip();
	}

	public AudioClip GetTakeClip() {
		return take.GetClip();
	}

	public AudioClip GetUseClip() {
		return use.GetClip();
	}

	public AudioClip GetPourOutClip() {
		return pourOut.GetClip();
	}

	[Serializable]
	private struct ClipSource {
		[SerializeField] private AudioClip clip;
		[SerializeField] private SoundCollection collection;

		public AudioClip GetClip() {
			if (clip != null)
				return clip;
			return collection != null ? collection.GetClip() : null;
		}
	}
}