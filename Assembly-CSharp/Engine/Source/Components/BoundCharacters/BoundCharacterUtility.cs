using Engine.Common.Commons;
using Engine.Common.Components;
using UnityEngine;

namespace Engine.Source.Components.BoundCharacters;

public static class BoundCharacterUtility {
	public static bool CharacterRegionDiscovered(BoundCharacterComponent character) {
		var component = character.HomeRegion?.GetComponent<IMapItemComponent>();
		return component == null || component.Discovered;
	}

	public static Gender GetGender(BoundCharacterComponent character) {
		var resource = (BoundCharacterPlaceholder)character.Resource;
		return resource != null ? resource.Gender : Gender.Male;
	}

	public static bool MedicineAttempted(BoundCharacterComponent character) {
		return (character.BoundHealthState.Value == BoundHealthStateEnum.Danger &&
		        character.ImmuneBoosterAttempted.Value) ||
		       (character.BoundHealthState.Value == BoundHealthStateEnum.Diseased && character.HealingAttempted.Value);
	}

	public static BoundHealthStateEnum PerceivedHealth(BoundCharacterComponent character) {
		var boundHealthStateEnum = character.BoundHealthState.Value;
		switch (boundHealthStateEnum) {
			case BoundHealthStateEnum.None:
			case BoundHealthStateEnum.__ServiceStates:
				boundHealthStateEnum = BoundHealthStateEnum.Normal;
				break;
			case BoundHealthStateEnum.TutorialPain:
			case BoundHealthStateEnum.TutorialDiagnostics:
				boundHealthStateEnum = BoundHealthStateEnum.Diseased;
				break;
		}

		if (boundHealthStateEnum == BoundHealthStateEnum.Danger && !CharacterRegionDiscovered(character))
			boundHealthStateEnum = BoundHealthStateEnum.Normal;
		return boundHealthStateEnum;
	}

	public static Sprite StateLargeSprite(
		BoundCharacterComponent character,
		BoundHealthStateEnum perceivedHealth) {
		var resource = (BoundCharacterPlaceholder)character.Resource;
		if (resource != null)
			switch (perceivedHealth) {
				case BoundHealthStateEnum.None:
					return resource.UndiscoveredNormalSprite;
				case BoundHealthStateEnum.Normal:
					return resource.LargeNormalSprite;
				case BoundHealthStateEnum.Danger:
					return resource.LargeDangerSprite;
				case BoundHealthStateEnum.Diseased:
					return resource.LargeDiseasedSprite;
				case BoundHealthStateEnum.Dead:
					return resource.LargeDeadSprite;
			}

		return null;
	}

	public static Sprite StateSprite(
		BoundCharacterComponent character,
		BoundHealthStateEnum perceivedHealth) {
		var resource = (BoundCharacterPlaceholder)character.Resource;
		if (resource != null)
			switch (perceivedHealth) {
				case BoundHealthStateEnum.None:
					return resource.UndiscoveredNormalSprite;
				case BoundHealthStateEnum.Normal:
					return resource.NormalSprite;
				case BoundHealthStateEnum.Danger:
					return resource.DangerSprite;
				case BoundHealthStateEnum.Diseased:
					return resource.DiseasedSprite;
				case BoundHealthStateEnum.Dead:
					return resource.DeadSprite;
			}

		return null;
	}

	public static string StateText(
		BoundCharacterComponent character,
		BoundHealthStateEnum percievedHealth) {
		if (percievedHealth == BoundHealthStateEnum.None || percievedHealth == BoundHealthStateEnum.Normal)
			return null;
		var gender = GetGender(character);
		return "{UI.Menu.Protagonist.BoundHealthState." + percievedHealth + "." + gender + "}";
	}
}