// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IAttackerPlayerComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.AttackerPlayer;
using System;

#nullable disable
namespace Engine.Common.Components
{
  public interface IAttackerPlayerComponent : IComponent
  {
    event Action<WeaponKind> WeaponUnholsterEndEvent;

    event Action<WeaponKind> WeaponHolsterStartEvent;

    void SetWeapon(WeaponKind weaponKind);

    void ResetWeapon();

    WeaponKind CurrentWeapon { get; }

    IEntity CurrentWeaponItem { get; }

    void HandsUnholster();

    void HandsHolster();

    void WeaponHandsUnholster();

    void WeaponFirearmUnholster();

    void WeaponMeleeUnholster();

    void WeaponLampUnholster();

    bool IsUnholstered { get; }
  }
}
