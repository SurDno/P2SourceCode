using System;
using Engine.Common.Services;
using UnityEngine;

namespace Engine.Behaviours.Localization;

public class ImageLocalizer : ImageLocalizerBase {
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

	[Serializable]
	private struct LanguageSprite {
		[SerializeField] private LanguageEnum language;
		[SerializeField] private Sprite sprite;

		public LanguageEnum Language => language;

		public Sprite Sprite => sprite;
	}
}