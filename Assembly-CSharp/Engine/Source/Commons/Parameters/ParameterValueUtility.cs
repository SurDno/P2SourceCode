using Engine.Common.Components.Parameters;

namespace Engine.Source.Commons.Parameters
{
  public static class ParameterValueUtility
  {
    public static void Set<T>(this IParameterValue<T> target, IParameter<T> value) where T : struct
    {
      ((IParameterValueSet<T>) target).Set(value);
    }
  }
}
