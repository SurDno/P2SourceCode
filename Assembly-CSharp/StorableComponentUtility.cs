using System;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

public static class StorableComponentUtility {
	public static bool IsWearable(IStorableComponent storable) {
		return storable.Groups.Contains(StorableGroup.Clothes_Ammo) ||
		       storable.Groups.Contains(StorableGroup.Clothes_Arms) ||
		       storable.Groups.Contains(StorableGroup.Clothes_Body) ||
		       storable.Groups.Contains(StorableGroup.Clothes_Feet) ||
		       storable.Groups.Contains(StorableGroup.Clothes_Head) ||
		       storable.Groups.Contains(StorableGroup.Weapons_Primary) ||
		       storable.Groups.Contains(StorableGroup.Weapons_Secondary);
	}

	public static bool IsUsable(IStorableComponent storable) {
		return storable.Groups.Contains(StorableGroup.Usable);
	}

	public static bool IsBottled(IStorableComponent storable) {
		return storable.Groups.Contains(StorableGroup.Bottled_Liquid);
	}

	public static bool IsSplittable(IStorableComponent storable) {
		return storable.Count > 1 && IsDraggable(storable);
	}

	public static bool IsDraggable(IStorableComponent storable) {
		return !storable.Groups.Contains(StorableGroup.Money) && !storable.Groups.Contains(StorableGroup.Key);
	}

	public static IStorableComponent Split(this IStorableComponent storable, int count) {
		if (count == 0 || count > storable.Count)
			throw new Exception();
		var template = (IEntity)storable.Owner.Template;
		var entity = ServiceLocator.GetService<IFactory>().Instantiate(template);
		ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
		var component = entity.GetComponent<IStorableComponent>();
		storable.Count -= count;
		if (storable.Count <= 0)
			storable.Owner.Dispose();
		component.Invoice = storable.Invoice;
		component.Max = storable.Max;
		component.Count = count;
		if (component.Count <= 0)
			component.Owner.Dispose();
		return component;
	}

	public static void Use(IStorableComponent storable) {
		((StorableComponent)storable).Use();
		--storable.Count;
		if (storable.Count > 0)
			return;
		((EngineComponent)storable).Owner.Dispose();
	}

	public static void PlayPutSound(IStorableComponent item) {
		if (item == null)
			Debug.LogError("Item is null");
		else {
			var putClip = ((StorableComponent)item).Placeholder?.SoundGroup?.GetPutClip();
			if (putClip == null)
				putClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance?.DefaultItemSoundGroup?.GetPutClip();
			MonoBehaviourInstance<UISounds>.Instance.PlaySound(putClip);
		}
	}

	public static void PlayTakeSound(IStorableComponent item) {
		if (item == null)
			Debug.LogError("Item is null");
		else {
			var takeClip = ((StorableComponent)item).Placeholder?.SoundGroup?.GetTakeClip();
			if (takeClip == null)
				takeClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultItemSoundGroup?.GetTakeClip();
			MonoBehaviourInstance<UISounds>.Instance.PlaySound(takeClip);
		}
	}

	public static void PlayUseSound(IStorableComponent item) {
		if (item == null)
			Debug.LogError("Item is null");
		else {
			var useClip = ((StorableComponent)item).Placeholder?.SoundGroup?.GetUseClip();
			if (useClip == null)
				useClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultItemSoundGroup?.GetUseClip();
			MonoBehaviourInstance<UISounds>.Instance.PlaySound(useClip);
		}
	}

	public static void PlayPourOutSound(IStorableComponent item) {
		if (item == null)
			Debug.LogError("Item is null");
		else {
			var pourOutClip = ((StorableComponent)item).Placeholder?.SoundGroup?.GetPourOutClip();
			if (pourOutClip == null)
				pourOutClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultItemSoundGroup
					?.GetPourOutClip();
			MonoBehaviourInstance<UISounds>.Instance.PlaySound(pourOutClip);
		}
	}
}