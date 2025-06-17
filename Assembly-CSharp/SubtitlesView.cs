using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Audio;
using Engine.Source.Services;
using Engine.Source.UI;
using UnityEngine;

public class SubtitlesView : EngineDependent
{
  [SerializeField]
  private GameObject lineViewPrefab;
  [SerializeField]
  private RectTransform layout;
  [SerializeField]
  private float fadeTime = 0.25f;
  private SubtitlesService service;
  private List<SubtitlesItem> items;
  private List<SubtitlesItem> itemPool;
  private List<GameObject> lineViewPool;
  private bool initialized;

  public float FadeTime => fadeTime;

  public void AddSubtitles(IEntity actor, string tag, AudioState audioState, Object context)
  {
    if (!service.SubtitlesEnabled || !service.DialogSubtitlesEnabled && ServiceLocator.GetService<UIService>().Active is IDialogWindow)
      return;
    string text = ServiceLocator.GetService<LocalizationService>().GetSubTitlesText(tag).Replace("<empty>", "").Trim();
    if (text == "")
      return;
    SubtitlesItem subtitlesItem = FindItem(actor);
    if (subtitlesItem == null)
    {
      if (itemPool.Count == 0)
      {
        subtitlesItem = new SubtitlesItem(this);
      }
      else
      {
        int index = itemPool.Count - 1;
        subtitlesItem = itemPool[index];
        itemPool.RemoveAt(index);
      }
      items.Add(subtitlesItem);
    }
    subtitlesItem.Start(actor, text, audioState, context);
  }

  public GameObject CreateLineView()
  {
    if (lineViewPool.Count == 0)
      return Instantiate(lineViewPrefab, layout, false);
    int index = lineViewPool.Count - 1;
    GameObject lineView = lineViewPool[index];
    lineViewPool.RemoveAt(index);
    lineView.transform.SetAsLastSibling();
    lineView.SetActive(true);
    return lineView;
  }

  private SubtitlesItem FindItem(IEntity actor)
  {
    for (int index = 0; index < items.Count; ++index)
    {
      if (items[index].Actor == actor)
        return items[index];
    }
    return null;
  }

  protected override void OnConnectToEngine()
  {
    if (!initialized)
    {
      service = ServiceLocator.GetService<SubtitlesService>();
      service.AddSubtitlesEvent += AddSubtitles;
      service.RemoveSubtitlesEvent += RemoveSubtitles;
      items = [];
      itemPool = [];
      lineViewPool = [];
      initialized = true;
    }
    UpdateItems();
  }

  private void OnDestroy()
  {
    if (!initialized)
      return;
    service.AddSubtitlesEvent -= AddSubtitles;
    service.RemoveSubtitlesEvent -= RemoveSubtitles;
  }

  protected override void OnDisconnectFromEngine()
  {
  }

  public void RemoveSubtitles(IEntity actor) => FindItem(actor)?.End();

  public void ReleaseLineView(GameObject itemView)
  {
    itemView.SetActive(false);
    lineViewPool.Add(itemView);
  }

  private void Update()
  {
    if (!initialized)
      return;
    UpdateItems();
  }

  private void UpdateItems()
  {
    for (int index = items.Count - 1; index >= 0; --index)
    {
      SubtitlesItem subtitlesItem = items[index];
      subtitlesItem.Update();
      if (subtitlesItem.Ended)
      {
        items.RemoveAt(index);
        itemPool.Add(subtitlesItem);
      }
    }
  }
}
