using Engine.Source.Services;

public class CombatCry(CombatCryEnum cryType, CombatServiceCharacterInfo character) {
  public float Timeout;
  public float Radius;
  public CombatServiceCharacterInfo CryTarget;

  public CombatCryEnum CryType => cryType;

  public CombatServiceCharacterInfo Character => character;
}
