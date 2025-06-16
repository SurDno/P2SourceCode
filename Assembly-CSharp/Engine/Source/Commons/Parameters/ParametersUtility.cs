using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

namespace Engine.Source.Commons.Parameters
{
  public static class ParametersUtility
  {
    public static IParameter<T> GetParameter<T>(IComponent component, ParameterNameEnum name) where T : struct
    {
      return component.GetComponent<ParametersComponent>()?.GetByName<T>(name);
    }

    public static IParameter<T> GetParameter<T>(IEntity owner, ParameterNameEnum name) where T : struct
    {
      return owner.GetComponent<ParametersComponent>()?.GetByName<T>(name);
    }
  }
}
