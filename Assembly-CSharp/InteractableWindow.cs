// Decompiled with JetBrains decompiler
// Type: InteractableWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class InteractableWindow : MonoBehaviour
{
  [SerializeField]
  private Image blockedImage;
  [SerializeField]
  private Image lockedImage;
  [SerializeField]
  private Image normalImage;
  [SerializeField]
  private Text text;
  [SerializeField]
  private Transform textGroupContainer;
  [SerializeField]
  private Title titlePrefab;
  [SerializeField]
  private ProgressHiddenCircle progressCircle;
  private List<Title> _spawnedTitleList = new List<Title>();
  private List<GameActionType> listeners = new List<GameActionType>();
  private List<KeyValuePair<GameActionType, bool>> cachedActions;

  public void SetInfo(InteractableWindow.IconType info, string text)
  {
    this.DeactivateAllTitles();
    if (!text.IsNullOrEmpty())
    {
      this.text.gameObject.SetActive(true);
      this.text.text = text;
    }
    switch (info)
    {
      case InteractableWindow.IconType.Normal:
        this.normalImage.gameObject.SetActive(true);
        break;
      case InteractableWindow.IconType.Locked:
        this.lockedImage.gameObject.SetActive(true);
        break;
      case InteractableWindow.IconType.Blocked:
        this.blockedImage.gameObject.SetActive(true);
        break;
    }
  }

  public void SetInfo(
    InteractableWindow.IconType info,
    string[] texts,
    List<KeyValuePair<Sprite, bool>> iconSprites,
    List<KeyValuePair<GameActionType, bool>> actions)
  {
    this.normalImage.gameObject.SetActive(false);
    this.lockedImage.gameObject.SetActive(false);
    this.blockedImage.gameObject.SetActive(false);
    this.text.gameObject.SetActive(false);
    this.progressCircle.Progress = 0.0f;
    this.cachedActions = actions;
    int index = 0;
    for (int length = texts.Length; index < length || index < this._spawnedTitleList.Count; ++index)
    {
      if (index < length)
      {
        if (index >= this._spawnedTitleList.Count)
          this._spawnedTitleList.Add(Object.Instantiate<Title>(this.titlePrefab, this.textGroupContainer));
        Title spawnedTitle = this._spawnedTitleList[index];
        if (!spawnedTitle.gameObject.activeSelf)
          spawnedTitle.gameObject.SetActive(true);
        KeyValuePair<Sprite, bool> iconSprite = iconSprites[index];
        spawnedTitle.SetText(texts[index], iconSprite.Key, iconSprite.Value);
      }
      else if (this._spawnedTitleList[index].gameObject.activeSelf)
        this._spawnedTitleList[index].gameObject.SetActive(false);
    }
    this.UpdateProgress();
    switch (info)
    {
      case InteractableWindow.IconType.Normal:
        this.normalImage.gameObject.SetActive(true);
        break;
      case InteractableWindow.IconType.Locked:
        this.lockedImage.gameObject.SetActive(true);
        break;
      case InteractableWindow.IconType.Blocked:
        this.blockedImage.gameObject.SetActive(true);
        break;
    }
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.textGroupContainer.GetComponent<RectTransform>());
  }

  public void UpdateProgress()
  {
    float num = 0.0f;
    if (this.cachedActions != null)
    {
      for (int index = 0; index < this.cachedActions.Count; ++index)
      {
        KeyValuePair<GameActionType, bool> cachedAction = this.cachedActions[index];
        if (cachedAction.Value)
        {
          InputService instance = InputService.Instance;
          cachedAction = this.cachedActions[index];
          string actionWithoutHold = InputUtility.GetHotKeyNameByActionWithoutHold(cachedAction.Key);
          float holdProgress = instance.GetHoldProgress(actionWithoutHold);
          if ((double) holdProgress > (double) num)
            num = holdProgress;
        }
      }
    }
    this.progressCircle.Progress = num;
  }

  public void DeactivateAllTitles()
  {
    this.progressCircle.Progress = 0.0f;
    this.normalImage.gameObject.SetActive(false);
    this.lockedImage.gameObject.SetActive(false);
    this.blockedImage.gameObject.SetActive(false);
    this.text.gameObject.SetActive(false);
    int index = 0;
    for (int count = this._spawnedTitleList.Count; index < count; ++index)
    {
      if (this._spawnedTitleList[index].gameObject.activeSelf)
        this._spawnedTitleList[index].gameObject.SetActive(false);
    }
  }

  public enum IconType
  {
    None,
    Normal,
    Locked,
    Blocked,
  }
}
