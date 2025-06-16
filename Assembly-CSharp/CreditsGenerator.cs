using System;
using System.Collections;
using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

public class CreditsGenerator : MonoBehaviour {
	[SerializeField] private LanguageEnum[] languages;
	[SerializeField] private TextAsset[] texts;
	[SerializeField] private string[] itemTags;
	[SerializeField] private CreditsItem[] itemPrefabs;
	[SerializeField] private float extents = 50f;
	[SerializeField] private int itemBufferSize = 64;
	private CreditsItem[] items;
	private float position;

	public float Size { get; private set; }

	public float Position {
		get => position;
		set {
			position = value;
			((RectTransform)transform).anchoredPosition = new Vector2(0.0f, position);
		}
	}

	private void Start() {
		StartCoroutine(Generate());
	}

	private IEnumerator Generate() {
		var localizationService = ServiceLocator.GetService<LocalizationService>();
		var language = localizationService != null ? localizationService.CurrentLanguage : LanguageEnum.English;
		TextAsset asset = null;
		for (var i = 0; i < languages.Length; ++i)
			if (languages[i] == language) {
				asset = texts[i];
				break;
			}

		if (!(asset == null)) {
			var document = asset.text;
			items = new CreditsItem[itemBufferSize];
			var itemCount = 0;
			var i = 0;
			while (i < document.Length) {
				i = document.IndexOf('<', i);
				if (i != -1) {
					++i;
					var openTagEnd = document.IndexOf('>', i);
					if (openTagEnd != -1) {
						var tag = document.Substring(i, openTagEnd - i);
						i = openTagEnd + 1;
						while (i < document.Length && (document[i] == '\n' || document[i] == '\r'))
							++i;
						var closeTag = "</" + tag + ">";
						var closeTagStart = document.IndexOf(closeTag, i, StringComparison.InvariantCultureIgnoreCase);
						var closeTagEnd = 0;
						if (closeTagStart == -1) {
							closeTagStart = document.Length;
							closeTagEnd = closeTagStart;
						} else
							closeTagEnd = closeTagStart + closeTag.Length;

						while (i < closeTagStart &&
						       (document[closeTagStart - 1] == '\n' || document[closeTagStart - 1] == '\r'))
							--closeTagStart;
						if (i < closeTagStart) {
							CreditsItem prefab = null;
							for (var j = 0; j < itemTags.Length; ++j)
								if (itemTags[j].Equals(tag, StringComparison.InvariantCultureIgnoreCase)) {
									prefab = itemPrefabs[j];
									break;
								}

							if (prefab != null) {
								CreditsItem instance = null;
								while (i < closeTagStart) {
									if (instance == null || instance.IsFull()) {
										do {
											yield return null;
										} while (Position + (double)extents <= Size);

										if (itemCount > 0) {
											var lastIndex = (itemCount - 1) % itemBufferSize;
											Size = items[lastIndex].UpdateBottomEnd();
										}

										instance = Instantiate(prefab, transform, false);
										instance.SetPosition(Size);
										var index = itemCount % itemBufferSize;
										if (items[index] != null)
											Destroy(items[index].gameObject);
										items[index] = instance;
										++itemCount;
									}

									var lineEnd = document.IndexOf('\n', i, closeTagStart - i);
									if (lineEnd == -1) {
										instance.AddLine(document.Substring(i, closeTagStart - i));
										break;
									}

									instance.AddLine(document.Substring(i, lineEnd - i));
									i = lineEnd + 1;
									while (i < closeTagStart && (document[i] == '\n' || document[i] == '\r'))
										++i;
								}

								instance = null;
							}

							prefab = null;
						}

						i = closeTagEnd;
						tag = null;
						closeTag = null;
					} else
						break;
				} else
					break;
			}

			yield return null;
			if (itemCount > 0)
				Size = items[(itemCount - 1) % itemBufferSize].UpdateBottomEnd();
		}
	}
}