using System;
using Assets.Engine.Source.Utility;
using Engine.Common.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Behaviours.Localization;

public class ImageLocalizerLazy : ImageLocalizerBase {
	[SerializeField] private LanguageSprite[] sprites;

	protected override Sprite GetSprite(LanguageEnum language) {
		Sprite sprite1 = null;
		for (var index = 0; index < sprites.Length; ++index) {
			var sprite2 = sprites[index];
			if (sprite2.Language == language) {
				sprite1 = sprite2.Sprite;
				break;
			}
		}

		return sprite1;
	}

	protected override void OnDisconnectFromEngine() {
		base.OnDisconnectFromEngine();
		for (var index = 0; index < sprites.Length; ++index)
			sprites[index].Dispose();
		Destroy(GetComponent<Image>().sprite);
	}

	[Serializable]
	private struct LanguageSprite : IDisposable {
		[SerializeField] private LanguageEnum language;
		[SerializeField] private ObjectReference spriteReference;
		private Sprite sprite;

		public LanguageEnum Language => language;

		public Sprite Sprite {
			get {
				if (sprite == null)
					sprite = ObjectCreator.InstantiateFromResources<Sprite>(spriteReference.Path);
				return sprite;
			}
		}

		public void Dispose() {
			if (!(sprite != null))
				return;
			sprite = null;
		}
	}
}