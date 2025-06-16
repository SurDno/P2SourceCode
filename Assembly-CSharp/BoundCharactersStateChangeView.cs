// Decompiled with JetBrains decompiler
// Type: BoundCharactersStateChangeView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
public class BoundCharactersStateChangeView : MonoBehaviour
{
  [SerializeField]
  private BoundCharacterStateChangeView characterViewPrefab;
  [SerializeField]
  private HideableView activeView;
  [SerializeField]
  private Button skipButton;
  private List<BoundCharacterComponent> characters = new List<BoundCharacterComponent>();
  private BoundCharacterStateChangeView characterView;
  private Action onCurrentCharacterEndAction;

  public event Action FinishedEvent;

  private void Awake() => this.skipButton.onClick.AddListener(new UnityAction(this.Skip));

  public void Show()
  {
    foreach (BoundCharacterComponent characterComponent in ServiceLocator.GetService<BoundCharactersService>().Items)
    {
      if (characterComponent.PreRollStateStored)
        this.characters.Add(characterComponent);
    }
    this.activeView.Visible = true;
    this.TryNextCharacter();
  }

  private void Finish()
  {
    this.characters.Clear();
    this.activeView.Visible = false;
    Action finishedEvent = this.FinishedEvent;
    if (finishedEvent == null)
      return;
    finishedEvent();
  }

  private void TryNextCharacter()
  {
    if (this.characters.Count > 0)
    {
      BoundCharacterComponent character = this.characters[0];
      this.characters.RemoveAt(0);
      this.characterView = UnityEngine.Object.Instantiate<BoundCharacterStateChangeView>(this.characterViewPrefab, this.transform, false);
      if (this.onCurrentCharacterEndAction == null)
        this.onCurrentCharacterEndAction = new Action(this.OnCurrentCharacterEnd);
      this.characterView.FinishEvent += this.onCurrentCharacterEndAction;
      this.characterView.Show(character);
      character.PreRollStateStored = false;
    }
    else
      this.Finish();
  }

  private void OnCurrentCharacterEnd()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.characterView.gameObject);
    this.characterView = (BoundCharacterStateChangeView) null;
    this.TryNextCharacter();
  }

  private void Skip() => this.characterView?.Skip();

  public void FinishAll()
  {
    for (int index = 0; index < this.characters.Count; ++index)
      this.characters[index].PreRollStateStored = false;
    this.characters.Clear();
    if (!((UnityEngine.Object) this.characterView != (UnityEngine.Object) null))
      return;
    this.characterView.Finish();
  }
}
