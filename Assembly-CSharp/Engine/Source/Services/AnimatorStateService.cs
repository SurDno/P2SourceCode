using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;

namespace Engine.Source.Services
{
  [GameService(typeof (AnimatorStateService))]
  public class AnimatorStateService : IInitialisable
  {
    public void Initialise()
    {
    }

    public void Terminate() => PlayerAnimatorState.Clear();
  }
}
