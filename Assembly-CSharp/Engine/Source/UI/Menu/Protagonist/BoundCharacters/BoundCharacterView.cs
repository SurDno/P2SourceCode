using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.BoundCharacters
{
  public class BoundCharacterView : MonoBehaviour
  {
    [SerializeField]
    private Sprite fallbackSprite;
    [SerializeField]
    private SpriteView portrait;
    [SerializeField]
    private StringView nameView;
    [SerializeField]
    private StringView stateView;
    [SerializeField]
    private HideableView medicatedView;
    [SerializeField]
    private HideableView groupSeenView;
    [SerializeField]
    private Color dangerColor;
    [SerializeField]
    private Color diseasedColor;
    [SerializeField]
    private Color deadColor;
    private BoundCharacterComponent character;

    private void ApplySeenCharacterState(bool instant)
    {
      if (this.character == null)
        return;
      BoundCharacterPlaceholder resource = (BoundCharacterPlaceholder) this.character.Resource;
      this.nameView.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(this.character.Name);
      string str = BoundCharacterUtility.StateText(this.character, this.character.SeenBoundHealthState);
      if (this.character.SeenBoundHealthState == BoundHealthStateEnum.Danger)
        str = "<color=" + this.dangerColor.ToRGBHex() + ">" + str + "</color>";
      else if (this.character.SeenBoundHealthState == BoundHealthStateEnum.Diseased)
        str = "<color=" + this.diseasedColor.ToRGBHex() + ">" + str + "</color>";
      else if (this.character.SeenBoundHealthState == BoundHealthStateEnum.Dead)
        str = "<color=" + this.deadColor.ToRGBHex() + ">" + str + "</color>";
      this.stateView.StringValue = str;
      Sprite sprite = BoundCharacterUtility.StateSprite(this.character, this.character.SeenBoundHealthState);
      if ((Object) sprite == (Object) null)
        sprite = this.fallbackSprite;
      this.portrait.SetValue(sprite, instant);
      this.medicatedView.Visible = BoundCharacterUtility.MedicineAttempted(this.character);
    }

    public bool IsGroupSeen() => this.character.SeenGroup == this.character.Group;

    public bool IsStateSeen()
    {
      return BoundCharacterUtility.PerceivedHealth(this.character) == this.character.SeenBoundHealthState;
    }

    public void MakeGroupSeen()
    {
      this.character.SeenGroup = this.character.Group;
      this.groupSeenView.Visible = true;
    }

    public void MakeStateSeen()
    {
      this.character.SeenBoundHealthState = BoundCharacterUtility.PerceivedHealth(this.character);
      this.ApplySeenCharacterState(false);
    }

    public void SetCharacter(BoundCharacterComponent value)
    {
      this.character = value;
      this.ApplySeenCharacterState(true);
      this.groupSeenView.Visible = this.IsGroupSeen();
      this.groupSeenView.SkipAnimation();
    }
  }
}
