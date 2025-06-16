using System;
using Engine.Common.Components.AttackerPlayer;

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
