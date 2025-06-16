using Engine.Behaviours.Unity.Mecanim;
using Engine.Common;
using System;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (AnimatorStateService)})]
  public class AnimatorStateService : IInitialisable
  {
    public void Initialise()
    {
    }

    public void Terminate() => PlayerAnimatorState.Clear();
  }
}
