using Engine.Common;

namespace Engine.Source.Commons.Abilities
{
  public interface IAbilityProjectile
  {
    void ComputeTargets(IEntity self, IEntity item, OutsideAbilityTargets targets);
  }
}
