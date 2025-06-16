// Decompiled with JetBrains decompiler
// Type: PlayerWeaponSwitchCommand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.AttackerPlayer;

#nullable disable
public class PlayerWeaponSwitchCommand
{
  public WeaponKind WeaponKind;
  public bool SwitchOn;
  public bool IsActive;
  public IEntity item;
}
