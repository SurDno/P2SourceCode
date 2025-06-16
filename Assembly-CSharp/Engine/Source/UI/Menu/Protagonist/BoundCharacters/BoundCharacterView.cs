using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components.BoundCharacters;
using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.BoundCharacters;

public class BoundCharacterView : MonoBehaviour {
	[SerializeField] private Sprite fallbackSprite;
	[SerializeField] private SpriteView portrait;
	[SerializeField] private StringView nameView;
	[SerializeField] private StringView stateView;
	[SerializeField] private HideableView medicatedView;
	[SerializeField] private HideableView groupSeenView;
	[SerializeField] private Color dangerColor;
	[SerializeField] private Color diseasedColor;
	[SerializeField] private Color deadColor;
	private BoundCharacterComponent character;

	private void ApplySeenCharacterState(bool instant) {
		if (character == null)
			return;
		var resource = (BoundCharacterPlaceholder)character.Resource;
		nameView.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(character.Name);
		var str = BoundCharacterUtility.StateText(character, character.SeenBoundHealthState);
		if (character.SeenBoundHealthState == BoundHealthStateEnum.Danger)
			str = "<color=" + dangerColor.ToRGBHex() + ">" + str + "</color>";
		else if (character.SeenBoundHealthState == BoundHealthStateEnum.Diseased)
			str = "<color=" + diseasedColor.ToRGBHex() + ">" + str + "</color>";
		else if (character.SeenBoundHealthState == BoundHealthStateEnum.Dead)
			str = "<color=" + deadColor.ToRGBHex() + ">" + str + "</color>";
		stateView.StringValue = str;
		var sprite = BoundCharacterUtility.StateSprite(character, character.SeenBoundHealthState);
		if (sprite == null)
			sprite = fallbackSprite;
		portrait.SetValue(sprite, instant);
		medicatedView.Visible = BoundCharacterUtility.MedicineAttempted(character);
	}

	public bool IsGroupSeen() {
		return character.SeenGroup == character.Group;
	}

	public bool IsStateSeen() {
		return BoundCharacterUtility.PerceivedHealth(character) == character.SeenBoundHealthState;
	}

	public void MakeGroupSeen() {
		character.SeenGroup = character.Group;
		groupSeenView.Visible = true;
	}

	public void MakeStateSeen() {
		character.SeenBoundHealthState = BoundCharacterUtility.PerceivedHealth(character);
		ApplySeenCharacterState(false);
	}

	public void SetCharacter(BoundCharacterComponent value) {
		character = value;
		ApplySeenCharacterState(true);
		groupSeenView.Visible = IsGroupSeen();
		groupSeenView.SkipAnimation();
	}
}