using Engine.Common.Services;
using Engine.Impl.Services;
using System;
using System.Collections;
using UnityEngine;

public class CreditsGenerator : MonoBehaviour
{
  [SerializeField]
  private LanguageEnum[] languages;
  [SerializeField]
  private TextAsset[] texts;
  [SerializeField]
  private string[] itemTags;
  [SerializeField]
  private CreditsItem[] itemPrefabs;
  [SerializeField]
  private float extents = 50f;
  [SerializeField]
  private int itemBufferSize = 64;
  private CreditsItem[] items;
  private float position;

  public float Size { get; private set; } = 0.0f;

  public float Position
  {
    get => this.position;
    set
    {
      this.position = value;
      ((RectTransform) this.transform).anchoredPosition = new Vector2(0.0f, this.position);
    }
  }

  private void Start() => this.StartCoroutine(this.Generate());

  private IEnumerator Generate()
  {
    LocalizationService localizationService = ServiceLocator.GetService<LocalizationService>();
    LanguageEnum language = localizationService != null ? localizationService.CurrentLanguage : LanguageEnum.English;
    TextAsset asset = (TextAsset) null;
    for (int i = 0; i < this.languages.Length; ++i)
    {
      if (this.languages[i] == language)
      {
        asset = this.texts[i];
        break;
      }
    }
    if (!((UnityEngine.Object) asset == (UnityEngine.Object) null))
    {
      string document = asset.text;
      this.items = new CreditsItem[this.itemBufferSize];
      int itemCount = 0;
      int i = 0;
      while (i < document.Length)
      {
        i = document.IndexOf('<', i);
        if (i != -1)
        {
          ++i;
          int openTagEnd = document.IndexOf('>', i);
          if (openTagEnd != -1)
          {
            string tag = document.Substring(i, openTagEnd - i);
            i = openTagEnd + 1;
            while (i < document.Length && (document[i] == '\n' || document[i] == '\r'))
              ++i;
            string closeTag = "</" + tag + ">";
            int closeTagStart = document.IndexOf(closeTag, i, StringComparison.InvariantCultureIgnoreCase);
            int closeTagEnd = 0;
            if (closeTagStart == -1)
            {
              closeTagStart = document.Length;
              closeTagEnd = closeTagStart;
            }
            else
              closeTagEnd = closeTagStart + closeTag.Length;
            while (i < closeTagStart && (document[closeTagStart - 1] == '\n' || document[closeTagStart - 1] == '\r'))
              --closeTagStart;
            if (i < closeTagStart)
            {
              CreditsItem prefab = (CreditsItem) null;
              for (int j = 0; j < this.itemTags.Length; ++j)
              {
                if (this.itemTags[j].Equals(tag, StringComparison.InvariantCultureIgnoreCase))
                {
                  prefab = this.itemPrefabs[j];
                  break;
                }
              }
              if ((UnityEngine.Object) prefab != (UnityEngine.Object) null)
              {
                CreditsItem instance = (CreditsItem) null;
                while (i < closeTagStart)
                {
                  if ((UnityEngine.Object) instance == (UnityEngine.Object) null || instance.IsFull())
                  {
                    do
                    {
                      yield return (object) null;
                    }
                    while ((double) this.Position + (double) this.extents <= (double) this.Size);
                    if (itemCount > 0)
                    {
                      int lastIndex = (itemCount - 1) % this.itemBufferSize;
                      this.Size = this.items[lastIndex].UpdateBottomEnd();
                    }
                    instance = UnityEngine.Object.Instantiate<CreditsItem>(prefab, this.transform, false);
                    instance.SetPosition(this.Size);
                    int index = itemCount % this.itemBufferSize;
                    if ((UnityEngine.Object) this.items[index] != (UnityEngine.Object) null)
                      UnityEngine.Object.Destroy((UnityEngine.Object) this.items[index].gameObject);
                    this.items[index] = instance;
                    ++itemCount;
                  }
                  int lineEnd = document.IndexOf('\n', i, closeTagStart - i);
                  if (lineEnd == -1)
                  {
                    instance.AddLine(document.Substring(i, closeTagStart - i));
                    break;
                  }
                  instance.AddLine(document.Substring(i, lineEnd - i));
                  i = lineEnd + 1;
                  while (i < closeTagStart && (document[i] == '\n' || document[i] == '\r'))
                    ++i;
                }
                instance = (CreditsItem) null;
              }
              prefab = (CreditsItem) null;
            }
            i = closeTagEnd;
            tag = (string) null;
            closeTag = (string) null;
          }
          else
            break;
        }
        else
          break;
      }
      yield return (object) null;
      if (itemCount > 0)
        this.Size = this.items[(itemCount - 1) % this.itemBufferSize].UpdateBottomEnd();
    }
  }
}
