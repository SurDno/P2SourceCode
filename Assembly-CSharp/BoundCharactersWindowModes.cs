using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoundCharactersWindowModes : MonoBehaviour
{
  [SerializeField]
  private HideableView allCharactersView;
  [SerializeField]
  private BoundCharactersStateChangeView changedCharactersView;

  private void OnEnable()
  {
    IEnumerable<BoundCharacterComponent> items = ServiceLocator.GetService<BoundCharactersService>().Items;
    bool flag = false;
    foreach (BoundCharacterComponent characterComponent in items)
    {
      if (characterComponent.PreRollStateStored)
      {
        flag = true;
        break;
      }
    }
    if (flag)
    {
      this.changedCharactersView.FinishedEvent += new Action(this.OnChangedCharacterViewFinished);
      this.changedCharactersView.Show();
    }
    else
    {
      this.allCharactersView.Visible = true;
      this.allCharactersView.SkipAnimation();
    }
  }

  private void OnDisable()
  {
    this.changedCharactersView.FinishedEvent -= new Action(this.OnChangedCharacterViewFinished);
    this.changedCharactersView.FinishAll();
    this.allCharactersView.Visible = false;
    this.allCharactersView.SkipAnimation();
  }

  private void OnChangedCharacterViewFinished()
  {
    this.changedCharactersView.FinishedEvent -= new Action(this.OnChangedCharacterViewFinished);
    this.allCharactersView.Visible = true;
  }
}
