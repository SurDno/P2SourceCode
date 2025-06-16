// Decompiled with JetBrains decompiler
// Type: CombatCry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services;

#nullable disable
public class CombatCry
{
  public float Timeout;
  public float Radius;
  public CombatServiceCharacterInfo CryTarget;
  private CombatCryEnum cryType;
  private CombatServiceCharacterInfo character;

  public CombatCryEnum CryType => this.cryType;

  public CombatServiceCharacterInfo Character => this.character;

  public CombatCry(CombatCryEnum cryType, CombatServiceCharacterInfo character)
  {
    this.cryType = cryType;
    this.character = character;
  }
}
