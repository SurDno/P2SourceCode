// Decompiled with JetBrains decompiler
// Type: SubtitlesView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Audio;
using Engine.Source.Services;
using Engine.Source.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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

  public float FadeTime => this.fadeTime;

  public void AddSubtitles(IEntity actor, string tag, AudioState audioState, UnityEngine.Object context)
  {
    if (!this.service.SubtitlesEnabled || !this.service.DialogSubtitlesEnabled && ServiceLocator.GetService<UIService>().Active is IDialogWindow)
      return;
    string text = ServiceLocator.GetService<LocalizationService>().GetSubTitlesText(tag).Replace("<empty>", "").Trim();
    if (text == "")
      return;
    SubtitlesItem subtitlesItem = this.FindItem(actor);
    if (subtitlesItem == null)
    {
      if (this.itemPool.Count == 0)
      {
        subtitlesItem = new SubtitlesItem(this);
      }
      else
      {
        int index = this.itemPool.Count - 1;
        subtitlesItem = this.itemPool[index];
        this.itemPool.RemoveAt(index);
      }
      this.items.Add(subtitlesItem);
    }
    subtitlesItem.Start(actor, text, audioState, context);
  }

  public GameObject CreateLineView()
  {
    if (this.lineViewPool.Count == 0)
      return UnityEngine.Object.Instantiate<GameObject>(this.lineViewPrefab, (Transform) this.layout, false);
    int index = this.lineViewPool.Count - 1;
    GameObject lineView = this.lineViewPool[index];
    this.lineViewPool.RemoveAt(index);
    lineView.transform.SetAsLastSibling();
    lineView.SetActive(true);
    return lineView;
  }

  private SubtitlesItem FindItem(IEntity actor)
  {
    for (int index = 0; index < this.items.Count; ++index)
    {
      if (this.items[index].Actor == actor)
        return this.items[index];
    }
    return (SubtitlesItem) null;
  }

  protected override void OnConnectToEngine()
  {
    if (!this.initialized)
    {
      this.service = ServiceLocator.GetService<SubtitlesService>();
      this.service.AddSubtitlesEvent += new Action<IEntity, string, AudioState, UnityEngine.Object>(this.AddSubtitles);
      this.service.RemoveSubtitlesEvent += new Action<IEntity>(this.RemoveSubtitles);
      this.items = new List<SubtitlesItem>();
      this.itemPool = new List<SubtitlesItem>();
      this.lineViewPool = new List<GameObject>();
      this.initialized = true;
    }
    this.UpdateItems();
  }

  private void OnDestroy()
  {
    if (!this.initialized)
      return;
    this.service.AddSubtitlesEvent -= new Action<IEntity, string, AudioState, UnityEngine.Object>(this.AddSubtitles);
    this.service.RemoveSubtitlesEvent -= new Action<IEntity>(this.RemoveSubtitles);
  }

  protected override void OnDisconnectFromEngine()
  {
  }

  public void RemoveSubtitles(IEntity actor) => this.FindItem(actor)?.End();

  public void ReleaseLineView(GameObject itemView)
  {
    itemView.SetActive(false);
    this.lineViewPool.Add(itemView);
  }

  private void Update()
  {
    if (!this.initialized)
      return;
    this.UpdateItems();
  }

  private void UpdateItems()
  {
    for (int index = this.items.Count - 1; index >= 0; --index)
    {
      SubtitlesItem subtitlesItem = this.items[index];
      subtitlesItem.Update();
      if (subtitlesItem.Ended)
      {
        this.items.RemoveAt(index);
        this.itemPool.Add(subtitlesItem);
      }
    }
  }
}
