namespace Engine.Common.Services
{
  public interface ICombatService
  {
    void AddPersonalEnemy(IEntity attacker, IEntity enemy);

    void RemovePersonalEnemy(IEntity attacker, IEntity enemy);
  }
}
