using Engine.Common.BoundCharacters;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.BoundCharacters
{
  public class BoundCharactersGroupView : MonoBehaviour
  {
    [SerializeField]
    private BoundCharacterView characterViewPrefab;
    [SerializeField]
    private StringView nameView;
    [SerializeField]
    private RectTransform layout;

    public void SetGroup(BoundCharacterGroup group)
    {
      if (group != 0)
      {
        this.nameView.StringValue = "{UI.Menu.Protagonist.BoundCharacterGroup." + group.ToString() + "}";
        this.nameView.gameObject.SetActive(true);
      }
      else
        this.nameView.gameObject.SetActive(false);
    }

    public BoundCharacterView AddCharacter(BoundCharacterComponent character)
    {
      BoundCharacterView boundCharacterView = Object.Instantiate<BoundCharacterView>(this.characterViewPrefab, (Transform) this.layout, false);
      boundCharacterView.SetCharacter(character);
      return boundCharacterView;
    }
  }
}
