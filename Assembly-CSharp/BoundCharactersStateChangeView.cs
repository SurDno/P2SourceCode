using System;
using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using Engine.Source.Services;

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

  private void Awake() => skipButton.onClick.AddListener(new UnityAction(Skip));

  public void Show()
  {
    foreach (BoundCharacterComponent characterComponent in ServiceLocator.GetService<BoundCharactersService>().Items)
    {
      if (characterComponent.PreRollStateStored)
        characters.Add(characterComponent);
    }
    activeView.Visible = true;
    TryNextCharacter();
  }

  private void Finish()
  {
    characters.Clear();
    activeView.Visible = false;
    Action finishedEvent = FinishedEvent;
    if (finishedEvent == null)
      return;
    finishedEvent();
  }

  private void TryNextCharacter()
  {
    if (characters.Count > 0)
    {
      BoundCharacterComponent character = characters[0];
      characters.RemoveAt(0);
      characterView = UnityEngine.Object.Instantiate<BoundCharacterStateChangeView>(characterViewPrefab, this.transform, false);
      if (onCurrentCharacterEndAction == null)
        onCurrentCharacterEndAction = OnCurrentCharacterEnd;
      characterView.FinishEvent += onCurrentCharacterEndAction;
      characterView.Show(character);
      character.PreRollStateStored = false;
    }
    else
      Finish();
  }

  private void OnCurrentCharacterEnd()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) characterView.gameObject);
    characterView = null;
    TryNextCharacter();
  }

  private void Skip() => characterView?.Skip();

  public void FinishAll()
  {
    for (int index = 0; index < characters.Count; ++index)
      characters[index].PreRollStateStored = false;
    characters.Clear();
    if (!((UnityEngine.Object) characterView != (UnityEngine.Object) null))
      return;
    characterView.Finish();
  }
}
