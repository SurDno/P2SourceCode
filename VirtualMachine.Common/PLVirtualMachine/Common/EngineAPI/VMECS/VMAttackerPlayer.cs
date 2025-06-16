using Engine.Common.Components;
using Engine.Common.Components.AttackerPlayer;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("AttackerPlayer", typeof (IAttackerPlayerComponent))]
  public class VMAttackerPlayer : VMEngineComponent<IAttackerPlayerComponent>
  {
    public const string ComponentName = "AttackerPlayer";

    [Property("IsUnholstered", "", false, false)]
    public bool IsUnholstered => this.Component.IsUnholstered;

    [Event("Hands holstered", "weapon type")]
    public event Action<WeaponKind> HandsHolsteredEvent;

    [Event("Hands unholstered", "weapon type")]
    public event Action<WeaponKind> HandsUnholsteredEvent;

    [Method("Set weapon", "weapon type", "")]
    public void SetWeapon(WeaponKind weaponKind) => this.Component.SetWeapon(weaponKind);

    [Method("Hands unholster", "", "")]
    public void HandsUnholster() => this.Component.HandsUnholster();

    [Method("Hands holster", "", "")]
    public void HandsHolster() => this.Component.HandsHolster();

    [Method("Weapon hands unholster", "", "")]
    public void WeaponHandsUnholster() => this.Component.WeaponHandsUnholster();

    [Method("Weapon firearm unholster", "", "")]
    public void WeaponFirearmUnholster() => this.Component.WeaponFirearmUnholster();

    [Method("Weapon melee unholster", "", "")]
    public void WeaponMeleeUnholster() => this.Component.WeaponMeleeUnholster();

    [Method("Weapon lamp unholster", "", "")]
    public void WeaponLampUnholsterWeapon() => this.Component.WeaponLampUnholster();

    public void OnHandsHolstered(WeaponKind weaponKind) => this.HandsHolsteredEvent(weaponKind);

    public void OnHandsUnHolstered(WeaponKind weaponKind) => this.HandsUnholsteredEvent(weaponKind);

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.WeaponHolsterStartEvent -= new Action<WeaponKind>(this.OnHandsHolstered);
      this.Component.WeaponUnholsterEndEvent -= new Action<WeaponKind>(this.OnHandsUnHolstered);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.WeaponHolsterStartEvent += new Action<WeaponKind>(this.OnHandsHolstered);
      this.Component.WeaponUnholsterEndEvent += new Action<WeaponKind>(this.OnHandsUnHolstered);
    }
  }
}
