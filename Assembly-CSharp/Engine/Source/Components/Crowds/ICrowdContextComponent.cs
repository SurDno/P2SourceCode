using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Components.Crowds
{
  public interface ICrowdContextComponent : IComponent
  {
    void RestoreState(List<IParameter> states, bool indoor);

    void StoreState(List<IParameter> states, bool indoor);
  }
}
