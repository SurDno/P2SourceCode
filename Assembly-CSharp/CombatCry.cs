using Engine.Source.Services;

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
