// Decompiled with JetBrains decompiler
// Type: StorableComponentUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using System;
using System.Linq;
using UnityEngine;

#nullable disable
public static class StorableComponentUtility
{
  public static bool IsWearable(IStorableComponent storable)
  {
    return storable.Groups.Contains<StorableGroup>(StorableGroup.Clothes_Ammo) || storable.Groups.Contains<StorableGroup>(StorableGroup.Clothes_Arms) || storable.Groups.Contains<StorableGroup>(StorableGroup.Clothes_Body) || storable.Groups.Contains<StorableGroup>(StorableGroup.Clothes_Feet) || storable.Groups.Contains<StorableGroup>(StorableGroup.Clothes_Head) || storable.Groups.Contains<StorableGroup>(StorableGroup.Weapons_Primary) || storable.Groups.Contains<StorableGroup>(StorableGroup.Weapons_Secondary);
  }

  public static bool IsUsable(IStorableComponent storable)
  {
    return storable.Groups.Contains<StorableGroup>(StorableGroup.Usable);
  }

  public static bool IsBottled(IStorableComponent storable)
  {
    return storable.Groups.Contains<StorableGroup>(StorableGroup.Bottled_Liquid);
  }

  public static bool IsSplittable(IStorableComponent storable)
  {
    return storable.Count > 1 && StorableComponentUtility.IsDraggable(storable);
  }

  public static bool IsDraggable(IStorableComponent storable)
  {
    return !storable.Groups.Contains<StorableGroup>(StorableGroup.Money) && !storable.Groups.Contains<StorableGroup>(StorableGroup.Key);
  }

  public static IStorableComponent Split(this IStorableComponent storable, int count)
  {
    if (count == 0 || count > storable.Count)
      throw new Exception();
    IEntity template = (IEntity) storable.Owner.Template;
    IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(template);
    ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
    IStorableComponent component = entity.GetComponent<IStorableComponent>();
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

  public static void Use(IStorableComponent storable)
  {
    ((StorableComponent) storable).Use();
    --storable.Count;
    if (storable.Count > 0)
      return;
    ((EngineComponent) storable).Owner.Dispose();
  }

  public static void PlayPutSound(IStorableComponent item)
  {
    if (item == null)
    {
      Debug.LogError((object) "Item is null");
    }
    else
    {
      AudioClip putClip = ((StorableComponent) item).Placeholder?.SoundGroup?.GetPutClip();
      if ((UnityEngine.Object) putClip == (UnityEngine.Object) null)
        putClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance?.DefaultItemSoundGroup?.GetPutClip();
      MonoBehaviourInstance<UISounds>.Instance.PlaySound(putClip);
    }
  }

  public static void PlayTakeSound(IStorableComponent item)
  {
    if (item == null)
    {
      Debug.LogError((object) "Item is null");
    }
    else
    {
      AudioClip takeClip = ((StorableComponent) item).Placeholder?.SoundGroup?.GetTakeClip();
      if ((UnityEngine.Object) takeClip == (UnityEngine.Object) null)
        takeClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultItemSoundGroup?.GetTakeClip();
      MonoBehaviourInstance<UISounds>.Instance.PlaySound(takeClip);
    }
  }

  public static void PlayUseSound(IStorableComponent item)
  {
    if (item == null)
    {
      Debug.LogError((object) "Item is null");
    }
    else
    {
      AudioClip useClip = ((StorableComponent) item).Placeholder?.SoundGroup?.GetUseClip();
      if ((UnityEngine.Object) useClip == (UnityEngine.Object) null)
        useClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultItemSoundGroup?.GetUseClip();
      MonoBehaviourInstance<UISounds>.Instance.PlaySound(useClip);
    }
  }

  public static void PlayPourOutSound(IStorableComponent item)
  {
    if (item == null)
    {
      Debug.LogError((object) "Item is null");
    }
    else
    {
      AudioClip pourOutClip = ((StorableComponent) item).Placeholder?.SoundGroup?.GetPourOutClip();
      if ((UnityEngine.Object) pourOutClip == (UnityEngine.Object) null)
        pourOutClip = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultItemSoundGroup?.GetPourOutClip();
      MonoBehaviourInstance<UISounds>.Instance.PlaySound(pourOutClip);
    }
  }
}
